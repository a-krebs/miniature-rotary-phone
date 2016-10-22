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
	private Dictionary<NetworkConnection, NetworkInstanceId> connection_players;
	private List<bool> used_numbers;

	void Awake () {
		player_numbers     = new Dictionary<NetworkInstanceId, int> ();
		player_connections = new Dictionary<NetworkInstanceId, NetworkConnection> ();
		connection_players = new Dictionary<NetworkConnection, NetworkInstanceId> ();
		used_numbers       = new List<bool> ();
		used_numbers.Add(true); // always consider number 0 'used'
		Debug.Log ("PlayerNumberManager initialized.");
	}

	/// Get or assign a player number.
	/// Make sure you always provide the NetworkInstanceId of the same Object!
	public int GetPlayerNumber (NetworkInstanceId player, NetworkConnection connection) {
		if (player_numbers.ContainsKey(player))
		{
			return player_numbers[player];
		}
		int number = GetLowestUnusedPlayerNumber();
		player_numbers.Add (player, number);
		player_connections.Add (player, connection);
		connection_players.Add (connection, player);
		return number;
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

	/// Call this when a player disconnects to release their player number
	public void ReleasePlayerNumber(NetworkConnection connection)
	{
		NetworkInstanceId player = connection_players[connection];
		int number = GetPlayerNumber(player);
		Debug.Log ("PlayerNumberManager ReleasePlayerNumber " + number);
		used_numbers[number] = false;
		player_numbers.Remove(player);
		player_connections.Remove(player);
		connection_players.Remove(connection);
	}

	private int GetLowestUnusedPlayerNumber()
	{
		int i = 0;
		for( ; i < used_numbers.Count; ++i )
		{
			if( !used_numbers[i] )
			{
				used_numbers[i] = true;
				Debug.Log ("PlayerNumberManager GetLowestUnusedPlayerNumber " + i);
				return i;
			}
		}
		used_numbers.Add(true);
		Debug.Log ("PlayerNumberManager GetLowestUnusedPlayerNumber " + (used_numbers.Count - 1));
		return used_numbers.Count - 1;
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
