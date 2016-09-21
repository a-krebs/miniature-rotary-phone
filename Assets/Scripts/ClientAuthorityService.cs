using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using NetworkRequest;

public class ClientAuthorityService : NetworkBehaviour
{
	private NetworkClient m_client;
	private uint m_nextRequestId = 0;
	private Dictionary<uint, NetworkRequest.Request> m_requests;


	void Awake()
	{
		m_requests = new Dictionary<uint, NetworkRequest.Request>();
	}

	public override void OnStartServer()
	{
		Debug.Log("ClientAuthorityService OnStartServer");
		NetworkServer.RegisterHandler(RequestAuthMessage.Type, OnRequestAuth);
		NetworkServer.RegisterHandler(ReleaseAuthMessage.Type, OnReleaseAuth);
	}

	public override void OnStartClient()
	{
		Debug.Log("ClientAuthorityService OnStartClient");
		Debug.Log("NetworkClient.allClients.Count: " + NetworkClient.allClients.Count);
		// client count must be 1, we're relying on it being the only connection to the server
		if(NetworkClient.allClients.Count != 1)
		{
			throw new System.Exception();
		}
		m_client = NetworkClient.allClients[0];
		m_client.RegisterHandler(GrantAuthMessage.Type, OnGrantAuth);
		m_client.RegisterHandler(DenyAuthMessage.Type, OnDenyAuth);
	}

	[Client]
	public void RequestAuth(NetworkInstanceId player, NetworkInstanceId obj, NetworkRequest.Result handler)
	{
		uint requestId = GetNextRequestId();
		if (m_requests.ContainsKey(requestId))
		{
			throw new System.Exception();
		}
		Debug.Log("Requesting client authority for player with netId " + player.Value + " for object with netId " + obj.Value);
		RequestAuthMessage msg = new RequestAuthMessage();
		msg.requestId = requestId;
		msg.playerNetId = player.Value;
		msg.objNetId = obj.Value;
		Request request = new Request();
		request.id = requestId;
		request.OnResult += handler;
		m_requests.Add(requestId, request);
		m_client.Send(RequestAuthMessage.Type, msg);
	}

	[Client]
	public void ReleaseAuth(NetworkInstanceId player, NetworkInstanceId obj)
	{
		uint requestId = GetNextRequestId();
		if (m_requests.ContainsKey(requestId))
		{
			throw new System.Exception();
		}
		Debug.Log("Releasing client authority for player with netId " + player.Value + " for object with netId " + obj.Value);
		ReleaseAuthMessage msg = new ReleaseAuthMessage();
		msg.requestId = requestId;
		msg.playerNetId = player.Value;
		msg.objNetId = obj.Value;
		m_client.Send(ReleaseAuthMessage.Type, msg);
	}

	[Server]
	void OnRequestAuth(NetworkMessage msg)
	{
		RequestAuthMessage request = msg.ReadMessage<RequestAuthMessage>();
		NetworkInstanceId player = new NetworkInstanceId(request.playerNetId);
		NetworkInstanceId obj    = new NetworkInstanceId(request.objNetId);

		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		NetworkConnection connection = manager.GetPlayerConnection(player);

		Debug.Log("Received RequestAuth message from player with netId " + player.Value + " for object with netId " + obj.Value);
		GameObject instance = NetworkServer.FindLocalObject(obj);
		if (instance.GetComponent<NetworkIdentity>().hasAuthority)
		{
			instance.GetComponent<NetworkIdentity>().AssignClientAuthority( connection );

			GrantAuthMessage response = new GrantAuthMessage();
			response.requestId = request.requestId;
			response.playerNetId = request.playerNetId;
			response.objNetId = request.objNetId;
			NetworkServer.SendToClient(connection.connectionId, GrantAuthMessage.Type, response);
			Debug.Log("Granted client authority to player with netId " + player.Value + " for object with netId " + obj.Value);
		} else {
			DenyAuthMessage response = new DenyAuthMessage();
			response.requestId = request.requestId;
			response.playerNetId = request.playerNetId;
			response.objNetId = request.objNetId;
			NetworkServer.SendToClient(connection.connectionId, DenyAuthMessage.Type, response);
			Debug.Log("Denied client authority to player with netId " + player.Value + " for object with netId " + obj.Value);
		}
	}

	[Server]
	void OnReleaseAuth(NetworkMessage msg)
	{
		ReleaseAuthMessage request = msg.ReadMessage<ReleaseAuthMessage>();
		NetworkInstanceId player = new NetworkInstanceId(request.playerNetId);
		NetworkInstanceId obj    = new NetworkInstanceId(request.objNetId);

		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		NetworkConnection connection = manager.GetPlayerConnection(player);

		Debug.Log("Received ReleaseAuth message from player with netId " + player.Value + " for object with netId " + obj.Value);
		GameObject instance = NetworkServer.FindLocalObject(obj);
		if(!instance.GetComponent<NetworkIdentity>().RemoveClientAuthority(connection))
		{
			Debug.Log("Failed to remove client authority from player with netId " + player.Value + " for object with netId " + obj.Value);
		}
	}

	[Client]
	void OnGrantAuth(NetworkMessage msg)
	{
		GrantAuthMessage response = msg.ReadMessage<GrantAuthMessage>();
		Debug.Log("Received GrantAuthMessage for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Succeeded();
		m_requests.Remove(requestId);
		Debug.Log("Was granted client authority.");
	}

	[Client]
	void OnDenyAuth(NetworkMessage msg)
	{
		DenyAuthMessage response = msg.ReadMessage<DenyAuthMessage>();
		Debug.Log("Received DenyAuthMessage for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Failed();
		m_requests.Remove(requestId);
		Debug.Log("Was denied client authority.");
	}

	[Client]
	private uint GetNextRequestId()
	{
		uint next = m_nextRequestId;
		m_nextRequestId++;
		return next;
	}

	public static ClientAuthorityService Instance()
	{
		GameObject obj = GameObject.FindWithTag("ClientAuthorityService");
		if( obj == null ) {
			Debug.Log ("Failed to get ClientAuthorityService.");
			throw new System.MemberAccessException();
		}
		return obj.GetComponent<ClientAuthorityService>();
	}
}
