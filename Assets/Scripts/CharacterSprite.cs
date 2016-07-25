using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CharacterSprite : NetworkBehaviour {

	public Sprite sprOne;
	public Sprite sprTwo;
	public Sprite monsterOne;
	public Sprite monsterTwo;
	public Sprite placeholder;

	private SpriteRenderer rend;
	private PlayerNumber playerNum;

	// Use this for initialization
	void Start () {
		rend = GetComponent<SpriteRenderer>();
		playerNum = GetComponent<PlayerNumber>();
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
		return playerNum.IsPlayerOne();
	}

	private bool IsPlayerTwo() {
		return playerNum.IsPlayerTwo();
	}
}
