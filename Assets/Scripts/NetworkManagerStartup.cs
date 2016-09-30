using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManagerStartup : MonoBehaviour {

    public NetworkManager manager;

	// Use this for initialization
	void Start () {
        manager = GetComponent<NetworkManager>();
        if (manager.isNetworkActive){
            manager.StartClient();
        }
        else
        {
            manager.StartHost();
        }
        
	}
	
}
