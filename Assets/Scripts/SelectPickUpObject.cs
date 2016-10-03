using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SelectPickUpObject : NetworkBehaviour {

	const float defaultSearchRadius = 5.5f;
	List<GameObject> availableObjects;
	List<GameObject> availableContainers;
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
		availableContainers = new List<GameObject>();
		selected = null;
		carried = null;
		cursor = GameObject.FindWithTag ("Cursor");
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (IsSelectableObject(other.gameObject)) {
			availableObjects.Add(other.gameObject);
		}
		if (IsSelectableContainer(other.gameObject)) {
			availableContainers.Add(other.gameObject);
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		if (availableObjects.Contains(other.gameObject)) {
			availableObjects.Remove(other.gameObject);
		}
		if (availableContainers.Contains(other.gameObject)) {
			availableContainers.Remove(other.gameObject);
		}
	}	

	void Update() {
		if (!isLocalPlayer)
		{
			return;
		}

		if (carried == null)
		{
			UpdateWhenNotCarrying();
		} else {
			UpdateWhenCarrying();
		}
	}

	private void UpdateWhenNotCarrying()
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
				puo.PickUp(transform, GetPickUpHandler(puo));
				Debug.Log("Picked up object.");
			} else if (selected.tag == "BoxContainer") {
				IContainer container = selected.GetComponent<BoxContainer>();
				GameObject cObj = selected;
				selected = null;
				HideCursor();
				PickUpObject puo = container.Get(transform, GetGetHandler(cObj));
				carried = (puo == null ? null : puo.gameObject);
				Debug.Log("Picked up object.");
			} else {
				// TODO error
				return;
			}
		}
	}

	private void UpdateWhenCarrying()
	{
		if (availableContainers.Count == 0 || selected == null || !availableContainers.Contains(selected)) {
			if (!UpdateSelected (null))
			{
				return;
			}
			HideCursor();
		}

		if (availableContainers.Count > 0 && selected == null) {
			if (!UpdateSelected (availableContainers[0]))
			{
				return;
			}
			// TODO remove for prod to hide cursor until selection changes
			ShowCursor (selected.transform);
		}

		if (Input.GetKeyDown (KeyCode.E) && selected != null) {
			Debug.Log("Selecting next available container.");
			int next = availableContainers.IndexOf (selected) + 1;
			if( next == availableContainers.Count )
			{
				next = 0;
			}
			if (!UpdateSelected (availableContainers[next]))
			{
				return;
			}
			ShowCursor (selected.transform);
		} else if (Input.GetKeyDown(KeyCode.Space)) {
			// put down
			Debug.Log("Going to put down object.");
			PickUpObject puo = carried.GetComponent<PickUpObject>();
			if (NotValidSlot(puo, selected)) {
				Debug.Log("Slot has wrong size.");
				return;
			}
			GameObject carrying = this.carried;
			carried = null;
			// slot might be null, but that's OK
			try {
				puo.PutDown(selected, GetPutDownHandler(carrying, selected));
			} catch {
				carried = carrying;
			}
			selected = null;
		}
	}

	private bool IsSelectableObject(GameObject obj) {
		bool valid = true;
		valid &= obj.tag == "Object" || obj.tag == "BoxContainer";
		valid &= !availableObjects.Contains(obj);
		valid &= obj.GetComponent<SpriteRenderer>().enabled;
		return valid;
	}

	private bool IsSelectableContainer(GameObject obj) {
		bool valid = true;
		valid &= !availableContainers.Contains(obj);
		if (valid && (obj.tag == "ObjectSlot" || obj.tag == "BoxContainer")) {
			IContainer container = IContainerUtils.GetIContainer(obj);
			valid &= container.Count < container.Capacity;
		} else {
			valid = false;
		}
		return valid;
	}

	private NetworkRequest.Result GetPickUpHandler(PickUpObject puo) {
		NetworkInstanceId playerNetId = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
		NetworkInstanceId objNetId    = puo.gameObject.GetComponent<NetworkIdentity>().netId;
		NetworkRequest.Result handler = delegate(bool success)
			{
				if (success) {
					if (isServer && isClient) {
						NetworkRequestService.Instance().NotifyObjectPickUp(playerNetId, objNetId);
					}
					return;
				}

				Debug.Log("SelectPickUpObject failure handler.");
				this.carried = null;
			};
		return handler;
	}

	private NetworkRequest.Result GetPutDownHandler(GameObject carrying, GameObject container) {
		NetworkInstanceId playerNetId    = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
		NetworkInstanceId objNetId       = carrying.GetComponent<NetworkIdentity>().netId;
		NetworkInstanceId containerNetId = new NetworkInstanceId(0);
		if (container != null ) {
			containerNetId = container.GetComponent<NetworkIdentity>().netId;
		}

		NetworkRequest.Result handler = delegate(bool success)
			{
				if (success) {
					if (isClient && isServer) {
						NetworkRequestService.Instance().NotifyObjectPutDown(playerNetId, objNetId, containerNetId);
					}
					return;
				}

				Debug.Log("SelectPickUpObject failure handler.");
				this.carried = carrying;
			};
		return handler;
	}

	private NetworkRequest.Result GetGetHandler(GameObject container) {
		NetworkInstanceId playerNetId    = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
		NetworkInstanceId containerNetId = new NetworkInstanceId(0);
		if (container != null ) {
			containerNetId = container.GetComponent<NetworkIdentity>().netId;
		}

		NetworkRequest.Result handler = delegate(bool success)
			{
				if (success) {
					if (isClient && isServer) {
						NetworkRequestService.Instance().NotifyContainerGet(playerNetId, containerNetId);
					}
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

	/// Returns true if the 'container' is a slot with the wrong size.
	/// Returns false otherwise.
	private bool NotValidSlot(PickUpObject puo, GameObject container) {
		if (container != null)
		{
			Slot slot = container.GetComponent<Slot>();
			if (slot != null)
			{
				Debug.Log("Slot is null");
				return false;

			}
			if (puo.size != slot.size){
				Debug.Log("Slot is not the right size");
				return false;
			}
		}
		return false;
	}
}
