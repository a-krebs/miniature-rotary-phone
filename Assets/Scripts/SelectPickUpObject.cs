using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SelectPickUpObject : NetworkBehaviour {

	const float defaultSearchRadius = 5.5f;
	List<GameObject> availableObjects;
	GameObject selected;
	GameObject carried;
	GameObject cursor;

	// draws wire mesh to visualize slot search radius
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (new Vector3 ( transform.position.x, transform.position.y, 0 ), defaultSearchRadius);
	}

	void Awake() {
		availableObjects = new List<GameObject>();
		selected = null;
		carried = null;
		cursor = GameObject.FindWithTag ("Cursor");
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (IsSelectableObject(other.gameObject))
		{
			Debug.Log("New PickUpObject or BoxContainer in range.");
			availableObjects.Add(other.gameObject);
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		if (availableObjects.Contains(other.gameObject)) {
			Debug.Log("PickUpObject or Box went out of range.");
			availableObjects.Remove(other.gameObject);
		}
	}	

	void Update() {
		if (!isLocalPlayer)
		{
			return;
		}

		if (carried == null)
		{
			if (availableObjects.Count == 0) {
				selected = null;
				HideCursor();
				return;
			}

			if (selected == null || !availableObjects.Contains(selected)) {
				if (!UpdateSelected (null))
				{
					return;
				}
				HideCursor();
			}

			if (selected == null) {
				if (!UpdateSelected (availableObjects[0]))
				{
					return;
				}
				// TODO remove for prod to hide cursor until selection changes
				ShowCursor (selected.transform);
			}

			if (Input.GetKeyDown (KeyCode.E)) {
				Debug.Log("Selecting next available object.");
				int next = availableObjects.IndexOf (selected) + 1;
				if( next == availableObjects.Count )
				{
					next = 0;
				}
				if (!UpdateSelected (availableObjects[next]))
				{
					return;
				}
				ShowCursor (selected.transform);
			} else if (Input.GetKeyDown(KeyCode.Space)) {
				Debug.Log("Going to pick up object.");
				if (selected == null) {
					Debug.Log("No object selected.");
					return;
				}
				if (selected.tag == "Object")
				{
					PickUpObject puo = selected.GetComponent<PickUpObject>();
					carried = selected;
					selected = null;
					HideCursor();
					puo.PickUp(transform, GetPickUpHandler());
					Debug.Log("Picked up object.");
				} else if (selected.tag == "BoxContainer") {
					IContainer container = selected.GetComponent<BoxContainer>();
					selected = null;
					HideCursor();
					PickUpObject puo = container.Get(transform, GetGetHandler());
					carried = (puo == null ? null : puo.gameObject);
					Debug.Log("Picked up object.");
				} else {
					// TODO error
					return;
				}
			}
		} else if (Input.GetKeyDown(KeyCode.Space)) {
			// put down
			Debug.Log("Going to put down object.");
			PickUpObject puo = carried.GetComponent<PickUpObject>();
			GameObject slot = GetClosestEmptySlot(defaultSearchRadius, puo.size);
			GameObject carrying = this.carried;
			carried = null;
			// slot might be null, but that's OK
			puo.PutDown(slot, GetPutDownHandler(carrying));
		}
	}

	private bool IsSelectableObject(GameObject obj) {
		bool valid = true;
		valid &= obj.tag == "Object" || obj.tag == "BoxContainer";
		valid &= !availableObjects.Contains(obj);
		valid &= obj.GetComponent<SpriteRenderer>().enabled;
		return valid;
	}

	private NetworkRequest.Result GetPickUpHandler() {
		NetworkRequest.Result handler = delegate(bool success)
			{
				if (success) {
					return;
				}

				Debug.Log("SelectPickUpObject failure handler.");
				this.carried = null;
			};
		return handler;
	}

	private NetworkRequest.Result GetPutDownHandler(GameObject carrying) {
		NetworkRequest.Result handler = delegate(bool success)
			{
				if (success) {
					return;
				}

				Debug.Log("SelectPickUpObject failure handler.");
				this.carried = carrying;
			};
		return handler;
	}

	private NetworkRequest.Result GetGetHandler() {
		NetworkRequest.Result handler = delegate(bool success)
			{
				if (success) {
					return;
				}

				Debug.Log("SelectPickupObject failure handler.");
				this.carried = null;
			};
		return handler;
	}

	private void HideCursor() {
		cursor.transform.parent = null;
		cursor.GetComponent<Cursor>().Hide();
	}

	private void ShowCursor( Transform parent ) {
		cursor.transform.position = parent.position;
		cursor.transform.parent = parent;
		cursor.GetComponent<Cursor>().Show();
	}

	private bool UpdateSelected (GameObject newSelected ) {
		if (selected != null && selected.tag == "Object" )
		{
			PickUpObject puo = selected.GetComponent<PickUpObject>();
			if (puo.beingCarried)
			{
				Debug.Log("Cannot select object being carried.");
				return false;
			}
		}
		selected = newSelected;
		return true;
	}

	/// Returns null if no game object in radius
	private GameObject GetClosestEmptySlot (float radius, PickUpObject.Size size) {
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
