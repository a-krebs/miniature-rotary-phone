using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.Networking;

public class CharacterMovement : NetworkBehaviour
{
	// RigidBody component instance for the player
	private Rigidbody2D playerRigidBody2D;

	//Variable to track how much movement is needed from input
	private float movePlayerVector;


	// Speed modifier for player movement
	public float speed = 4.0f;
	private Animator anim;
	//Initialize any component references
	void Awake()
	{
		playerRigidBody2D = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
		anim = GetComponent<Animator> ();
		anim.speed = 0.5f;
	}

	public override void OnStartLocalPlayer (){
		Debug.Log ("On start local player has been called!");
		GameObject camera = GameObject.FindWithTag ("MainCamera");
		if (camera != null) {
			Camera2DFollow followScript = camera.GetComponent ("Camera2DFollow") as Camera2DFollow;
			if (followScript != null) {
				followScript.target = this.transform;
			} else {
				Debug.Log ("followScript is null.");
			}
		} else {
			Debug.Log ("Camera is null.");
		}
	}

	// Update is called once per frame
	void Update () {
		// Get the horizontal input.
		if (!isLocalPlayer) {
			return;
		}
		movePlayerVector = Input.GetAxis("Horizontal");

		playerRigidBody2D.velocity = new Vector2(movePlayerVector * speed, playerRigidBody2D.velocity.y);

		anim.SetFloat("speed", movePlayerVector);


	}

}

