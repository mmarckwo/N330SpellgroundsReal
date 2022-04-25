using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{

    [SerializeField] private TextMeshProUGUI playerCountText;

    // there will always be at least 1 player connected.
    private int count = 1;

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        count++;
        this.photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered, count);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        count--;
        this.photonView.RPC("UpdatePlayerCount", RpcTarget.AllBuffered, count);
    }

    [PunRPC]
    private void UpdatePlayerCount(int playerCount)
    {
        count = playerCount;
        playerCountText.SetText("Connected players: " + count);
    }
}
