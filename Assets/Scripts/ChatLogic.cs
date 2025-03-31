using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using TMPro;
using System;
using System.IO;

public class ChatLogic : MonoBehaviourPun
{
    private string localPFPPath;

    private Transform chatContent;
    private ScrollRect chatView;

    private TMP_Text chatNameText;
    private TMP_Text memberCountText;

    private TMP_InputField messageInputField;

    private Button sendButton;
    private Button leaveButton;

    private void Awake()
    {
        localPFPPath = Application.persistentDataPath + "/ProfilePictures/profile.jpg";

        chatContent = GameObject.Find("Content").GetComponent<Transform>();
        chatView = GameObject.Find("Chat View").GetComponent<ScrollRect>();

        messageInputField = GameObject.Find("Message InputField").GetComponent<TMP_InputField>();

        sendButton = GameObject.Find("Send Button").GetComponent<Button>();
        sendButton.onClick.AddListener(SendButton);

        leaveButton = GameObject.Find("Leave Button").GetComponent<Button>();
        leaveButton.onClick.AddListener(LeaveChatButton);

        chatNameText = GameObject.Find("Chat Name Text").GetComponent<TMP_Text>();
        memberCountText = GameObject.Find("Member Count Text").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        StartCoroutine(CountMembers());

        chatNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    private IEnumerator CountMembers()
    {
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                memberCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} участник";
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && PhotonNetwork.CurrentRoom.PlayerCount < 5)
            {
                memberCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} участника";
            }
            else
            {
                memberCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} участников";
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateView()
    {
        chatView.verticalNormalizedPosition = 0f;
    }

    public void LeaveChatButton()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("Main");
    }

    public void SendButton()
    {
        if (messageInputField.text != "")
        {
            string username = DataManager.Username;
            string messageText = messageInputField.text;

            if (File.Exists(localPFPPath))
            {
                byte[] imageData = File.ReadAllBytes(localPFPPath);
                string base64String = Convert.ToBase64String(imageData);

                if (base64String.Length > 30000)
                    base64String = base64String.Substring(0, 30000);

                photonView.RPC("MessageHandler", RpcTarget.All, messageText, username, base64String);
            }
            else
                photonView.RPC("MessageHandler", RpcTarget.All, messageText, username, null);

            messageInputField.text = "";
        }
    }

    [PunRPC]
    private void MessageHandler(string receivedText, string username, string base64String)
    {
        GameObject message = Resources.Load<GameObject>("Message");

        TMP_Text messageContent = message.transform.Find("Message Text").GetComponent<TMP_Text>();
        messageContent.text = receivedText;

        TMP_Text usernameAndDate = message.transform.Find("Sender's Name Text").GetComponent<TMP_Text>();
        usernameAndDate.text = $"{username}<size=13px>    {DateTime.Now:HH:mm}</size>";

        Image userImage = message.transform.Find("Sender's Pfp Mask").Find("Sender's Pfp").GetComponent<Image>();

        if (base64String != null)
        {
            byte[] imageData = Convert.FromBase64String(base64String);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            userImage.sprite = SpriteFromTexture(texture);
        }
        else
        {
            Sprite defaultIcon = Resources.Load<Sprite>("Textures/icon_anonymous");
            userImage.sprite = defaultIcon;
        }

        Instantiate(message, chatContent);

        if (chatView.verticalNormalizedPosition < 0.3f)
            Invoke("UpdateView", 0.1f);
    }

    private Sprite SpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}