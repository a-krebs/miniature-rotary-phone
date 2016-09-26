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

	public static event Placed OnPlaced;
	public static event PickedUp OnPickedUp;

	public enum Size { Small, Medium, Large };
	public Size size;

	[SyncVar]
	public bool beingCarried = false;

	private void PickUpInternal(Transform parent)
	{
		if (beingCarried) {
			Debug.Log("Object is already being carried.");
			throw new System.Exception();
		}

		UpdateParent(parent, true);

		// TODO what slot did we pick up from?
		//OnPickedUp (this.gameObject, null);
	}

	public void PickUp(Transform parent, NetworkRequest.Result handler)
	{
		if (beingCarried) {
			Debug.Log("Object is already being carried.");
			throw new System.Exception();
		}

		//Transform parent = player.transform;

		if (isClient && isServer) {
			PickUpInternal(parent);
			handler(true);
		} else if (isServer) {
			PickUpInternal(parent);
			handler(true);
		} else if (isClient) {
			GameObject player = PlayerNumber.GetLocalPlayerGameObject();
			NetworkInstanceId playerNetId = player.GetComponent<NetworkIdentity>().netId;
			Transform previousParent = transform.parent;
			Vector2 previousPosition = transform.position;
			Result internalHandler = delegate (bool success)
				{
					if(!success)
					{
						Debug.Log("PickUpObject.PickedUp failure handler.");
						this.beingCarried = false;
						this.transform.position = previousPosition;
						this.transform.parent = previousParent;
					}
					handler(success);
				};
			Debug.Log("Picking up PickUpObject, player with netId " + playerNetId + ", object with netId " + netId.Value);
			PickUpInternal(parent);
			NetworkRequestService.Instance().RequestPickUp(playerNetId, netId, internalHandler);
		} else {
			Debug.LogError("PickUpObject PickUp(...) called with invalid state.");
			throw new System.Exception();
		}

		// TODO what slot did we pick up from?
		//OnPickedUp (this.gameObject, null);
	}

	private void PutDownInternal(GameObject container)
	{
		if (container != null) {
			IContainer c = GetIContainer(container);
			if (c.Count >= c.Capacity) {
				Debug.Log("Container full.");
				throw new System.Exception();
			}
			UpdateParent(container.transform, false);
		} else {
			UpdateParent(null, false);
		}
	}

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
			NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;

			Transform previousParent = transform.parent;
			Vector2 previousPosition = transform.position;
			Result internalHandler = delegate (bool success)
				{
					if(!success)
					{
						Debug.Log("PickUpObject.PickedUp failure handler.");
						this.beingCarried = false;
						this.transform.position = previousPosition;
						this.transform.parent = previousParent;
					}
					handler(success);
				};

			Debug.Log("Putting down PickUpObject, player with netId " + player.Value + ", object with netId " + netId.Value + ", container netId: " + containerNetId.Value);
			PutDownInternal(container);
			NetworkRequestService.Instance().RequestPutDown(player, netId, containerNetId, internalHandler);
		} else {
			Debug.LogError("PickUpObject PutDown(...) called with invalid state.");
			throw new System.Exception();
		}


		//OnPlaced (this.gameObject, null);
	}

	public void UpdateParent(Transform parent, bool beingCarried)
	{
		this.beingCarried = beingCarried;
		if (parent != null) {
			transform.position = parent.position;
		} else {
			// TODO set on ground
		}
		transform.parent = parent;
	}

	public static IContainer GetIContainer(GameObject containerGameObj)
	{
		IContainer containerInstance = null;
		if (containerGameObj.tag == "BoxContainer")
		{
			Debug.Log("Put into BoxContainer.");
			BoxContainer box = containerGameObj.GetComponent<BoxContainer>();
			containerInstance = box;
		} else if (containerGameObj.tag == "ObjectSlot") {
			Debug.Log("Put into ObjectSlot.");
			Slot slot = containerGameObj.GetComponent<Slot>();
			containerInstance = slot;
		}

		return containerInstance;
	}
}
