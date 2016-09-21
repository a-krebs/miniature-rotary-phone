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

	public PickUpObject Get(Transform parent, NetworkRequest.Result handler)
	{
		if (Count < 1)
		{
			Debug.Log("Slot empty.");
			return null;
		}
		GameObject child = transform.GetChild(0).gameObject;
		child.transform.position = parent.position;
		child.transform.parent = parent;
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
		if (obj.size != size)
		{
			Debug.Log("Slot and object have different sizes.");
			throw new System.MemberAccessException();
		}
		obj.transform.position = transform.position;
		obj.transform.parent = transform;
	}
}
