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
		try
		{
			playerNum = PlayerNumber.GetLocalPlayerNumber();
		} catch {
			playerNum = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isServer && !isClient)
		{
			// One of the clients might be a server too.
			// If dedicated server, use placeholder.
			return;
		}
		if (playerNum == null)
		{
			try
			{
				playerNum = PlayerNumber.GetLocalPlayerNumber();
			} catch {
				playerNum = null;
			}
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
