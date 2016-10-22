using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class NetworkManagerStartup : MonoBehaviour {

	private NetworkManager manager;
	private JSONNode config;
	private string role;
	private string address;
	private int port;

	// Use this for initialization
	void Awake()
	{
		Debug.Log("NetworkManagerStartup starting");
		manager = GetComponent<NetworkManager>();
		if(manager == null){
			Debug.Log("Error: no network Manager");
		}
		// this path is relative to the working directory
		string text = System.IO.File.ReadAllText(@"network_config.json");
		config = JSON.Parse(text);

	}

	void Start()
	{
		Debug.Log("NetworkManagerStartup in start function");
		role = config["role"].Value;
		address = config["server-address"].Value;
		port = config["server-port"].AsInt;

		Debug.Log("NetworkManagerStartup starting as " + role);

		manager.networkAddress = address;
		manager.networkPort = port;

		if (role == "client") {
			manager.StartClient();
		} else if (role == "server") {
			manager.StartServer();
		} else if (role == "host") {
			manager.StartHost();
		} else {
			Debug.Log("NetworkManagerStartup config missing 'role'.");
		}
	}

	void Update() {
		Network.TestConnection (true);
		if(Input.GetKeyDown("joystick button 10")){
			NetworkManager.Shutdown ();
			SceneManager.LoadScene(0); 
		}
	}
}
	
