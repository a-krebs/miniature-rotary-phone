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

	public class RequestAuthMessage : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 1;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
	}

	public class GrantAuthMessage : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 2;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
	}

	public class DenyAuthMessage : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 3;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
	}

	public class ReleaseAuthMessage : MessageBase
	{
		public static short Type = UnityEngine.Networking.MsgType.Highest + 4;
		public uint requestId;
		public uint playerNetId;
		public uint objNetId;
	}
}
