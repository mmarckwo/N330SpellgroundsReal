using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;

    private GameObject p1Spawn;
    private GameObject p2Spawn;

    public Player masterPlayer = null;
	public Player serverPlayer = null;
	
	public bool playerIsMaster;
	
	public bool ShouldntUpdate(Player player){
		
		if(!player.photonView.IsMine) return(true);
		
		return((this.masterPlayer == null) || (this.serverPlayer == null));
		
	}
	
	private Player SpawnPlayer(string name, bool isMaster, GameObject spawn){
		
		
		object[] customData = new object[1];
		customData[0] = isMaster;
		
		
		GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, spawn.transform.position, spawn.transform.rotation, 0, customData);
		
		player.name = name;
		//player.tag = "Player";
		player.transform.Find("Player").tag = "Player";
		
		Player newPlayer = player.GetComponentInChildren<Player>();
		newPlayer.gameManager = this;
		
		return(newPlayer);
		
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
			
			this.masterPlayer = SpawnPlayer("ClientPlayer",true,p1Spawn);
			
			playerIsMaster = true;
			
        }
        else
        {
            Debug.Log("You are not the master client.");
            
			/*serverPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, p2Spawn.transform.position, p2Spawn.transform.rotation).GetComponent<Player>();
            serverPlayer.GameObject.name = "EnemyPlayer";
            serverPlayer.GameObject.tag = "Player";
			serverPlayer.isMaster = false;*/
			
			this.serverPlayer = SpawnPlayer("EnemyPlayer",false,p2Spawn);
			
			playerIsMaster = false;
			
        }
		
		//masterPlayer.isMaster = false;
		
    }
	
	public void EndScreen(string playerType){
		
		// playerType is either Player or Enemy.
		if(playerType == "Enemy"){
			
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
