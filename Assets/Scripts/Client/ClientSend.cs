using UnityEngine;
using NetLib;

public static class ClientSend {

	/// <summary>
	/// Vector which player intends to move. This will be normalized on the server.
	/// </summary>
	/// <param name="move">Movement ector</param>
	public static void SendMovement(Vector2 move){
		// using(PacketBuilder pb = new PacketBuilder()){
		// 	pb.Write(move);
		// 	Client.Instance.SendUDP(pb.Build());
		// }
	}
}