using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/// An instance of PlayerNumberManager must be available
/// on the server.
public class PlayerNumberManager : MonoBehaviour {

	// maps from network ID to player number;
	private Dictionary<NetworkInstanceId, int> player_numbers;
	private Dictionary<NetworkInstanceId, NetworkConnection> player_connections;

	void Awake () {
		player_numbers     = new Dictionary<NetworkInstanceId, int> ();
		player_connections = new Dictionary<NetworkInstanceId, NetworkConnection> ();
		Debug.Log ("PlayerNumberManager initialized.");
	}

	/// Get or assign a player number.
	/// Make sure you always provide the NetworkInstanceId of the same Object!
	public int GetPlayerNumber (NetworkInstanceId player, NetworkConnection connection) {
		if (player_numbers.ContainsKey(player))
		{
			return player_numbers[player];
		}
		player_numbers.Add (player, player_numbers.Count + 1);
		player_connections.Add (player, connection);
		return player_numbers[player];
	}

	/// Get a connection for a player, but do not assign a new number.
	/// Make sure you always provide the NetworkInstanceId of the same Object!
	public NetworkConnection GetPlayerConnection (NetworkInstanceId player)
	{
		return player_connections[player];
	}

	/// Get a player number, but do not assign a new number.
	/// Make sure you always provide the NetworkInstanceId of the same Object!
	public int GetPlayerNumber(NetworkInstanceId player)
	{
		return player_numbers[player];
	}

	public static PlayerNumberManager GetServerPlayerNumberManager()
	{
		GameObject obj = GameObject.FindWithTag("PlayerNumberManager");
		if( obj == null ) {
			Debug.Log ("Failed to get PlayerNumberManager.");
			throw new System.MemberAccessException();
		}
		PlayerNumberManager manager = obj.GetComponent<PlayerNumberManager>();
		return manager;
	}
}
