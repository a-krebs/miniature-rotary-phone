using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using NetworkRequest;

public class NetworkRequestService : NetworkBehaviour
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
		Debug.Log("NetworkRequestService OnStartServer");
		NetworkServer.RegisterHandler(RequestPickUpMsg.Type, OnRequestPickUp);
		NetworkServer.RegisterHandler(RequestPutDownMsg.Type, OnRequestPutDown);
	}

	public override void OnStartClient()
	{
		Debug.Log("NetworkRequestService OnStartClient");
		Debug.Log("NetworkClient.allClients.Count: " + NetworkClient.allClients.Count);
		// client count must be 1, we're relying on it being the only connection to the server
		if(NetworkClient.allClients.Count != 1)
		{
			throw new System.Exception();
		}
		m_client = NetworkClient.allClients[0];
		m_client.RegisterHandler(PickUpSucceededMsg.Type, OnPickUpSucceeded);
		m_client.RegisterHandler(PickUpFailedMsg.Type, OnPickUpFailed);
		m_client.RegisterHandler(PutDownSucceededMsg.Type, OnPutDownSucceeded);
		m_client.RegisterHandler(PutDownFailedMsg.Type, OnPutDownFailed);
	}

	[Client]
	public void RequestPickUp(NetworkInstanceId player, NetworkInstanceId obj, NetworkRequest.Result handler)
	{
		uint requestId = GetNextRequestId();
		if (m_requests.ContainsKey(requestId))
		{
			throw new System.Exception();
		}
		Debug.Log("Requesting PickUp for player with netId " + player.Value + " for object with netId " + obj.Value);
		RequestPickUpMsg msg = new RequestPickUpMsg();
		msg.requestId = requestId;
		msg.playerNetId = player.Value;
		msg.objNetId = obj.Value;
		Request request = new Request();
		request.id = requestId;
		request.OnResult += handler;
		m_requests.Add(requestId, request);
		m_client.Send(RequestPickUpMsg.Type, msg);
	}

	[Client]
	public void RequestPutDown(NetworkInstanceId player, NetworkInstanceId obj, NetworkInstanceId container, NetworkRequest.Result handler)
	{
		uint requestId = GetNextRequestId();
		if (m_requests.ContainsKey(requestId))
		{
			throw new System.Exception();
		}
		Debug.Log("Requesting PutDown for player with netId " + player.Value + " for object with netId " + obj.Value + " into container with netId " + container.Value);
		RequestPutDownMsg msg = new RequestPutDownMsg();
		msg.requestId = requestId;
		msg.playerNetId = player.Value;
		msg.objNetId = obj.Value;
		msg.containerNetId = container.Value;
		Request request = new Request();
		request.id = requestId;
		request.OnResult += handler;
		m_requests.Add(requestId, request);
		m_client.Send(RequestPutDownMsg.Type, msg);
	}

	[Server]
	void OnRequestPickUp(NetworkMessage msg)
	{
		RequestPickUpMsg request = msg.ReadMessage<RequestPickUpMsg>();
		NetworkInstanceId player = new NetworkInstanceId(request.playerNetId);
		NetworkInstanceId obj    = new NetworkInstanceId(request.objNetId);

		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		NetworkConnection connection = manager.GetPlayerConnection(player);

		Debug.Log("Received RequestPickUp message from player with netId " + player.Value + " for object with netId " + obj.Value);

		GameObject playerInstance = NetworkServer.FindLocalObject(player);
		GameObject objInstance = NetworkServer.FindLocalObject(obj);

		PickUpObject puo = objInstance.GetComponent<PickUpObject>();

		bool allow = true;
		allow &= playerInstance != null;
		allow &= objInstance != null;
		allow &= (puo != null && !puo.beingCarried);

		if (allow)
		{
			puo.beingCarried = true;
			objInstance.transform.position = playerInstance.transform.position;
			objInstance.transform.parent = playerInstance.transform;

			PickUpSucceededMsg response = new PickUpSucceededMsg();
			response.requestId = request.requestId;
			response.playerNetId = request.playerNetId;
			response.objNetId = request.objNetId;
			NetworkServer.SendToClient(connection.connectionId, PickUpSucceededMsg.Type, response);
			Debug.Log("PickUp Succeeded: player with netId " + player.Value + "picked up object with netId " + obj.Value);
		} else {
			PickUpFailedMsg response = new PickUpFailedMsg();
			response.requestId = request.requestId;
			response.playerNetId = request.playerNetId;
			response.objNetId = request.objNetId;
			NetworkServer.SendToClient(connection.connectionId, PickUpFailedMsg.Type, response);
			Debug.Log("PickUp Failed: player with netId " + player.Value + "did not pickup object with netId " + obj.Value);
		}
	}

	[Server]
	void OnRequestPutDown(NetworkMessage msg)
	{
		RequestPutDownMsg request   = msg.ReadMessage<RequestPutDownMsg>();
		NetworkInstanceId player    = new NetworkInstanceId(request.playerNetId);
		NetworkInstanceId obj       = new NetworkInstanceId(request.objNetId);
		NetworkInstanceId container = new NetworkInstanceId(request.containerNetId);

		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		NetworkConnection connection = manager.GetPlayerConnection(player);

		Debug.Log("Received RequestPutDown message from player with netId " + player.Value + " for object with netId " + obj.Value + " and container netId: " + container.Value);

		GameObject playerInstance = NetworkServer.FindLocalObject(player);
		GameObject objInstance = NetworkServer.FindLocalObject(obj);
		IContainer containerInstance = null; // fetched below

		PickUpObject puo = objInstance.GetComponent<PickUpObject>();

		bool allow = true;
		allow &= playerInstance != null;
		allow &= objInstance != null;
		allow &= puo != null;

		if (allow && !puo.beingCarried)
		{
			// TODO make sure the requesting player is carrying the object
			Debug.Log("PutDown Failed: object not being carried?");
			allow = false;
		}

		if (allow && container.Value != 0)
		{
			GameObject containerGameObj = NetworkServer.FindLocalObject(container);

			if (containerGameObj.tag == "BoxContainer")
			{
				BoxContainer box = containerGameObj.GetComponent<BoxContainer>();
				containerInstance = box;
			} else if (containerGameObj.tag == "ObjectSlot") {
				Slot slot = containerGameObj.GetComponent<Slot>();
				containerInstance = slot;
			}

			if (containerInstance == null)
			{
				Debug.Log("PutDown Failed: couldn't get IContainer.");
				allow = false;
			} else if(containerInstance.Count >= containerInstance.Capacity) {
				Debug.Log("PutDown Failed: container capacity exceeded.");
				allow = false;
			}
		}

		if (allow)
		{
			puo.beingCarried = false;
			if (containerInstance != null)
			{
				containerInstance.Put(puo);
			} else {
				// TODO drop on ground
				objInstance.transform.position = playerInstance.transform.position;
				objInstance.transform.parent = null;
			}

			PutDownSucceededMsg response = new PutDownSucceededMsg();
			response.requestId = request.requestId;
			response.playerNetId = request.playerNetId;
			response.objNetId = request.objNetId;
			response.containerNetId = request.containerNetId;
			NetworkServer.SendToClient(connection.connectionId, PutDownSucceededMsg.Type, response);
			Debug.Log("PutDown Succeeded: player with netId " + player.Value + "put down object with netId " + obj.Value + (container.Value == 0 ? "" : " into container with netId " + container.Value));
		} else {
			PutDownFailedMsg response = new PutDownFailedMsg();
			response.requestId = request.requestId;
			response.playerNetId = request.playerNetId;
			response.objNetId = request.objNetId;
			response.containerNetId = request.containerNetId;
			NetworkServer.SendToClient(connection.connectionId, PutDownFailedMsg.Type, response);
			Debug.Log("PutDown Failed: player with netId " + player.Value + " did not pickup object with netId " + obj.Value + " and container netId " + container.Value);
		}
	}

	[Client]
	void OnPickUpSucceeded(NetworkMessage msg)
	{
		PickUpSucceededMsg response = msg.ReadMessage<PickUpSucceededMsg>();
		Debug.Log("Received PickUpSucceededMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Succeeded();
		m_requests.Remove(requestId);
	}

	[Client]
	void OnPickUpFailed(NetworkMessage msg)
	{
		PickUpFailedMsg response = msg.ReadMessage<PickUpFailedMsg>();
		Debug.Log("Received PickUpFailedMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Failed();
		m_requests.Remove(requestId);
	}

	[Client]
	void OnPutDownSucceeded(NetworkMessage msg)
	{
		PutDownSucceededMsg response = msg.ReadMessage<PutDownSucceededMsg>();
		Debug.Log("Received PutDownSucceededMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Succeeded();
		m_requests.Remove(requestId);
	}

	[Client]
	void OnPutDownFailed(NetworkMessage msg)
	{
		PutDownFailedMsg response = msg.ReadMessage<PutDownFailedMsg>();
		Debug.Log("Received PutDownFailedMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Failed();
		m_requests.Remove(requestId);
	}

	[Client]
	private uint GetNextRequestId()
	{
		uint next = m_nextRequestId;
		m_nextRequestId++;
		return next;
	}

	public static NetworkRequestService Instance()
	{
		GameObject obj = GameObject.FindWithTag("NetworkRequestService");
		if( obj == null ) {
			Debug.Log ("Failed to get NetworkRequestService.");
			throw new System.MemberAccessException();
		}
		return obj.GetComponent<NetworkRequestService>();
	}
}
