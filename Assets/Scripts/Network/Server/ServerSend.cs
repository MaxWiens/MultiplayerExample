namespace Network{
	public class ServerSend{
		public static void Welcome(byte clientIdx, string msg){
			UnityEngine.Debug.Log("Adding  player2");
			using (PacketBuilder pb = new PacketBuilder(ServerPackets.welcome)) {
				pb.Write(clientIdx);
				pb.Write(msg);
				Server.SendTCP(clientIdx, pb.Build());
			}
		}
	}
}