using UnityEngine;
using UnityEngine.Networking;

public class Slot : NetworkBehaviour, IContainer {
	public PickUpObject.Size size;
	
	private GameObject obj = null;

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
		return null;
	}

	public void Put(PickUpObject obj)
	{
		throw new System.NotImplementedException();
	}
}
