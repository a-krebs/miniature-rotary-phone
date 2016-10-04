using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Score : NetworkBehaviour {

	[SyncVar]
	public int score;
	float timeLeft = 10.0f;
	public Text text;
	public GameObject panelVis;
	public Slider scoreSlider;
	public NetworkManager manager;

	void Start ()
	{
		score = 0;
		panelVis.SetActive (false);
		manager = GetComponent<NetworkManager> ();

	}

	public override void OnStartServer()
	{
		PickUpObject.OnPlaced += PickUpObjectPlaced;
		PickUpObject.OnPickedUp += PickUpObjectPickedUp;
	}

	void Update()
	{
		scoreSlider.value = score;
		if (score >= 70){
			panelVis.SetActive (true);
			timeLeft -= Time.deltaTime;
			text.text = "Restart in:" + Mathf.Round(timeLeft);
			if(timeLeft < 0)
			{

				NetworkManager.Shutdown ();
				SceneManager.LoadScene(0);
			}
		}
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
			score += 0;
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
			score -= 10;
		} else {
			score -= 0;
		}

		Debug.Log("Score: " + score);
	}
}
