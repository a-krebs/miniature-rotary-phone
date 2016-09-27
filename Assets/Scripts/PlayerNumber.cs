using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// Gets a number for the player.
///
/// Requires a PlayerNumberManager to be available on the server.
/// Must be attached to the local player object.
///
/// Use static method `GetLocalPlayerNumber()` to get local player number.
/// Use static method `GetLocalPlayerGameObject()` to get the local player's GameObject.
public class PlayerNumber : NetworkBehaviour {

	// player number (player 1, player 2, etc.)
	[SyncVar]
	private int playerNum;

	void Awake () {
		Debug.Log ("PlayerNumber Awake.");
		playerNum = 0;
	}

	public override void OnStartClient () {
		Debug.Log ("PlayerNumber OnStartClient.");
	}

	public override void OnStartLocalPlayer () {
		Debug.Log ("PlayerNumber OnStartLocalPlayer.");
		gameObject.tag = "LocalPlayer";
	}

	public override void OnStartServer () {
		Debug.Log ("PlayerNumber OnStartServer.");
		UpdatePlayerNumber();
	}

	public bool IsPlayerOne () {
		ThrowIfPlayerNumberNotSet();
		return playerNum == 1;
	}

	public bool IsPlayerTwo () {
		ThrowIfPlayerNumberNotSet();
		return playerNum == 2;
	}

	private void ThrowIfPlayerNumberNotSet() {
		if (playerNum == 0) {
			throw new System.MemberAccessException ("Player number not initialized.");
		}
	}

	[Server]
	private void UpdatePlayerNumber () {
		Debug.Log ("PlayerNumber UpdatePlayerNumber.");
		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		playerNum = manager.GetPlayerNumber( netId, connectionToClient );
		Debug.Log ("Updated player number. NetId: " + netId + ", PlayerNum: " + playerNum );
	}

	public static PlayerNumber GetLocalPlayerNumber() {
		PlayerNumber num = GetLocalPlayerGameObject().GetComponent<PlayerNumber>();
		if (num == null) {
			throw new System.MemberAccessException ("Local player found, but has no PlayerNumber Component.");
		}

		return num;
	}

	public static GameObject GetLocalPlayerGameObject() {
		GameObject player = GameObject.FindWithTag ("LocalPlayer");
		if (player == null) {
			throw new System.MemberAccessException ("Local player not found.");
		}
		return player;
	}
}
