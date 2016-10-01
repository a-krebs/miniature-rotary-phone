using UnityEngine;
using UnityEngine.Networking;

public class Slot : NetworkBehaviour, IContainer {

	public enum GoodFor { Player1, Player2, Both, None };

	public PickUpObject.Size size;
	public GoodFor goodFor;
	
	public int Count
	{
		get
		{
			return IContainerUtils.GetChildCount(transform);
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
		PickUpObject puo = child.GetComponent<PickUpObject>();
		puo.UpdateParent(parent, true);
		return puo;
	}

	public PickUpObject Get(Transform parent, NetworkRequest.Result handler)
	{
		if (isServer && isClient) {
			PickUpObject child = GetChild(parent);
			handler(child != null);
			if (child != null) {
				NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
				NetworkInstanceId childNetId = child.gameObject.GetComponent<NetworkIdentity>().netId;
				NetworkRequestService.Instance().NotifyContainerGet(player, childNetId, netId);
			}
			return child;
		} else if (isServer) {
			PickUpObject child = GetChild(parent);
			handler(child != null);
			return child;
		} else if (isClient) {
			PickUpObject child = GetChild(parent);
			NetworkRequest.Result internalHandler = delegate (bool success)
				{
					if(!success)
					{
						Debug.Log("BoxContainer.Get failure handler.");
						this.PutChild(child);
					}
					handler(success);
				};
			NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
			NetworkRequestService.Instance().RequestContainerGet(player, netId, internalHandler);
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
		obj.UpdateParent(transform, false);
	}

	public void Put(PickUpObject obj, NetworkRequest.Result handler)
	{
		if (isServer && isClient) {
			PutChild(obj);
			handler(true);
			NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
			NetworkInstanceId objNetId = obj.gameObject.GetComponent<NetworkIdentity>().netId;
			NetworkRequestService.Instance().NotifyContainerPut(player, objNetId, netId);
		} else if (isServer) {
			PutChild(obj);
			handler(true);
		} else if (isClient) {
			Transform oldParent = obj.gameObject.transform.parent;
			PutChild(obj);
			NetworkRequest.Result internalHandler = delegate (bool success)
				{
					if(!success)
					{
						Debug.Log("BoxContainer.Get failure handler.");
						obj.gameObject.GetComponent<SpriteRenderer>().enabled = true;
						obj.UpdateParent(oldParent, true);
					}
					handler(success);
				};
			NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;
			NetworkInstanceId objNetId = obj.GetComponent<NetworkIdentity>().netId;
			NetworkRequestService.Instance().RequestContainerPut(player, netId, objNetId, internalHandler);
			throw new System.NotImplementedException();
		} else {
			Debug.LogError("BoxContainer Put(...) called with invalid state.");
			throw new System.Exception();
		}
	}

	void Update() {
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		if (goodFor == GoodFor.Player1) {
			// blue
			renderer.color = new Color(141f/255, 226f/255, 224f/255, 160f/255);
		} else if (goodFor == GoodFor.Player2) {
			// yellow
			renderer.color = new Color(240f/255, 236f/255, 120f/255, 160f/255);
		} else if (goodFor == GoodFor.Both) {
			// green
			renderer.color = new Color(0f/255, 203f/255, 20f/255, 160f/255);
		} else if (goodFor == GoodFor.None) {
			// white
			renderer.color = new Color(1f, 1f, 1f, 160f/255);
		}
	}
}
