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

	[Client]
	public void PickUp(NetworkRequest.Result handler)
	{
		if (beingCarried) {
			Debug.Log("Object is already being carried.");
			throw new System.Exception();
		}

		/*
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
		*/
		NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
		NetworkRequestService.Instance().RequestPickUp(player, netId, handler);

		/*
		beingCarried = true;
		transform.position = parent.position;
		transform.parent = parent;
		*/
		// TODO what slot did we pick up from?
		//OnPickedUp (this.gameObject, null);
	}

	[Client]
	public void PutDown(GameObject container, NetworkRequest.Result handler)
	{
		NetworkInstanceId containerNetId = new NetworkInstanceId(0);

		if (container != null) {
			containerNetId = container.GetComponent<NetworkIdentity>().netId;
		} 

		NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
		NetworkRequestService.Instance().RequestPutDown(player, netId, containerNetId, handler);

		//OnPlaced (this.gameObject, null);
	}

	public void UpdateParent(Transform parent, bool beingCarried)
	{
		beingCarried = beingCarried;
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
