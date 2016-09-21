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

	public PickUpObject Get(Transform parent, NetworkRequest.Result handler)
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
		// TODO null check
		return child.GetComponent<PickUpObject>();
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

	void Update() {
		for(int i = 0; i < transform.childCount; ++i) {
			GameObject obj = transform.GetChild(i).gameObject;
			obj.SetActive(false);
		}
	}
}
