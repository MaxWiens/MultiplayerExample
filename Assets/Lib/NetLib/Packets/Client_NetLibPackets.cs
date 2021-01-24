using System;

namespace NetLib {
	using ClientPTD = PacketTypeData<ClientPacketHandler>;

	public class Client_NetLibPackets {
		public ClientPTD[] GetPacketData()=> new ClientPTD[]{
			new ClientPTD(PacketNames.Welcome, HandleWelcome),
			new ClientPTD(PacketNames.ServerFull, HandleServerFull)
		};

		public event Action<byte> Welcome;
		public event Action ServerFull;

		private void HandleWelcome(PacketReader packetReader){
			Welcome?.Invoke(packetReader.NextByte());
		}

		private void HandleServerFull(PacketReader packetReader){
			ServerFull?.Invoke();
		}
	}
}