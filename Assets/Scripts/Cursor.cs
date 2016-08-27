using UnityEngine;

public class Cursor : MonoBehaviour {

	void Awake () {
		Hide();
	}

	public void Show () {
		GetComponent<Renderer>().enabled = true;
	}

	public void Hide () {
		GetComponent<Renderer>().enabled = false;
	}
}
