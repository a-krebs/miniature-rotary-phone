using UnityEngine;

/// Utility functions for working with IContainers
public class IContainerUtils
{

	/// Utility method to get the IContainer from different GameObjects.
	///
	/// Throws NotImplementedException for unknown GameObject tags.
	public static IContainer GetIContainer(GameObject containerGameObj)
	{
		IContainer containerInstance = null;
		if (containerGameObj.tag == "BoxContainer")
		{
			BoxContainer box = containerGameObj.GetComponent<BoxContainer>();
			containerInstance = box;
		} else if (containerGameObj.tag == "ObjectSlot") {
			Slot slot = containerGameObj.GetComponent<Slot>();
			containerInstance = slot;
		} else {
			throw new System.NotImplementedException();
		}

		return containerInstance;
	}

	/// Get child count but ignore objects with tag "Cursor"
	public static int GetChildCount(Transform transform)
	{
		int count = 0;
		for (int i = 0; i < transform.childCount; ++i) {
			if (transform.GetChild(i).gameObject.tag != "Cursor") {
				count++;
			}
		}
		return count;
	}
}
