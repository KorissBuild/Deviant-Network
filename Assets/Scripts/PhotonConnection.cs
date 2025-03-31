using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;
public class PhotonConnection : MonoBehaviourPunCallbacks
{
    public string[] availableRegions;

    public ChatInfo ChatInfo;
    public Transform Content;
    public TMP_InputField ChatNameInputField;
    public TMP_InputField FindChatInputField;
    public TMP_Dropdown RegionDropdown;
    public TMP_Text regionText;

    private bool roomListUpdated;
    private bool isInitialized;

    private MainInterface mainInterface;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "";

        mainInterface = GetComponent<MainInterface>();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("region"))
        {
            int savedRegionIdex = PlayerPrefs.GetInt("region");
            RegionDropdown.value = savedRegionIdex;

            ConnectToSavedRegion(RegionDropdown.value);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        isInitialized = true;
    }

    private void ConnectToSavedRegion(int index)
    {
        string currentRegion = availableRegions[index];

        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = currentRegion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log($"Текущий регион: {PhotonNetwork.CloudRegion}");
    }

    public void CreateChatButton()
    {
        if (ChatNameInputField.text == "")
            return;

        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = 20;
        roomOptions.IsVisible = mainInterface.isPublic;
        PhotonNetwork.JoinOrCreateRoom(ChatNameInputField.text, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Chat");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomListUpdated)
            return;

        foreach (RoomInfo info in roomList)
        {
            ChatInfo chatInfo = Instantiate(ChatInfo, Content);
            chatInfo.SetInfo(info);
        }

        roomListUpdated = true;
    }

    public void QuickJoinButton()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void SelectRegion(int index)
    {
        if (!isInitialized)
            return;

        PlayerPrefs.SetInt("region", index);

        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Main");
    }

    public void FindChatButton()
    {
        PhotonNetwork.JoinRoom(FindChatInputField.text);
    }

    public void RefreshButton()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Main");
    }
}