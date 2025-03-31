using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class DataManager : MonoBehaviour
{
    public Image profileImage;
    public TMP_Text UsernameText;

    public bool isChangeUsernamePanelOpened;
    public GameObject ChangeUsernamePanel;
    public TMP_InputField ChangeUsernameInputField;

    private FirebaseAuth firebaseAuth;
    private FirebaseUser firebaseUser;
    private DatabaseReference databaseReference;

    public static string Username;
    private string localPFPPath;

    private void Awake()
    {
        localPFPPath = Application.persistentDataPath + "/ProfilePictures/profile.jpg";
        LoadProfilePicture();

        firebaseAuth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        string email = PlayerPrefs.GetString("email");
        string pass = PlayerPrefs.GetString("password");

        StartCoroutine(GetData(email, pass));
    }

    private IEnumerator GetData(string email, string pass)
    {
        var loginTask = firebaseAuth.SignInWithEmailAndPasswordAsync(email, pass);
        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogException(loginTask.Exception);
        }
        else
        {
            firebaseUser = loginTask.Result.User;

            databaseReference.Child("users").Child(firebaseUser.UserId).Child("username").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                Username = task.Result.Value.ToString();
                UsernameText.text = $"{Username}\n<size=20px>ID: {firebaseUser.UserId}</size>";
            });
        }
    }

    public void ToggleUsernamePanelButton()
    {
        if (!isChangeUsernamePanelOpened)
        {
            isChangeUsernamePanelOpened = true;

            ChangeUsernamePanel.SetActive(true);
        }
        else
        {
            isChangeUsernamePanelOpened = false;

            ChangeUsernamePanel.SetActive(false);
            ChangeUsernameInputField.text = "";
        }
    }

    public void SubmitNewUsername()
    {
        databaseReference.Child("users").Child(firebaseUser.UserId).Child("username").SetValueAsync(ChangeUsernameInputField.text);

        Username = ChangeUsernameInputField.text;
        UsernameText.text = $"{ChangeUsernameInputField.text}\n<size=20px>ID: {firebaseUser.UserId}</size>";
        ToggleUsernamePanelButton();
    }

    public void LogOutButton()
    {
        firebaseAuth.SignOut();
        SceneManager.LoadScene("Login");
        PlayerPrefs.DeleteAll();
    }

    public void PickImageFromGallery()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                SaveImageLocally(path);
            }
        }, "Выберите изображение", "image/*");

        if (permission == NativeGallery.Permission.Denied)
        {
            Debug.LogError("Нет разрешения на использование галереи!");
        }
    }

    #region Image Compression
    private void SaveImageLocally(string sourcePath)
    {
        string directory = Application.persistentDataPath + "/ProfilePictures";

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        Texture2D texture = LoadAndCompressImage(sourcePath, 256, 256);
        byte[] compressedData = texture.EncodeToJPG(75);
        File.WriteAllBytes(localPFPPath, compressedData);

        LoadProfilePicture();
        Debug.Log("Изображение сохранено в: " + localPFPPath);
    }

    private void LoadProfilePicture()
    {
        if (File.Exists(localPFPPath))
        {
            byte[] imageData = File.ReadAllBytes(localPFPPath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            profileImage.sprite = SpriteFromTexture(texture);
        }
    }

    private Texture2D LoadAndCompressImage(string path, int width, int height)
    {
        byte[] imageData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        RenderTexture.active = renderTexture;

        Graphics.Blit(texture, renderTexture);

        Texture2D resizedTexture = new Texture2D(width, height);
        resizedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resizedTexture.Apply();

        RenderTexture.active = null;
        renderTexture.Release();
        return resizedTexture;
    }

    private Sprite SpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    #endregion
}