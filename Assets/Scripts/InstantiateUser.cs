using UnityEngine;
using Photon.Pun;

public class InstantiateUser : MonoBehaviour
{
    private void Awake()
    {
        PhotonNetwork.Instantiate("User", Vector3.zero, Quaternion.identity);
    }
}