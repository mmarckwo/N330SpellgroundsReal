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
        playerCountText.SetText("Connected players: " + count.ToString());
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        count--;
        playerCountText.SetText("Connect players: " + count.ToString());
    }

    // need to put a punrpc in here?
}
