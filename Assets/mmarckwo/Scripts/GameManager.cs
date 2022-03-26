using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{

    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("you are the master client.");
            GameObject p = null;
            p = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0, 310, 0), Quaternion.identity);
            p.name = "ClientPlayer";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
