using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CharacterSprite : NetworkBehaviour {

	public Sprite sprOne;
	public Sprite sprTwo;
	public Sprite monsterOne;
	public Sprite monsterTwo;
	public Sprite placeholder;

	// player number (player 1, player 2, etc.)
	[SyncVar]
	private int playerNum;

	private SpriteRenderer rend;

	void Init() {
		playerNum = 0;
	}

	// Use this for initialization
	void Start () {
		rend = GetComponent<SpriteRenderer>();
	}

	public override void OnStartServer()
	{
		UpdatePlayerNumber();
	}

	void Update() {
		if( !isServer && !isLocalPlayer && IsPlayerOne() ) {
			rend.sprite = monsterOne;
		} else if ( !isServer && !isLocalPlayer && IsPlayerTwo() ) {
			rend.sprite = monsterTwo;
		} else if ( (isServer || isLocalPlayer) && IsPlayerOne() ) {
			rend.sprite = sprOne;
		} else if ( (isServer || isLocalPlayer) && IsPlayerTwo()) {
			rend.sprite = sprTwo;
		} else {
			rend.sprite = placeholder;
		}
	}

	private bool IsPlayerOne() {
		return playerNum == 1;
	}

	private bool IsPlayerTwo() {
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
