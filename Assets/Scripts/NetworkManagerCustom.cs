using UnityEngine;
using UnityEngine.Networking;


/// Derives from NetworkManager to get events
public class NetworkManagerCustom : NetworkManager {

	public override void OnServerDisconnect(NetworkConnection conn) {
		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		manager.ReleasePlayerNumber(conn);
		base.OnServerDisconnect(conn);
	}
}
