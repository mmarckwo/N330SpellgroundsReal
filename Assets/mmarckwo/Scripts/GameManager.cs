using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    private GameObject p1Spawn;
    private GameObject p2Spawn;

    public Player masterPlayer;
	public Player serverPlayer;
	
	public bool playerIsMaster;
	
	private void SpawnPlayer(string name, bool isMaster, GameObject spawn){
		
		
		GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, spawn.transform.position, spawn.transform.rotation);
		
		player.name = name;
		player.tag = "Player";
		
		masterPlayer = player.GetComponent<Player>();
		masterPlayer.isMaster = isMaster;
		masterPlayer.gameManager = this;
		
	}
	
    // Start is called before the first frame update
    void Start()
    {
        // find initial spawn locations in game world.
        p1Spawn = GameObject.Find("P1 Spawn");
        p2Spawn = GameObject.Find("P2 Spawn");

        // stop syncing scenes because players will be sent to win or lose screens after the battle.
        PhotonNetwork.AutomaticallySyncScene = false;

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("You are the master client.");
			
			SpawnPlayer("ClientPlayer",true,p1Spawn);
			
			playerIsMaster = true;
			
        }
        else
        {
            Debug.Log("You are not the master client.");
            
			/*serverPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, p2Spawn.transform.position, p2Spawn.transform.rotation).GetComponent<Player>();
            serverPlayer.GameObject.name = "EnemyPlayer";
            serverPlayer.GameObject.tag = "Player";
			serverPlayer.isMaster = false;*/
			
			SpawnPlayer("EnemyPlayer",false,p2Spawn);
			
			playerIsMaster = false;
			
        }
		
		//masterPlayer.isMaster = false;
		
    }
	
	public void EndScreen(bool won){
		
		if(won){
			
			Debug.Log("player wins");
			Cursor.lockState = CursorLockMode.Confined;
			SceneManager.LoadScene("YouWin");
			
		}else{
			
			Debug.Log("enemy wins");
			Cursor.lockState = CursorLockMode.Confined;
			SceneManager.LoadScene("YouLose");
			
		}
		
	}
	
}
