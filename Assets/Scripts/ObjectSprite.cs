using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ObjectSprite : NetworkBehaviour {

	public Sprite sprOne;
	public Sprite sprTwo;
	public Sprite placeholder;

	private SpriteRenderer rend;
	private PlayerNumber playerNum;

	// Use this for initialization
	void Start () {
		rend = GetComponent<SpriteRenderer>();
		playerNum = PlayerNumber.GetLocalPlayerNumber();
	}
	
	// Update is called once per frame
	void Update () {
		if (playerNum == null)
		{
			playerNum = PlayerNumber.GetLocalPlayerNumber();
			return;
		}
		if (playerNum.IsPlayerOne()) {
			rend.sprite = sprOne;
		} else if (playerNum.IsPlayerTwo()) {
			rend.sprite = sprTwo;
		} else {
			rend.sprite = placeholder;
		}
	}
}
