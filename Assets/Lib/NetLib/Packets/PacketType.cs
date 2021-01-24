using System;
using System.Collections.Generic;
using System.Text;


// Packets are formatted like the following:
//
// Client to Server Packets using TCP and all Server to client packets (TCP & UDP)
// int packet body length (added automatically with PacketBuilder.Build)
// ushort packet type (what method to call as in PacketHandler call)
// bytes... body
//
// Client to Server Packets using UDP
// int clientIndex
// int packet body length
// ushort packet type (what method to call as in PacketHandler)
// bytes... body
//
// Note: when packets are sent, multiple packets are sent so the bytes of a read

namespace NetLib{
	public delegate void ClientPacketHandler(PacketReader packetReader);
	public delegate void ServerPacketHandler(byte sendingClientIdx, PacketReader packetReader);

	public class PacketType {
		public readonly string Name;
		public readonly ushort ID;
		public PacketType(string name, ushort id){
			Name = name;
			ID = id;
		}
	}

	public class HandlerPacketType<H> : PacketType where H : Delegate {
		public H Handle {get; private set;}
		public HandlerPacketType(string name, ushort id, H handler) : base(name, id) {
			Handle = handler;
		}
	}

	[System.Serializable]
	public class PacketException : System.Exception
	{
		public PacketException() { }
		public PacketException(string message) : base(message) { }
		public PacketException(string message, System.Exception inner) : base(message, inner) { }
		protected PacketException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}