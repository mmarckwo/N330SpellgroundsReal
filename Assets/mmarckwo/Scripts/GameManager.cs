using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    private GameObject p1Spawn;
    private GameObject p2Spawn;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // find initial spawn locations in game world.
        p1Spawn = GameObject.Find("P1 Spawn");
        p2Spawn = GameObject.Find("P2 Spawn");

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("you are the master client.");
            player = PhotonNetwork.Instantiate(this.playerPrefab.name, p1Spawn.transform.position, Quaternion.identity);
            player.name = "ClientPlayer";
            player.tag = "Player";
        } 
        else
        {
            Debug.Log("you are not the master client.");
            player = PhotonNetwork.Instantiate(this.playerPrefab.name, p2Spawn.transform.position, Quaternion.identity);
            player.name = "EnemyPlayer";
            player.tag = "Player";
        }
    }
}
