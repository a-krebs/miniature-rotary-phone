using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkManagerStartup : MonoBehaviour {

    public NetworkManager manager;

    // Use this for initialization
    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }


}
	
