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
		NetworkServer.RegisterHandler(RequestObjectPickUpMsg.Type, OnRequestObjectPickUp);
		NetworkServer.RegisterHandler(RequestObjectPutDownMsg.Type, OnRequestObjectPutDown);
		NetworkServer.RegisterHandler(RequestContainerGetMsg.Type, OnRequestContainerGet);
		NetworkServer.RegisterHandler(RequestContainerPutMsg.Type, OnRequestContainerPut);
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
		m_client.RegisterHandler(ObjectPickUpSucceededMsg.Type, OnObjectPickUpSucceeded);
		m_client.RegisterHandler(ObjectPickUpFailedMsg.Type, OnObjectPickUpFailed);
		m_client.RegisterHandler(ObjectPutDownSucceededMsg.Type, OnObjectPutDownSucceeded);
		m_client.RegisterHandler(ObjectPutDownFailedMsg.Type, OnObjectPutDownFailed);
		m_client.RegisterHandler(ContainerGetSucceededMsg.Type, OnContainerGetSucceeded);
		m_client.RegisterHandler(ContainerGetFailedMsg.Type, OnContainerGetFailed);
		m_client.RegisterHandler(ContainerPutSucceededMsg.Type, OnContainerPutSucceeded);
		m_client.RegisterHandler(ContainerPutFailedMsg.Type, OnContainerPutFailed);
	}

	/// Request that 'player' picks up 'obj'.
	///
	/// Param 'handler' will be called with argument 'true' if requests succeeds,
	/// 'false' if request fails.
	[Client]
	public void RequestObjectPickUp(NetworkInstanceId player, NetworkInstanceId obj, NetworkRequest.Result handler)
	{
		uint requestId = GetNextRequestId();
		if (m_requests.ContainsKey(requestId))
		{
			throw new System.Exception();
		}
		Debug.Log("Requesting PickUp for player with netId " + player.Value + " for object with netId " + obj.Value);
		RequestObjectPickUpMsg msg = new RequestObjectPickUpMsg();
		msg.requestId = requestId;
		msg.playerNetId = player.Value;
		msg.objNetId = obj.Value;
		Request request = new Request();
		request.id = requestId;
		request.OnResult += handler;
		m_requests.Add(requestId, request);
		m_client.Send(RequestObjectPickUpMsg.Type, msg);
	}

	/// Request that 'player' puts down 'obj' into 'container'.
	/// Param 'container' can be 0 if object is to be placed on the ground.
	///
	/// Param 'handler' will be called with argument 'true' if requests succeeds,
	/// 'false' if request fails.
	[Client]
	public void RequestObjectPutDown(NetworkInstanceId player, NetworkInstanceId obj, NetworkInstanceId container, NetworkRequest.Result handler)
	{
		uint requestId = GetNextRequestId();
		if (m_requests.ContainsKey(requestId))
		{
			throw new System.Exception();
		}
		Debug.Log("Requesting PutDown for player with netId " + player.Value + " for object with netId " + obj.Value + " into container with netId " + container.Value);
		RequestObjectPutDownMsg msg = new RequestObjectPutDownMsg();
		msg.requestId = requestId;
		msg.playerNetId = player.Value;
		msg.objNetId = obj.Value;
		msg.containerNetId = container.Value;
		Request request = new Request();
		request.id = requestId;
		request.OnResult += handler;
		m_requests.Add(requestId, request);
		m_client.Send(RequestObjectPutDownMsg.Type, msg);
	}

	/// Request that 'player' gets an object from 'container'
	///
	/// Param 'handler' will be called with argument 'true' if requests succeeds,
	/// 'false' if request fails.
	[Client]
	public void RequestContainerGet(NetworkInstanceId player, NetworkInstanceId container, NetworkRequest.Result handler)
	{
		uint requestId = GetNextRequestId();
		if (m_requests.ContainsKey(requestId))
		{
			throw new System.Exception();
		}
		Debug.Log("Requesting Get for player with netId " + player.Value + " for container with netId " + container.Value);
		RequestContainerGetMsg msg = new RequestContainerGetMsg();
		msg.requestId = requestId;
		msg.playerNetId = player.Value;
		msg.containerNetId = container.Value;
		Request request = new Request();
		request.id = requestId;
		request.OnResult += handler;
		m_requests.Add(requestId, request);
		m_client.Send(RequestContainerGetMsg.Type, msg);
	}

	/// Request that 'player' places 'obj' into 'container'.
	///
	/// Param 'handler' will be called with argument 'true' if requests succeeds,
	/// 'false' if request fails.
	[Client]
	public void RequestContainerPut(NetworkInstanceId player, NetworkInstanceId container, NetworkInstanceId obj, NetworkRequest.Result handler)
	{
		uint requestId = GetNextRequestId();
		if (m_requests.ContainsKey(requestId))
		{
			throw new System.Exception();
		}
		Debug.Log("Requesting Get for player with netId " + player.Value + " for container with netId " + container.Value);
		RequestContainerPutMsg msg = new RequestContainerPutMsg();
		msg.requestId = requestId;
		msg.playerNetId = player.Value;
		msg.containerNetId = container.Value;
		msg.objNetId = obj.Value;
		Request request = new Request();
		request.id = requestId;
		request.OnResult += handler;
		m_requests.Add(requestId, request);
		m_client.Send(RequestContainerPutMsg.Type, msg);
	}

	//// Server-side request handler.
	[Server]
	void OnRequestObjectPickUp(NetworkMessage msg)
	{
		RequestObjectPickUpMsg request = msg.ReadMessage<RequestObjectPickUpMsg>();
		NetworkInstanceId player = new NetworkInstanceId(request.playerNetId);
		NetworkInstanceId obj    = new NetworkInstanceId(request.objNetId);

		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		NetworkConnection connection = manager.GetPlayerConnection(player);

		Debug.Log("Received RequestObjectPickUp message from player with netId " + player.Value + " for object with netId " + obj.Value);

		try {
			GameObject playerInstance = NetworkServer.FindLocalObject(player);
			GameObject objInstance = NetworkServer.FindLocalObject(obj);

			PickUpObject puo = objInstance.GetComponent<PickUpObject>();

			bool allow = true;
			allow &= playerInstance != null;
			allow &= objInstance != null;
			allow &= (puo != null && !puo.beingCarried);

			if (allow)
			{
				puo.PickUp(playerInstance.transform, GetNoOpHandler());

				ObjectPickUpSucceededMsg response = new ObjectPickUpSucceededMsg();
				response.requestId = request.requestId;
				response.playerNetId = request.playerNetId;
				response.objNetId = request.objNetId;
				NetworkServer.SendToClient(connection.connectionId, ObjectPickUpSucceededMsg.Type, response);
				Debug.Log("PickUp Succeeded: player with netId " + player.Value + "picked up object with netId " + obj.Value);
			} else {
				SendObjectPickUpFailedMsg(request, connection);
				Debug.Log("PickUp Failed: player with netId " + player.Value + "did not pickup object with netId " + obj.Value);
			}
		} catch {
			SendObjectPickUpFailedMsg(request, connection);
			Debug.Log("PickUp Failed: player with netId " + player.Value + "did not pickup object with netId " + obj.Value);
		}
	}

	[Server]
	private void SendObjectPickUpFailedMsg(RequestObjectPickUpMsg request, NetworkConnection connection)
	{
		ObjectPickUpFailedMsg response = new ObjectPickUpFailedMsg();
		response.requestId = request.requestId;
		response.playerNetId = request.playerNetId;
		response.objNetId = request.objNetId;
		NetworkServer.SendToClient(connection.connectionId, ObjectPickUpFailedMsg.Type, response);
	}

	//// Server-side request handler.
	[Server]
	void OnRequestObjectPutDown(NetworkMessage msg)
	{
		RequestObjectPutDownMsg request   = msg.ReadMessage<RequestObjectPutDownMsg>();
		NetworkInstanceId player    = new NetworkInstanceId(request.playerNetId);
		NetworkInstanceId obj       = new NetworkInstanceId(request.objNetId);
		NetworkInstanceId container = new NetworkInstanceId(request.containerNetId);

		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		NetworkConnection connection = manager.GetPlayerConnection(player);

		Debug.Log("Received RequestObjectPutDown message from player with netId " + player.Value + " for object with netId " + obj.Value + " and container netId: " + container.Value);

		try {
			GameObject playerInstance = NetworkServer.FindLocalObject(player);
			GameObject objInstance = NetworkServer.FindLocalObject(obj);
			GameObject containerInstance = null; // fetched below

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
				containerInstance = NetworkServer.FindLocalObject(container);
			}

			if (allow)
			{
				puo.PutDown(containerInstance, GetNoOpHandler());

				ObjectPutDownSucceededMsg response = new ObjectPutDownSucceededMsg();
				response.requestId = request.requestId;
				response.playerNetId = request.playerNetId;
				response.objNetId = request.objNetId;
				response.containerNetId = request.containerNetId;
				NetworkServer.SendToClient(connection.connectionId, ObjectPutDownSucceededMsg.Type, response);
				Debug.Log("PutDown Succeeded: player with netId " + player.Value + "put down object with netId " + obj.Value + (container.Value == 0 ? "" : " into container with netId " + container.Value));
			} else {
				SendObjectPutDownFailedMsg(request, connection);
				Debug.Log("PutDown Failed: player with netId " + player.Value + " did not put down object with netId " + obj.Value + " and container netId " + container.Value);
			}
		} catch {
			SendObjectPutDownFailedMsg(request, connection);
			Debug.Log("PutDown Failed: player with netId " + player.Value + " did not put down object with netId " + obj.Value + " and container netId " + container.Value);
		}
	}

	[Server]
	private void SendObjectPutDownFailedMsg(RequestObjectPutDownMsg request, NetworkConnection connection)
	{
		ObjectPutDownFailedMsg response = new ObjectPutDownFailedMsg();
		response.requestId = request.requestId;
		response.playerNetId = request.playerNetId;
		response.objNetId = request.objNetId;
		response.containerNetId = request.containerNetId;
		NetworkServer.SendToClient(connection.connectionId, ObjectPutDownFailedMsg.Type, response);
	}

	//// Server-side request handler.
	[Server]
	void OnRequestContainerGet(NetworkMessage msg)
	{
		RequestContainerGetMsg request       = msg.ReadMessage<RequestContainerGetMsg>();
		NetworkInstanceId player    = new NetworkInstanceId(request.playerNetId);
		NetworkInstanceId container = new NetworkInstanceId(request.containerNetId);

		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		NetworkConnection connection = manager.GetPlayerConnection(player);

		Debug.Log("Received RequestContainerGet message from player with netId " + player.Value + " for container with netId: " + container.Value);

		GameObject playerInstance = NetworkServer.FindLocalObject(player);
		GameObject containerGameObj = NetworkServer.FindLocalObject(container);
		IContainer containerInstance = null; // fetched below

		if (containerGameObj.tag == "BoxContainer")
		{
			BoxContainer box = containerGameObj.GetComponent<BoxContainer>();
			containerInstance = box;
		} else if (containerGameObj.tag == "ObjectSlot") {
			Slot slot = containerGameObj.GetComponent<Slot>();
			containerInstance = slot;
		}

		PickUpObject puo = containerInstance.Get(playerInstance.transform, GetNoOpHandler());

		if (puo != null)
		{
			NetworkInstanceId obj = puo.GetComponent<NetworkIdentity>().netId;
			ContainerGetSucceededMsg response = new ContainerGetSucceededMsg();
			response.requestId = request.requestId;
			response.playerNetId = request.playerNetId;
			response.containerNetId = request.containerNetId;
			response.objNetId = obj.Value;
			NetworkServer.SendToClient(connection.connectionId, ContainerGetSucceededMsg.Type, response);
			Debug.Log("Get Succeeded: player with netId " + player.Value + " got object with netId " + obj.Value + " from container with netId " + container.Value);
		} else {
			ContainerGetFailedMsg response = new ContainerGetFailedMsg();
			response.requestId = request.requestId;
			response.playerNetId = request.playerNetId;
			response.containerNetId = request.containerNetId;
			NetworkServer.SendToClient(connection.connectionId, ContainerGetFailedMsg.Type, response);
			Debug.Log("Get Failed: player with netId " + player.Value + " did not get anything from container with netId " + container.Value);
		}
	}

	//// Server-side request handler.
	[Server]
	void OnRequestContainerPut(NetworkMessage msg)
	{
		RequestContainerPutMsg request       = msg.ReadMessage<RequestContainerPutMsg>();
		NetworkInstanceId player    = new NetworkInstanceId(request.playerNetId);
		NetworkInstanceId container = new NetworkInstanceId(request.containerNetId);
		NetworkInstanceId obj       = new NetworkInstanceId(request.objNetId);

		PlayerNumberManager manager = PlayerNumberManager.GetServerPlayerNumberManager();
		NetworkConnection connection = manager.GetPlayerConnection(player);

		Debug.Log("Received RequestContainerPut message from player with netId " + player.Value + " for object with netId " + obj.Value + " and container netId: " + container.Value);

		try {
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
				Debug.Log("Put Failed: object not being carried?");
				allow = false;
			}

			if (allow && container.Value != 0)
			{
				GameObject containerGameObj = NetworkServer.FindLocalObject(container);
				containerInstance = PickUpObject.GetIContainer(containerGameObj);

				if (containerInstance == null)
				{
					Debug.Log("Put Failed: couldn't get IContainer.");
					allow = false;
				} else if(containerInstance.Count >= containerInstance.Capacity) {
					Debug.Log("Put Failed: container capacity exceeded.");
					allow = false;
				}
			}

			if (allow)
			{
				containerInstance.Put(puo, GetNoOpHandler());

				ContainerPutSucceededMsg response = new ContainerPutSucceededMsg();
				response.requestId = request.requestId;
				response.playerNetId = request.playerNetId;
				response.objNetId = request.objNetId;
				response.containerNetId = request.containerNetId;
				NetworkServer.SendToClient(connection.connectionId, ContainerPutSucceededMsg.Type, response);
				Debug.Log("Put Succeeded: player with netId " + player.Value + "put down object with netId " + obj.Value + (container.Value == 0 ? "" : " into container with netId " + container.Value));
			} else {
				SendContainerPutFailedMsg(request, connection);
				Debug.Log("Put Failed: player with netId " + player.Value + " did not put down object with netId " + obj.Value + " and container netId " + container.Value);
			}
		} catch {
			SendContainerPutFailedMsg(request, connection);
			Debug.Log("Put Failed: player with netId " + player.Value + " did not put down object with netId " + obj.Value + " and container netId " + container.Value);
		}
	}

	[Server]
	private void SendContainerPutFailedMsg(RequestContainerPutMsg request, NetworkConnection connection)
	{
		ContainerPutFailedMsg response = new ContainerPutFailedMsg();
		response.requestId = request.requestId;
		response.playerNetId = request.playerNetId;
		response.objNetId = request.objNetId;
		response.containerNetId = request.containerNetId;
		NetworkServer.SendToClient(connection.connectionId, ContainerPutFailedMsg.Type, response);
	}

	/// Client-side response handler. Calls the handler passed in with the request.
	[Client]
	void OnObjectPickUpSucceeded(NetworkMessage msg)
	{
		ObjectPickUpSucceededMsg response = msg.ReadMessage<ObjectPickUpSucceededMsg>();
		Debug.Log("Received ObjectPickUpSucceededMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Succeeded();
		m_requests.Remove(requestId);
	}

	/// Client-side response handler. Calls the handler passed in with the request.
	[Client]
	void OnObjectPickUpFailed(NetworkMessage msg)
	{
		ObjectPickUpFailedMsg response = msg.ReadMessage<ObjectPickUpFailedMsg>();
		Debug.Log("Received ObjectPickUpFailedMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Failed();
		m_requests.Remove(requestId);
	}

	/// Client-side response handler. Calls the handler passed in with the request.
	[Client]
	void OnObjectPutDownSucceeded(NetworkMessage msg)
	{
		ObjectPutDownSucceededMsg response = msg.ReadMessage<ObjectPutDownSucceededMsg>();
		Debug.Log("Received ObjectPutDownSucceededMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Succeeded();
		m_requests.Remove(requestId);
	}

	/// Client-side response handler. Calls the handler passed in with the request.
	[Client]
	void OnObjectPutDownFailed(NetworkMessage msg)
	{
		ObjectPutDownFailedMsg response = msg.ReadMessage<ObjectPutDownFailedMsg>();
		Debug.Log("Received ObjectPutDownFailedMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Failed();
		m_requests.Remove(requestId);
	}

	/// Client-side response handler. Calls the handler passed in with the request.
	[Client]
	void OnContainerGetSucceeded(NetworkMessage msg)
	{
		ContainerGetSucceededMsg response = msg.ReadMessage<ContainerGetSucceededMsg>();
		Debug.Log("Received ContainerGetSucceededMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Succeeded();
		m_requests.Remove(requestId);
	}

	/// Client-side response handler. Calls the handler passed in with the request.
	[Client]
	void OnContainerGetFailed(NetworkMessage msg)
	{
		ContainerGetFailedMsg response = msg.ReadMessage<ContainerGetFailedMsg>();
		Debug.Log("Received ContainerGetFailedMsg for player with netId " + response.playerNetId + " for container with netId " + response.containerNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Failed();
		m_requests.Remove(requestId);
	}

	/// Client-side response handler. Calls the handler passed in with the request.
	[Client]
	void OnContainerPutSucceeded(NetworkMessage msg)
	{
		ContainerPutSucceededMsg response = msg.ReadMessage<ContainerPutSucceededMsg>();
		Debug.Log("Received ContainerPutSucceededMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
		uint requestId = response.requestId;
		Request request = m_requests[requestId];
		request.Succeeded();
		m_requests.Remove(requestId);
	}

	/// Client-side response handler. Calls the handler passed in with the request.
	[Client]
	void OnContainerPutFailed(NetworkMessage msg)
	{
		ContainerPutFailedMsg response = msg.ReadMessage<ContainerPutFailedMsg>();
		Debug.Log("Received ContainerPutFailedMsg for player with netId " + response.playerNetId + " for object with netId " + response.objNetId);
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

	[Server]
	private NetworkRequest.Result GetNoOpHandler()
	{
		return delegate (bool success){};
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
