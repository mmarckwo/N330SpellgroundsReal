using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class PlayerListing : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;

    private Player player;

    public void SetPlayerInfo(Player playerInfo)
    {
        player = playerInfo;
        text.text = player.name;
    }
}
