
namespace NetLib {
	using ServerPTD = PacketTypeData<ServerPacketHandler>;
	public class Server_NetLibPackets {
		public ushort WelcomeID;
		public ushort ServerFullID;
		public ServerPTD[] GetPacketData()=> new ServerPTD[]{
			new ServerPTD(PacketNames.Welcome),
			new ServerPTD(PacketNames.ServerFull)
		};
	}
}