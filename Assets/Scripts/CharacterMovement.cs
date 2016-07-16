using UnityEngine;
using UnityStandardAssets._2D;
using UnityEngine.Networking;

public class CharacterMovement : NetworkBehaviour
{
	// RigidBody component instance for the player
	private Rigidbody2D playerRigidBody2D;

	//Variable to track how much movement is needed from input
	private float movePlayerVector;

	// For determining which way the player is currently facing.
	private bool facingRight;

	// Speed modifier for player movement
	public float speed = 4.0f;

	//Initialize any component references
	void Awake()
	{
		
		playerRigidBody2D = (Rigidbody2D)GetComponent(typeof(Rigidbody2D));
	
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

		if (movePlayerVector > 0 && !facingRight)
		{
			Flip();
		}
		else if (movePlayerVector < 0 && facingRight)
		{
			Flip();
		}
	}

	void Flip()
	{
		// Switch the way the player is labeled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}

