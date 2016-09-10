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
		score += 50;
	}

	private void PickUpObjectPickedUp( GameObject obj, GameObject slot)
	{
		if (slot == null)
		{
			return;
		}
		Debug.Log ("Object picked up!");
		score -= 50;
	}
}
