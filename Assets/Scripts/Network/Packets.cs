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

namespace Network{
	/// <summary>
	/// Handles packet recieved from client
	/// </summary>
	/// <param name="clientIdx">Client which sent the packet</param>
	/// <param name="packetReader">Packet to be read</param>
	public delegate void ServerPacketHandler(byte clientIdx, PacketReader packetReader);

	/// <summary>
	/// Handles packet recieved from server
	/// </summary>
	/// <param name="packetReader">Packet to be read</param>
	public delegate void ClientPacketHandler(PacketReader packetReader);

	/// <summary>
	/// Packets sent from server to client
	/// </summary>
	public enum ServerPackets : ushort {
		welcome = 1,
		serverFull,
	}

	/// <summary>
	/// Packets sent from client to server
	/// </summary>
	public enum ClientPackets : ushort {
		welcomeReceived = 1
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