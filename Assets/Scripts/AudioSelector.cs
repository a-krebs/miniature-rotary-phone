using UnityEngine;
using System.Collections;

public class AudioSelector : MonoBehaviour {

	public AudioClip trackOne;
	public AudioClip trackTwo;

	private PlayerNumber playerNum;
	private AudioSource source;

	void Start () {
		source = GetComponent<AudioSource>();
		try
		{
			playerNum = PlayerNumber.GetLocalPlayerNumber();
		} catch {
			playerNum = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if( playerNum == null ) {
			try
			{
				playerNum = PlayerNumber.GetLocalPlayerNumber();
			} catch {
				playerNum = null;
			}
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
