using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CharacterSprite : NetworkBehaviour {

	public Sprite sprGal;
	public Sprite sprDude;
	public Sprite sprTeeth;
	public Sprite sprBurb;
	public Sprite placeholder;

	private SpriteRenderer rend;
	private PlayerNumber playerNum;
	private Animator animator;

	// Use this for initialization
	void Start () {
		rend = GetComponent<SpriteRenderer>();
		playerNum = GetComponent<PlayerNumber>();
		animator = GetComponent<Animator>();
	}

	void Update() {
		if( !isLocalPlayer && IsPlayerOne() ) {
			rend.sprite = sprBurb;
			animator.runtimeAnimatorController = Resources.Load("Animation/burb") as RuntimeAnimatorController;
		} else if ( !isLocalPlayer && IsPlayerTwo() ) {
			rend.sprite = sprTeeth;
			animator.runtimeAnimatorController = Resources.Load("Animation/teethlegs") as RuntimeAnimatorController;
		} else if ( isLocalPlayer && IsPlayerOne() ) {
			rend.sprite = sprGal;
			animator.runtimeAnimatorController = Resources.Load("Animation/gal") as RuntimeAnimatorController;
		} else if ( isLocalPlayer && IsPlayerTwo()) {
			rend.sprite = sprDude;
			animator.runtimeAnimatorController = Resources.Load("Animation/dude") as RuntimeAnimatorController;
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
