using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class Score : NetworkBehaviour {

	[SyncVar]
	public int score;

	public Slider scoreSlider;                                 

	void Awake ()
	{
		score = 0;
	}

	public override void OnStartServer()
	{
		PickUpObject.OnPlaced += PickUpObjectPlaced;
		PickUpObject.OnPickedUp += PickUpObjectPickedUp;
	}

	void Update()
	{
		scoreSlider.value = score;
	}

	private void PickUpObjectPlaced( GameObject obj, GameObject slot)
	{
		Debug.Log ("Object placed down!");

		if (slot == null) {
			return;
		}

		Slot s = slot.GetComponent<Slot>();

		if (s.goodFor == Slot.GoodFor.Both) {
			score += 20;
		} else {
			score += 10;
		}

		Debug.Log("Score: " + score);
	}

	private void PickUpObjectPickedUp( GameObject obj, GameObject slot)
	{
		Debug.Log ("Object picked up!");

		if (slot == null) {
			return;
		}

		Slot s = slot.GetComponent<Slot>();

		if (s.goodFor == Slot.GoodFor.Both) {
			score -= 20;
		} else {
			score -= 10;
		}

		Debug.Log("Score: " + score);
	}
}
