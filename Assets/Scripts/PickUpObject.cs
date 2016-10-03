using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using NetworkRequest;

/// Objects that can be picked up and moved.
///
/// Attach this component to objects that players should be able to pick up.
public class PickUpObject : NetworkBehaviour
{
	public delegate void Placed(GameObject obj, GameObject slot);
	public delegate void PickedUp(GameObject obj, GameObject slot);

    /// Called when PickUpObject is set down. Param 'slot' might be null
    /// if object is set on ground or into box.
    public static event Placed OnPlaced;

	/// Called when PickUpObject is picked up. Param 'slot' might be null
	/// if object is picked up from the ground or a box.
	public static event PickedUp OnPickedUp;

	public enum Size { Small, Medium, Large };
	public Size size;

	[SyncVar]
	public bool beingCarried = false;


    /// Actually pick up the PickUpObject, assigning the new parent.
    private void PickUpInternal(Transform parent)
	{
		if (beingCarried) {
			Debug.Log("Object is already being carried.");
			throw new System.Exception();
		}

		UpdateParent(parent, true);

	}

	/// Pick up the object.
	///
	/// If this is called on a network client, a network request is made,
	/// then the object is picked up. If the request fails later, the pick
	/// up action is reverted.
	///
	/// If this is called on a server or host, the object is picked up.
	public void PickUp(Transform parent, NetworkRequest.Result handler)
	{
		if (beingCarried) {
			Debug.Log("Object is already being carried.");
			throw new System.Exception();
		}

		if (isClient && isServer) {
			PickUpInternal(parent);
			handler(true);
		} else if (isServer) {
			PickUpInternal(parent);
			handler(true);
		} else if (isClient) {
			Transform previousParent = transform.parent;
			Result internalHandler = delegate (bool success)
				{
					if(!success)
					{
						Debug.Log("PickUpObject.PickedUp failure handler.");
						this.UpdateParent(previousParent, false);
					}
					handler(success);
				};

			NetworkInstanceId playerNetId = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
			Debug.Log("Picking up PickUpObject, player with netId " + playerNetId + ", object with netId " + netId.Value);
			PickUpInternal(parent);
			NetworkRequestService.Instance().RequestObjectPickUp(playerNetId, netId, internalHandler);
		} else {
			Debug.LogError("PickUpObject PickUp(...) called with invalid state.");
			throw new System.Exception();
		}
	}

	// Actually place the PickUpObject into the container, if enough Capacity.
	private void PutDownInternal(GameObject container)
	{
		if (container != null) {
			IContainer c = IContainerUtils.GetIContainer(container);
			if (c.Count >= c.Capacity) {
				Debug.Log("Container full.");
				throw new System.Exception();
			}
			UpdateParent(container.transform, false);
		} else {
			UpdateParent(null, false);
		}
	}

	/// Put down the object.
	///
	/// If the 'container' is null, place the object on the ground.
	///
	/// If this is called on a network client, a network request is made,
	/// then the object is put down. If the request fails later, the
	/// action is reverted.
	///
	/// If this is called on a server or host, the object is placed down.
	public void PutDown(GameObject container, NetworkRequest.Result handler)
	{
		NetworkInstanceId containerNetId = new NetworkInstanceId(0);

		if (container != null) {
			containerNetId = container.GetComponent<NetworkIdentity>().netId;
		}

		if (isClient && isServer) {
			PutDownInternal(container);
			handler(true);
		} else if (isServer) {
			PutDownInternal(container);
			handler(true);
		} else if (isClient) {

			Transform previousParent = transform.parent;
			Result internalHandler = delegate (bool success)
				{
					if(!success)
					{
						Debug.Log("PickUpObject.PutDown failure handler.");
						this.UpdateParent(previousParent, true);
					}
					handler(success);
				};

			NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
			Debug.Log("Putting down PickUpObject, player with netId " + player.Value + ", object with netId " + netId.Value + ", container netId: " + containerNetId.Value);
			PutDownInternal(container);
			NetworkRequestService.Instance().RequestObjectPutDown(player, netId, containerNetId, internalHandler);
		} else {
			Debug.LogError("PickUpObject PutDown(...) called with invalid state.");
			throw new System.Exception();
		}
	}

	/// Update the PickUpObject's parent transform, and set 'beingCarried'.
	public void UpdateParent(Transform parent, bool beingCarried)
	{
		this.beingCarried = beingCarried;

		Transform oldParent = transform.parent;

		UpdateSortingOrder(parent);

		if (parent != null) {
			transform.position = parent.position;
		} else {
			GameObject ground = GameObject.FindWithTag("EdgeCollider");
			// this relies on first BoxCollider2D component being the 'floor'
			Bounds bounds = ground.GetComponent<BoxCollider2D>().bounds;
			Vector3 closest = bounds.ClosestPoint(new Vector3(transform.position.x, transform.position.y, 0));
			// add the size of the floor collider to raise the object off of the ground slightly
			transform.position = new Vector2(transform.position.x, closest.y + bounds.size.y);
		}
		transform.parent = parent;

		// fire events last so that event consumers have the current state of the PickUpObject
		FireEvents(oldParent, parent);
	}

	private void UpdateSortingOrder(Transform newParent)
	{
		SpriteRenderer rend = GetComponent<SpriteRenderer>();

		if (newParent != null && newParent.gameObject.tag == "LocalPlayer") {
			rend.sortingOrder = 13; // magic numbers yaaay (this is 1 higher than the LocalPlayer's sort order (see CharacterSprite.cs))
		} else if (newParent != null && newParent.gameObject.tag == "Player") {
			rend.sortingOrder = 11; // this is one higher than the Player prefab's sort order
		} else {
			rend.sortingOrder = 5; // back to PickUpObject prefab default sort order
		}
	}

	private void FireEvents(Transform oldParent, Transform newParent)
	{
		GameObject newParentObj = newParent != null ? newParent.gameObject : null;

		if (newParentObj != null && (newParentObj.tag == "LocalPlayer" || newParentObj.tag == "Player")) {
			// OnPickedUp
			if (oldParent != null) {
				GameObject obj = oldParent.gameObject;
				if (obj.GetComponent<Slot>() != null && OnPickedUp != null) {
					OnPickedUp (this.gameObject, obj);
				}
			} else if (OnPickedUp != null) {
				OnPickedUp (this.gameObject, null);
			}
		} else {
			// OnPlaced
			if (OnPlaced != null) {
				if (newParentObj != null && newParentObj.GetComponent<Slot>() != null) {
					OnPlaced (this.gameObject, newParentObj);
				} else {
					OnPlaced (this.gameObject, null);
				}
			}
		}
	}
}
