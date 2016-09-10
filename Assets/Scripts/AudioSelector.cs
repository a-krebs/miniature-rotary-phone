using UnityEngine;
using System.Collections;

public class AudioSelector : MonoBehaviour {

	public AudioClip trackOne;
	public AudioClip trackTwo;

	private PlayerNumber playerNum;
	private AudioSource source;

	void Start () {
		source = GetComponent<AudioSource>();
		playerNum = PlayerNumber.GetLocalPlayerNumber();
	}
	
	// Update is called once per frame
	void Update () {
		if( playerNum == null ) {
			playerNum = PlayerNumber.GetLocalPlayerNumber();
			return;
		}
		if( source.isPlaying ) {
			return;
		} else {
			if( playerNum.IsPlayerOne() ) {
				source.clip = trackOne;
			} else {
				source.clip = trackTwo;
			}
			source.Play();
		}
	
	}
}
