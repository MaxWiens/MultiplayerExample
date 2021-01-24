using NetLib;
using UnityEngine;

public static class ServerSend{
	private static ushort _entitySimpleTransformID = 0;
	public static void SendEntitySimpleTransform(int playerIdx, Transform transform){
		throw new System.NotImplementedException("Finish this");
		Vector3 pos = transform.position;
		using(PacketBuilder pb = new PacketBuilder(_entitySimpleTransformID)) {
			// write player index
			pb.Write(playerIdx);
			// Write position
			pb.Write(pos.x);
			pb.Write(pos.z);
			// Write rotation around y axis
			pb.Write(transform.rotation.eulerAngles.y);
			// build and send
			//ServerLogic.SendUDPAll(pb.Build());
		}
	}

	public static void EntityPosition(byte toClientIdx, uint entityIdx){
		throw new System.NotImplementedException("Finish this");
		/*
		using (PacketBuilder pb = new PacketBuilder(ServerPackets.welcome)) {
			// pb.Write(clientIdx);
			// pb.Write(msg);
			// Server.SendUDP(toClientIdx, pb.Build());
		}
		*/
	}
}




