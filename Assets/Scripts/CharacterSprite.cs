using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CharacterSprite : NetworkBehaviour {


	public Sprite placeholder;
    public Sprite greenBackground;
    public Sprite pinkBackground;

	private SpriteRenderer rend;
	private PlayerNumber playerNum;
	private Animator animator;
    public SpriteRenderer background;

	// Use this for initialization
	void Start () {
		rend = GetComponent<SpriteRenderer>();
		playerNum = GetComponent<PlayerNumber>();
		animator = GetComponent<Animator>();
        background = GameObject.Find("env_room_background").GetComponent<SpriteRenderer>();
	}

	void Update() {
		if( !isLocalPlayer && IsPlayerOne() ) {
			animator.runtimeAnimatorController = Resources.Load("Animation/burb") as RuntimeAnimatorController;

		} else if ( !isLocalPlayer && IsPlayerTwo() ) {
			animator.runtimeAnimatorController = Resources.Load("Animation/teethlegs") as RuntimeAnimatorController;

		} else if ( isLocalPlayer && IsPlayerOne() ) {
      		background.sprite = pinkBackground;
			rend.sortingOrder = 12;
			animator.runtimeAnimatorController = Resources.Load("Animation/gal") as RuntimeAnimatorController;

		} else if ( isLocalPlayer && IsPlayerTwo()) {

      		background.sprite = greenBackground;
			rend.sortingOrder = 12;
			animator.runtimeAnimatorController = Resources.Load("Animation/dude") as RuntimeAnimatorController;

		} else {
			background.sprite = greenBackground;
			rend.sortingOrder = 12;
			animator.runtimeAnimatorController = Resources.Load("Animation/dude") as RuntimeAnimatorController;
		}
	}

	private bool IsPlayerOne() {
		return playerNum.IsPlayerOne();
	}

	private bool IsPlayerTwo() {
		return playerNum.IsPlayerTwo();
	}
}
