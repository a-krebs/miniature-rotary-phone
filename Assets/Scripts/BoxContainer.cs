using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// An IContainer that can hold an unlimited number of PickUpObjects.
public class BoxContainer : NetworkBehaviour, IContainer
{
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
			return int.MaxValue;
		}
	}

	public PickUpObject Get(Transform parent)
	{
		if (Count < 1)
		{
			Debug.Log("BoxContainer empty.");
			return null;
		}
		GameObject child = transform.GetChild(0).gameObject;
		child.transform.position = parent.position;
		child.transform.parent = parent;
		child.SetActive(true);
		PickUpObject puo = child.GetComponent<PickUpObject>();
		puo.beingCarried = true;
		return puo;
	}

	public PickUpObject Get(Transform parent, NetworkRequest.Result handler)
	{
		NetworkInstanceId player = PlayerNumber.GetLocalPlayerGameObject().GetComponent<NetworkIdentity>().netId;

		PickUpObject child = transform.GetChild(0).gameObject.GetComponent<PickUpObject>();
		NetworkRequestService.Instance().RequestGet(player, netId, handler);
		return child;
	}

	public void Put(PickUpObject obj)
	{
		if (Count >= Capacity)
		{
			Debug.Log("Slot not empty.");
			throw new System.MemberAccessException();
		}
		obj.transform.position = transform.position;
		obj.transform.parent = transform;
	}

	public void Put(PickUpObject obj, NetworkRequest.Result handler)
	{
		throw new System.NotImplementedException();
	}

	void Update() {
		for(int i = 0; i < transform.childCount; ++i) {
			GameObject obj = transform.GetChild(i).gameObject;
			// special case for Cursor
			if (obj.tag != "Cursor")
			{
				obj.SetActive(false);
			}
		}
	}
}
