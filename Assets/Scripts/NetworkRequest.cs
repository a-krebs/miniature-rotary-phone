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

	// TODO combine Succeeded and Failed messages into one type, with `bool success` member?
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
}
