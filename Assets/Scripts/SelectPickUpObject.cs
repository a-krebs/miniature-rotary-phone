using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class SelectPickUpObject : MonoBehaviour {

	List<GameObject> availableObjects;
	GameObject selected;
	GameObject cursor;

	void Awake() {
		availableObjects = new List<GameObject>();
		UpdateSelected (null);
		cursor = GameObject.FindWithTag ("Cursor");
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Object" && !availableObjects.Contains(other.gameObject)) {
			availableObjects.Add(other.gameObject);
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		if (availableObjects.Contains(other.gameObject)) {
			availableObjects.Remove(other.gameObject);
		}
	}	

	void Update() {
		if (availableObjects.Count == 0) {
			UpdateSelected (null);
			HideCursor();
			return;
		}

		if (selected == null || !availableObjects.Contains(selected)) {
			HideCursor();
			if (!UpdateSelected (null))
			{
				return;
			}
		}

		if (selected == null) {
			if (!UpdateSelected (availableObjects[0]))
			{
				return;
			}
		}

		if (Input.GetKeyDown (KeyCode.E)) {
			int next = availableObjects.IndexOf (selected) + 1;
			if( next == availableObjects.Count )
			{
				next = 0;
			}
			if (!UpdateSelected (availableObjects[next]))
			{
				return;
			}
			ShowCursor (selected.transform.position);
		}
	}

	private void HideCursor() {
		cursor.GetComponent<Cursor>().Hide();
	}

	private void ShowCursor( Vector2 position ) {
		cursor.transform.position = position;
		cursor.GetComponent<Cursor>().Show();
	}

	private bool UpdateSelected (GameObject newSelected ) {
		if (selected != null)
		{
			PickUpObject puo = selected.GetComponent<PickUpObject>();
			if (puo.beingCarried)
			{
				return false;
			}
			puo.selected = false;
		}
		selected = newSelected;
		if (selected != null)
		{
			selected.GetComponent<PickUpObject>().selected = true;
		}
		return true;
	}
}
