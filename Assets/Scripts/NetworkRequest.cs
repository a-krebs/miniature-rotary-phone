using UnityEngine.Networking;

namespace NetworkRequest
{
	public delegate void Result(bool success);

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
	public class RequestPickUpMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 1;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
	}

	public class PickUpSucceededMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 2;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
	}

	public class PickUpFailedMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 3;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
	}

	public class RequestPutDownMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 4;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
		public uint containerNetId;
	}

	public class PutDownSucceededMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 5;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
		public uint containerNetId;
	}

	public class PutDownFailedMsg : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 6;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
		public uint containerNetId;
	}
}
