using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class Score : NetworkBehaviour {

	public int score;                           
	public Slider scoreSlider;                                 



	void Awake ()
	{
		score = 0;
	}


	public void ChangeScore (int amount)
	{


		score += amount;


		scoreSlider.value = score;


	}
		     
}