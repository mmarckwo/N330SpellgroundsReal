using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class RoomListing : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;

    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        text.text = roomInfo.Name;
    }

    public void Click_JoinRoom()
    {
        PhotonNetwork.JoinRoom(RoomInfo.Name);
    }
}
