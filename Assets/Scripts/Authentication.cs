using UnityEngine;
using System.Collections;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine.SceneManagement;

public class Authentication : MonoBehaviour
{
    [Header("Firebase")]
    private FirebaseAuth firebaseAuth;
    private FirebaseUser firebaseUser;
    private DatabaseReference databaseReference;

    public CurrentState currentState;
    public enum CurrentState { Registration, Login}

    [Header("Texts")]
    public TMP_Text appNameText;
    public TMP_Text switchStateText;
    public TMP_Text errorHandlerText;

    [Header("Login")]
    public TMP_InputField emailLogin;
    public TMP_InputField passLogin;

    [Header("Registration")]
    public TMP_InputField nameRegister;
    public TMP_InputField emailRegister;
    public TMP_InputField passRegister;
    public TMP_InputField confirmRegister;

    [Header("Other")]
    public GameObject[] StateList;
    public GameObject StartPanel;

    private void Awake()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password"))
        {
            LoginByCache(PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password"));
        }
        else
        {
            Destroy(StartPanel);
        }
    }

    private void LoginByCache(string email, string password)
    {
        StartCoroutine(Login(email, password));
    }

    public void ToggleStateButton()
    {
        if (currentState == CurrentState.Registration)
        {
            currentState = CurrentState.Login;
            StateList[0].SetActive(false);
            StateList[1].SetActive(true);

            appNameText.text = "Deviant Network\n<size=20px>LOGIN</size>";
            switchStateText.text = "Нет аккаунта? Создай здесь!";
        }
        else
        {
            currentState = CurrentState.Registration;
            StateList[0].SetActive(true);
            StateList[1].SetActive(false);

            appNameText.text = "Deviant Network\n<size=20px>REGISTRATION</size>";
            switchStateText.text = "Уже есть аккаунт?";
        }
    }

    public void RegisterationButton()
    {
        StartCoroutine(Registration(nameRegister.text, emailRegister.text, passRegister.text, confirmRegister.text));
    }

    public void LoginButton()
    {
        StartCoroutine(Login(emailLogin.text, passLogin.text));
    }

    private IEnumerator Registration(string username, string email, string password, string confirmedPassword)
    {
        if (username == "")
        {
            errorHandlerText.text = "Error: missing username!";
            errorHandlerText.color = Color.red;
            yield return new WaitForSeconds(5f);
            errorHandlerText.text = "";
        }
        else if (password != confirmedPassword)
        {
            errorHandlerText.text = "Error: password does not match!";
            errorHandlerText.color = Color.red;
            yield return new WaitForSeconds(5f);
            errorHandlerText.text = "";
        }
        else
        {
            var RegisterTask = firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "register failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "missing email!";
                        break;
                    case AuthError.MissingPassword:
                        message = "missing password!";
                        break;
                    case AuthError.WeakPassword:
                        message = "weak password!";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "email already in use!";
                        break;
                }
                errorHandlerText.text = $"Error: {message}";
                errorHandlerText.color = Color.red;
                yield return new WaitForSeconds(5f);
                errorHandlerText.text = "";
            }
            else
            {
                firebaseUser = RegisterTask.Result.User;

                UserProfile profile = new UserProfile();

                var ProfileTask = firebaseUser.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                AddValuesInBase();
                StateList[0].SetActive(false);
                StateList[1].SetActive(true);

                if (ProfileTask.Exception != null)
                {
                    FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                    errorHandlerText.text = "Error: firebaseAuthname set failed!";
                    errorHandlerText.color = Color.red;
                    yield return new WaitForSeconds(5f);
                    errorHandlerText.text = "";
                }
                else
                {
                    errorHandlerText.text = "Account is created! Log in";
                    errorHandlerText.color = Color.green;
                    yield return new WaitForSeconds(5f);
                    errorHandlerText.text = "";
                }
            }
        }
    }

    private void AddValuesInBase()
    {
        databaseReference.Child("users").Child(firebaseUser.UserId).Child("username").SetValueAsync(nameRegister.text);
    }

    private IEnumerator Login(string email, string password)
    {
        var LoginTask = firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            Destroy(StartPanel);
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "login failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "missing email!";
                    break;
                case AuthError.MissingPassword:
                    message = "missing password!";
                    break;
                case AuthError.WrongPassword:
                    message = "wrong password!";
                    break;
                case AuthError.InvalidEmail:
                    message = "invalid email!";
                    break;
                case AuthError.UserNotFound:
                    message = "account does not exist!";
                    break;
                case AuthError.UserDisabled:
                    message = "account is banned!";
                    break;
            }
            errorHandlerText.text = $"Error: {message}";
            errorHandlerText.color = Color.red;
            yield return new WaitForSeconds(5f);
            errorHandlerText.text = "";
        }
        else
        {
            firebaseUser = LoginTask.Result.User;
            errorHandlerText.text = "";
            PlayerPrefs.SetString("email", email);
            PlayerPrefs.SetString("password", password);
            SceneManager.LoadScene("Main");
        }
    }
}