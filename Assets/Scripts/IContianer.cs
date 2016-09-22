using UnityEngine;

/// Container that can hold one or more PickUpObjects.
public interface IContainer
{
	/// Get the number of PickUpObjects currently in the container.
	int Count { get; }

	/// Get the max capacity of the container.
	int Capacity { get; }

	/// Get a PickUpObject from the container.
	/// Return null if container is empty.
	PickUpObject Get(Transform parent);
	PickUpObject Get(Transform parent, NetworkRequest.Result handler);

	/// Put the given PickUpObject in the container.
	/// Throw an exception if the container is full.
	void Put(PickUpObject obj);
	void Put(PickUpObject obj, NetworkRequest.Result handler);
}
