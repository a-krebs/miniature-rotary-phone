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

	const float defaultSearchRadius = 5.5f;
	// only used on server
	GameObject currentObject = null;

	[SyncVar]
	bool beingCarried = false;

	// draws wire mesh to visualize slot search radius
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (new Vector3 ( transform.position.x, transform.position.y, 0 ), defaultSearchRadius);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (!isServer) {
			return;
		}
		if (beingCarried || currentObject != null) {
			Debug.Log ("Object already being carried. Ignoring collision.");
			return;
		}
		Debug.Log ("Object entered object trigger collider.");
		GameObject gameObject = other.gameObject;
		if (gameObject.tag != "Player" && gameObject.tag != "LocalPlayer") {
			Debug.Log ("Object in collider is not a player.");
			return;
		}

		currentObject = gameObject;
		GetComponent<NetworkIdentity>().AssignClientAuthority( currentObject.GetComponent<NetworkIdentity>().connectionToClient );
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (!isServer)
		{
			Debug.Log ("Not server. Ignoring collision.");
			return;
		}
		if (beingCarried) {
			Debug.Log ("Object already being carried. Ignoring collision.");
			return;
		}
		if (other.gameObject == currentObject) {
			Debug.Log ("Player exited object trigger collider.");
			GetComponent<NetworkIdentity>().RemoveClientAuthority( currentObject.GetComponent<NetworkIdentity>().connectionToClient );
			currentObject = null;
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
				// local version of the object is authoritative,
				// so remove the local transform and set position
				transform.parent = null;
				GameObject slot = GetClosestEmptySlot (defaultSearchRadius);
				if (slot != null) {
					Debug.Log ("Updating slot position.");
					transform.position = slot.transform.position;
				} else {
					Debug.Log ("No slot within range.");
					// TODO error? drop?
				}
				CmdPutDown();
			}
		}
		else
		{
			if(Input.GetMouseButtonDown(0))
			{
				CmdPickUp();
				// local version of the object is authoritative,
				// so use local transform
				transform.parent = PlayerNumber.GetLocalPlayerGameObject().transform;
			}
		}
	}

	[Command]
	void CmdPickUp()
	{
		Debug.Log ("Picking up.");
		if (currentObject == null) {
			throw new System.MemberAccessException ("Invalid state. Cannot pick up.");
		}
		beingCarried = true;
	}

	[Command]
	void CmdPutDown()
	{
		Debug.Log ("Putting down.");
		if (!beingCarried || currentObject == null) {
			throw new System.MemberAccessException ("Invalid state. Cannot put down.");
		}
		beingCarried = false;
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
			// TODO test this
			//if (collider.gameObject.GetComponent<SlotSize>().size != size) {
			//	continue;
			//}
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
