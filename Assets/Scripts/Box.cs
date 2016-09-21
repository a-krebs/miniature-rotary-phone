using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// An IContainer that can hold an unlimited number of PickUpObjects.
public class Box : NetworkBehaviour, IContainer
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
		return null;
	}

	public void Put(PickUpObject obj)
	{
		throw new System.NotImplementedException();
	}
}
