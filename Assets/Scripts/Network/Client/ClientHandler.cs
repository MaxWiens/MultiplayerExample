using UnityEngine;
namespace Network{
	public static class ClientHandler {
		public static void Welcome(PacketReader packetReader){
			byte clientIdx = packetReader.NextByte();
			string msg = packetReader.NextString();
			Debug.Log($"Message from server: {msg}");
			Client.Instance.FinalizeConnection(clientIdx);
		}
	}
}
