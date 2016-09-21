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
		if (other.gameObject.tag == "Object" && !availableObjects.Contains(other.gameObject)) {
			Debug.Log("New PickUpObject in range.");
			availableObjects.Add(other.gameObject);
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		if (availableObjects.Contains(other.gameObject)) {
			Debug.Log("PickUpObject went out of range.");
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
			} else if (Input.GetMouseButtonDown(0)) {
				Debug.Log("Going to pick up object.");
				// pick up
				if (selected == null) {
					Debug.Log("No object selected.");
					return;
				}
				PickUpObject puo = selected.GetComponent<PickUpObject>();
				carried = selected;
				selected = null;
				puo.PickUp(transform, GetPickUpHandler());
				Debug.Log("Picked up object.");
			}
		} else if (Input.GetMouseButtonDown(0)) {
			// put down
			Debug.Log("Going to put down object.");
			PickUpObject puo = carried.GetComponent<PickUpObject>();
			GameObject slot = GetClosestEmptySlot(defaultSearchRadius, puo.size);
			if (slot == null) {
				//carried = null;
				//puo.PutDown(null);
			} else {
				carried = null;
				puo.PutDown(slot.transform);
			}
		}
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
		if (selected != null)
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
