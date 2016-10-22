using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// Derives from NetworkManager to get events
public class NetworkManagerCustom : NetworkManager {

	public override void OnServerDisconnect(NetworkConnection conn) {
		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		manager.ReleasePlayerNumber(conn);
		base.OnServerDisconnect(conn);
	}

	public override void OnClientError(NetworkConnection conn, int errorCode) {
		Debug.Log ("NetworkManagerCustom.OnClientError");
		base.OnClientError(conn, errorCode);
		HandleConnectionError();
	}

	public override void OnClientDisconnect(NetworkConnection conn) {
		Debug.Log ("NetworkManagerCustom.OnClientDisconnect");
		base.OnClientDisconnect(conn);
		HandleConnectionError();
	}
	
	private void HandleConnectionError() {
		NetworkManager.Shutdown ();
		SceneManager.LoadScene(0);
	}
}
