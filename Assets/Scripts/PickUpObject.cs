using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// Objects that can be picked up and moved.
///
/// Attach to objects that players should be able to pick up.
///
/// Collisions are detected on the server. Client Authority is granted to
/// the player that collides with the object. The client can then issue commands
/// to pick up the object. When the authoritative player object leaves the
/// collider, Client Authority is removed again.
public class PickUpObject : NetworkBehaviour {

	public delegate void Placed(GameObject obj, GameObject slot);
	public delegate void PickedUp(GameObject obj, GameObject slot);

	public static event Placed OnPlaced;
	public static event PickedUp OnPickedUp;

	public enum Size { Small, Medium, Large };
	public Size size;
	public bool selected = false;

	const float defaultSearchRadius = 5.5f;

	// only used on server
	GameObject currentOwner = null;

	[SyncVar]
	public bool beingCarried = false;

	// draws wire mesh to visualize slot search radius
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (new Vector3 ( transform.position.x, transform.position.y, 0 ), defaultSearchRadius);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (!isServer) {
			return;
		}
		if (beingCarried || currentOwner != null) {
			Debug.Log ("Object already being carried. Ignoring collision.");
			return;
		}
		Debug.Log ("Object entered object trigger collider.");
		GameObject gameObject = other.gameObject;
		if (gameObject.tag != "Player" && gameObject.tag != "LocalPlayer") {
			Debug.Log ("Object in collider is not a player.");
			return;
		}

		currentOwner = gameObject;
		GetComponent<NetworkIdentity>().AssignClientAuthority( currentOwner.GetComponent<NetworkIdentity>().connectionToClient );
	}
	
	void OnTriggerExit2D(Collider2D other) {
		if (!isServer)
		{
			Debug.Log ("Not server. Ignoring collision.");
			return;
		}
		if (beingCarried) {
			Debug.Log ("Object already being carried. Ignoring collision.");
			return;
		}
		if (other.gameObject == currentOwner) {
			Debug.Log ("Player exited object trigger collider.");
			GetComponent<NetworkIdentity>().RemoveClientAuthority( currentOwner.GetComponent<NetworkIdentity>().connectionToClient );
			currentOwner = null;
		}
	}	
	
	void Update()
	{
		if (!hasAuthority) {
			return;
		}
		if (beingCarried)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (!selected)
				{
					return;
				}

				GameObject slot = GetClosestEmptySlot (defaultSearchRadius);
				if (slot != null) {
					Debug.Log ("Updating slot position.");
					// local version of the object is authoritative,
					// so remove the local transform and set position
					transform.position = slot.transform.position;
					transform.parent = slot.transform;
					CmdPutDown();
					beingCarried = false;

					OnPlaced (this.gameObject, slot);
				} else {
					Debug.Log ("No slot within range.");
					// TODO error? drop?
					return;
				}
			}
		}
		else
		{
			if(Input.GetMouseButtonDown(0))
			{
				if (!selected)
				{
					return;
				}

				CmdPickUp();
				beingCarried = true;
				// local version of the object is authoritative,
				// so use local transform
				Transform localPlayer = PlayerNumber.GetLocalPlayerGameObject().transform;
				transform.position = localPlayer.position;
				transform.parent = localPlayer;

				// TODO what slot did we pick up from?
				OnPickedUp (this.gameObject, null);
			}
		}
	}

	[Command]
	void CmdPickUp()
	{
		Debug.Log ("Picking up.");
		if (currentOwner == null) {
			throw new System.MemberAccessException ("Invalid state. Cannot pick up.");
		}
	}

	[Command]
	void CmdPutDown()
	{
		Debug.Log ("Putting down.");
		if (!beingCarried || currentOwner == null) {
			throw new System.MemberAccessException ("Invalid state. Cannot put down.");
		}
	}

	/// Returns null if no game object in radius
	private GameObject GetClosestEmptySlot (float radius) {
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);
		Collider2D closest = null;
		float closestDist = 0;
		Debug.Log ("Colliders in range: " + hitColliders.Length);
		foreach (Collider2D collider in hitColliders) {
			Debug.Log ("Inspecting Collider: " + collider.gameObject.name);
			if (collider.gameObject.tag != "ObjectSlot") {
				continue;
			}
			Slot slot = collider.gameObject.GetComponent<Slot>();
			if (slot == null || slot.size != size) {
				continue;
			}
			if (slot.gameObject.transform.childCount != 0) {
				continue;
			}
			if (closest == null) {
				closest = collider;
				closestDist = Vector2.Distance (transform.position, collider.transform.position);
				continue;
			}
			float distance = Vector2.Distance (transform.position, collider.transform.position);
			if (distance < closestDist) {
				closestDist = distance;
				closest = collider;
			}
		}
		if (closest != null) {
			return closest.gameObject;
		} else {
			return null;
		}
	}
}
