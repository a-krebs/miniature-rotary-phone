using UnityEngine;
using UnityEngine.Networking;

public class Slot : NetworkBehaviour, IContainer {
	public PickUpObject.Size size;
	
	public int Count
	{
		get
		{
			return transform.childCount;
		}
	}

	public int Capacity
	{
		get
		{
			return 1;
		}
	}

	private PickUpObject GetChild(Transform parent)
	{
		if (Count < 1)
		{
			Debug.Log("Slot empty.");
			return null;
		}
		GameObject child = transform.GetChild(0).gameObject;
		child.transform.position = parent.position;
		child.transform.parent = parent;
		PickUpObject puo = child.GetComponent<PickUpObject>();
		puo.beingCarried = true;
		return puo;
	}

	public PickUpObject Get(Transform parent, NetworkRequest.Result handler)
	{
		if (isServer && isClient) {
			PickUpObject child = GetChild(parent);
			handler(child != null);
			return child;
		} else if (isServer) {
			PickUpObject child = GetChild(parent);
			handler(child != null);
			return child;
		} else if (isClient) {
			// TODO undo?
			PickUpObject child = GetChild(parent);
			NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
			NetworkRequestService.Instance().RequestGet(player, netId, handler);
			return child;
		} else {
			Debug.LogError("BoxContainer Get(...) called with invalid state.");
			throw new System.Exception();
		}
	}

	private void PutChild(PickUpObject obj)
	{
		if (Count >= Capacity)
		{
			Debug.Log("Slot not empty.");
			throw new System.MemberAccessException();
		}
		if (obj.size != size)
		{
			Debug.Log("Slot and object have different sizes.");
			throw new System.MemberAccessException();
		}
		obj.transform.position = transform.position;
		obj.transform.parent = transform;
	}

	public void Put(PickUpObject obj, NetworkRequest.Result handler)
	{
		if (isServer && isClient) {
			PutChild(obj);
			handler(true);
		} else if (isServer) {
			PutChild(obj);
			handler(true);
		} else if (isClient) {
			PutChild(obj);
			NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
			//NetworkRequestService.Instance().RequestPut(player, netId, handler);
			throw new System.NotImplementedException();
		} else {
			Debug.LogError("BoxContainer Put(...) called with invalid state.");
			throw new System.Exception();
		}
	}
}
