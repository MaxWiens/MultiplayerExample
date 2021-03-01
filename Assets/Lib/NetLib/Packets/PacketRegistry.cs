using System;
using System.Collections.Generic;
using System.Linq;
namespace NetLib {
	public static class PacketRegistry{
		public enum RegisterResult : byte {
			Successful,
			Full,
			NameAlreadyRegistered
		}
	}

	public class PacketRegistry<Handler> where Handler : Delegate {
		private Dictionary<string, PacketType> _packets = new Dictionary<string, PacketType>();
		private List<PacketType> _packetList = new List<PacketType>();
		public Dictionary<ushort, Handler> Handlers = new Dictionary<ushort, Handler>();

		public PacketRegistry(List<PacketTypeData<Handler>> packetTypesData){
			foreach(PacketTypeData<Handler> d in (from packet in packetTypesData orderby packet select packet)){
				PacketRegistry.RegisterResult result = Register(d);
				switch(result){
					case PacketRegistry.RegisterResult.Full:
						throw new RegisterException("Packet registry became full");
					case PacketRegistry.RegisterResult.NameAlreadyRegistered:
						throw new RegisterException($"Packet name \"{d.Name}\" already registered");
				}
			}
		}

		public PacketType GetPacket(ushort id)
			=> id < _packetList.Count ? _packetList[id] : null;

		public PacketType GetPacket(string name)
			=> _packets.TryGetValue(name, out PacketType packet) ? packet : null;

		public ushort GetID(string name)
			=> _packets.TryGetValue(name, out PacketType packet) ? packet.ID : (ushort)0;

		public int PacketTypeCount => _packetList.Count;

		private PacketRegistry.RegisterResult Register(PacketTypeData<Handler> packetData){
			if(_packets.ContainsKey(packetData.Name))
				return PacketRegistry.RegisterResult.NameAlreadyRegistered;
			int count = _packetList.Count;
			if(count <= ushort.MaxValue){
				PacketType packetType;
				Handler handler = packetData.Handler;
				if(handler != null){
					packetType = new HandlerPacketType<Handler>(packetData.Name, (ushort)count, handler);
					Handlers.Add((ushort)count, handler);
				}else{
					packetType = new PacketType(packetData.Name, (ushort)count);
				}
				_packetList.Add(packetType);
				_packets.Add(packetData.Name, packetType);
				return PacketRegistry.RegisterResult.Successful;
			}else{
				return PacketRegistry.RegisterResult.Full;
			}
		}
	}
}