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
	///
	/// The handler is called asynchronously when the call
	/// succeeds or fails. The caller must revert any actions taken
	/// appropriately. The handler is *NOT* guaranteed to be called
	/// after this method returns; it may be called before.
	PickUpObject Get(Transform parent, NetworkRequest.Result handler);

	/// Put the given PickUpObject in the container.
	/// Throw an exception if the container is full.
	///
	/// The handler is called asynchronously when the call
	/// succeeds or fails. The caller must revert any actions taken
	/// appropriately. The handler is *NOT* guaranteed to be called
	/// after this method returns; it may be called before.
	void Put(PickUpObject obj, NetworkRequest.Result handler);
}
