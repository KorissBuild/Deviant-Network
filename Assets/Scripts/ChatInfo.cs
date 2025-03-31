using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class ChatInfo : MonoBehaviour
{
    public TMP_Text ChatNameText;
    public TMP_Text MemberCountText;

    public void SetInfo(RoomInfo info)
    {
        ChatNameText.text = info.Name;
        MemberCountText.text = info.PlayerCount.ToString();
    }

    public void JoinButton()
    {
        PhotonNetwork.JoinRoom(ChatNameText.text);
    }
}