using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerNumberManager : MonoBehaviour {

	// maps from network ID to player number;
	private Dictionary<NetworkInstanceId, int> player_numbers;

	void Start() {
		player_numbers = new Dictionary<NetworkInstanceId, int>();
	}

	public int GetPlayerNumber( NetworkInstanceId player )
	{
		if( player_numbers.ContainsKey( player ) )
		{
			return player_numbers[player];
		}
		player_numbers.Add( player, player_numbers.Count + 1 );
		return player_numbers[player];
	}
}
