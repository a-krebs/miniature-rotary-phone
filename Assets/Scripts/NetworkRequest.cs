using UnityEngine.Networking;

namespace NetworkRequest
{
	/// Type for callbacks used by NetworkRequestService methods.
	///
	/// To be called with 'true' if the request succeeds, and 'false'
	/// if the request fails.
	public delegate void Result(bool success);

	/// Container to track asynchronous network requests.
	public class Request
	{
		public uint id;
		public event Result OnResult;

		public void Succeeded()
		{
			OnResult(true);
		}

		public void Failed()
		{
			OnResult(false);
		}
	}

	////////////////////////////////////////////////////////////////////////
	/// The following are requests with Succeeded and Failed responses.
	///
	/// NetworkRequestService allows requesting these actions from the client.
	////////////////////////////////////////////////////////////////////////

	public class RequestObjectPickUpMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 1;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
	}

	public class ObjectPickUpSucceededMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 2;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
	}

	public class ObjectPickUpFailedMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 3;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
	}

	public class RequestObjectPutDownMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 4;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
		public uint containerNetId;
	}

	public class ObjectPutDownSucceededMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 5;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
		public uint containerNetId;
	}

	public class ObjectPutDownFailedMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 6;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
		public uint containerNetId;
	}

	public class RequestContainerGetMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 7;
		public uint requestId;
		public uint playerNetId;
		public uint containerNetId;
	}

	public class ContainerGetSucceededMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 8;
		public uint requestId;
		public uint playerNetId;
		public uint containerNetId;
		public uint objNetId;
	}

	public class ContainerGetFailedMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 9;
		public uint requestId;
		public uint playerNetId;
		public uint containerNetId;
	}

	public class RequestContainerPutMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 10;
		public uint requestId;
		public uint playerNetId;
		public uint containerNetId;
		public uint objNetId;
	}

	public class ContainerPutSucceededMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 11;
		public uint requestId;
		public uint playerNetId;
		public uint containerNetId;
		public uint objNetId;
	}

	public class ContainerPutFailedMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 12;
		public uint requestId;
		public uint playerNetId;
		public uint containerNetId;
		public uint objNetId;
	}

	////////////////////////////////////////////////////////////////////////
	/// The following are action notifications.
	///
	/// NetworkRequestService handles these on the clients and updates object
	/// states as required.
	////////////////////////////////////////////////////////////////////////

	public class ObjectPickUpHappenedMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 13;
		public uint playerNetId;
		public uint objNetId;
	}

	public class ObjectPutDownHappenedMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 14;
		public uint playerNetId;
		public uint objNetId;
		public uint containerNetId;
	}

	public class ContainerGetHappenedMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 15;
		public uint playerNetId;
		public uint objNetId;
		public uint containerNetId;
	}

	public class ContainerPutHappenedMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 16;
		public uint playerNetId;
		public uint objNetId;
		public uint containerNetId;
	}
}
