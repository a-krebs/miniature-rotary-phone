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
		selected = null;
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
		//availableObjects.Sort();

		if (availableObjects.Count == 0) {
			selected = null;
			HideCursor();
			return;
		}

		if (selected == null || !availableObjects.Contains(selected)) {
			selected = null;
			HideCursor();
		}

		if (Input.GetKeyDown (KeyCode.E)) {
			if (selected == null) {
				selected = availableObjects[0];
			} else {
				int next = availableObjects.IndexOf (selected) + 1;
				if( next == availableObjects.Count )
				{
					next = 0;
				}
				selected = availableObjects[next];
			}
		}

		if (selected != null) {
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
}
