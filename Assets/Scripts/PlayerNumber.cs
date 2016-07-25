using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerNumber : NetworkBehaviour {

	// player number (player 1, player 2, etc.)
	[SyncVar]
	private int playerNum;

	void Init() {
		playerNum = 0;
	}

	public override void OnStartServer()
	{
		UpdatePlayerNumber();
	}

	public bool IsPlayerOne() {
		return playerNum == 1;
	}

	public bool IsPlayerTwo() {
		return playerNum == 2;
	}

	[Server]
	private void UpdatePlayerNumber() {
		GameObject obj = GameObject.FindWithTag("PlayerNumberManager");
		if( obj == null ) {
			Debug.Log ("Failed to get PlayerNumberManager.");
		}
		PlayerNumberManager manager = obj.GetComponent<PlayerNumberManager>();
		playerNum = manager.GetPlayerNumber( netId );
	}
}
