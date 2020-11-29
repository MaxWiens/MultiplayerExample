using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System;

namespace Network {
	public static class Server {
		public static ushort MaxClients { get; private set;}
		public static int Port { get; private set; }
		public static Dictionary<ushort, ServerPacketHandler> PacketHandlers;

		private static Client[] _clients;
		private static Stack<byte> _avilableClientIndices;

		private static TcpListener _tcpListener;
		private static UdpClient _udpSocket;

		/// <summary>
		/// Starts server
		/// </summary>
		/// <param name="port">Port to send and receive messages from</param>
		/// <param name="maxClients">Max number of clients to support. Max 256</param>
		public static void Start(int port, ushort maxClients){
			Debug.Log($"Starting server on port {port}, max clients {maxClients}...");

			if(maxClients > 256)
				throw new ArgumentOutOfRangeException("Max clients greater than 256");
			else if(maxClients == 0)
				throw new ArgumentOutOfRangeException("Max clients is zero");

			MaxClients = maxClients;
			Port = port;
			PacketHandlers = new Dictionary<ushort, ServerPacketHandler>();

			_clients = new Client[MaxClients];
			_avilableClientIndices = new Stack<byte>(MaxClients);
			for(byte clientIdx = 0; clientIdx < MaxClients; clientIdx++ )
				_avilableClientIndices.Push(clientIdx);
			Debug.Log(_avilableClientIndices.Count);

			_tcpListener = new TcpListener(IPAddress.Any, Port);
			_tcpListener.Start();
			_tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

			_udpSocket = new UdpClient(Port);
			_udpSocket.BeginReceive(UDPRecvCallback, null);

			Debug.Log("Server started!");
		}

		/// <summary>
		/// Sends a packet to a client using TCP
		/// </summary>
		/// <param name="clientIdx">Client to send packet to</param>
		/// <param name="packet">Packet to send</param>
		public static void SendTCP(byte clientIdx, byte[] packet){
			Client client;
			if((client = _clients[clientIdx]) != null){
				try{
					client.TcpStream.BeginWrite(packet, 0, packet.Length, null, null);
				}catch (Exception ex){
					Debug.Log($"Error sending data to client {clientIdx} using TCP: {ex}");
				}
			}
			else
				Debug.Log($"Attempt to send packet to {clientIdx} who doesn't exist");
		}

		/// <summary>
		/// Sends a packet to all clients using TCP
		/// </summary>
		/// <param name="packet">Packet to send</param>
		public static void SendTCPAll(byte[] packet){
			Client client;
			for (int idx = 0; idx < _clients.Length; idx++){
				if((client = _clients[idx]) != null){
					try{
						client.TcpStream.BeginWrite(packet, 0, packet.Length, null, null);
					}catch (Exception ex){
						Debug.Log($"Error sending data to client {idx} using TCP: {ex}");
					}
				}
				else
					Debug.Log($"Attempt to send packet to {idx} who doesn't exist");
			}
		}

		/// <summary>
		/// Sends a packet to all clients excluding one using TCP
		/// </summary>
		/// <param name="packet">Packet to send</param>
		/// <param name="clientIdx">Client to exclude</param>
		public static void SendTCPAll(byte[] packet, byte clientIdx){
			Client client;
			for (int idx = 0; idx < _clients.Length; idx++){
				if(idx != clientIdx && (client = _clients[idx]) != null){
					try{
						client.TcpStream.BeginWrite(packet, 0, packet.Length, null, null);
					}catch (Exception ex){
						Debug.Log($"Error sending data to client {idx} using TCP: {ex}");
					}
				}
				else
					Debug.Log($"Attempt to send packet to {idx} who doesn't exist");
			}
		}

		/// <summary>
		/// Sends a packet to a client using UDP
		/// </summary>
		/// <param name="clientIdx">Client to send packet to</param>
		/// <param name="packet">Packet to send</param>
		public static void SendUDP(byte clientIdx, byte[] packet){
			Client client;
			if((client = _clients[clientIdx]) != null){
				try{
					_udpSocket.BeginSend(packet, packet.Length, client.IPEndPoint, null, null);
				}catch (Exception ex){
					Debug.Log($"Error sending data to client {clientIdx} using TCP: {ex}");
				}
			}
			else
				Debug.Log($"Attempt to send packet to {clientIdx} who doesn't exist");
		}

		/// <summary>
		/// Sets up new client to server
		/// </summary>
		/// <param name="result">Result of the connection</param>
		private static void TCPConnectCallback(IAsyncResult result){
			TcpClient socket = _tcpListener.EndAcceptTcpClient(result);
			_tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null); // continue to listen for annother connection asynchronously
			Debug.Log($"Incoming connection from {socket.Client.RemoteEndPoint}");

			// add client to clients array
			if(_avilableClientIndices.Count > 0){
				Debug.Log("Adding  player");
				byte newClientIdx = _avilableClientIndices.Pop();
				_clients[newClientIdx] = new Client(socket, newClientIdx);
				ServerSend.Welcome(newClientIdx, "Welcome! -The Server");
			}
			else{
				Debug.Log($"{socket.Client.RemoteEndPoint} failed to connect. Server full!");
				using(PacketBuilder pb = new PacketBuilder(ServerPackets.serverFull)){
					byte[] packet = pb.Build();
					_udpSocket.BeginSend(packet, packet.Length, (IPEndPoint)socket.Client.RemoteEndPoint, null, null);
				}

			}
		}

		/// <summary>
		/// Recieves UDP data
		/// </summary>
		/// <param name="result">Result of the recieve</param>
		private static void UDPRecvCallback(IAsyncResult result){
			try{
				IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = _udpSocket.EndReceive(result, ref endPoint);
				_udpSocket.BeginReceive(UDPRecvCallback, null); // continue to listen for more data asynchronously

				if(data.Length <= 0) return;

				using(PacketReader pr = new PacketReader(data)){
					byte clientIdx = pr.NextByte();
					Client client;
					if((client = _clients[clientIdx]) != null && client.EndPointStr == endPoint.ToString()){ // TODO string comparison?

						// handle data
						int packetlength = pr.NextInt();
						if(pr.UnreadLength >= packetlength){
							byte[] packetBytes = pr.NextBytes(packetlength);
							ThreadManager.ExecuteOnMainThread(()=>{
								using(PacketReader packetReader = new PacketReader(packetBytes)){
									ushort packetType = (ushort)packetReader.NextShort();
									if(PacketHandlers.TryGetValue(packetType, out ServerPacketHandler handler))
										handler(clientIdx, packetReader);
								}
							});
						}
						else throw new Exception("ERROR, impartial udp packet?!?");

					}
				}
			}catch(Exception ex){
				Debug.Log($"Error receiving UDP data: {ex}");
				return;
			}
			throw new NotImplementedException("finish this");
		}

		private static void Disconnect(byte clientIdx){
			if(_clients[clientIdx] != null){
				_clients[clientIdx].TcpStream.Close();
				_clients[clientIdx] = null;
			}
		}

		private class Client {
			public readonly string EndPointStr;
			public readonly IPEndPoint IPEndPoint;
			public readonly byte ClientIndex;
			public TcpClient Socket;
			public NetworkStream TcpStream;
			private byte[] _tcpRecvBuffer;
			private PacketReader _packetReader;

			/// <summary>
			/// Initalize and connect client
			/// </summary>
			/// <param name="socket">Client Socket to connect to</param>
			public Client(TcpClient socket, byte index){
				//IPEndPoint e = new IPEndPoint()
				ClientIndex = index;
				Socket = socket;
				EndPointStr = Socket.Client.RemoteEndPoint.ToString();
				IPEndPoint = Socket.Client.RemoteEndPoint as IPEndPoint;
				if(IPEndPoint == null){
					throw new Exception("CASTING IPENDPOINT WEIRDNESS");
				}
				Socket.ReceiveBufferSize = Constants.DATA_BUFFER_SIZE;
				Socket.SendBufferSize = Constants.DATA_BUFFER_SIZE;

				TcpStream = Socket.GetStream();

				_tcpRecvBuffer = new byte[Constants.DATA_BUFFER_SIZE];

				TcpStream.BeginRead(_tcpRecvBuffer, 0, Constants.DATA_BUFFER_SIZE, TCPReceiveCallback, null);
			}

			public void TCPReceiveCallback(IAsyncResult result){
				try{
					int byteLength = TcpStream.EndRead(result);
					if(byteLength <= 0){
						Disconnect(ClientIndex);
						return;
					}
					byte[] data = new byte[byteLength];
					Array.Copy(_tcpRecvBuffer, data, byteLength);

					TCPHandleData(data);
				}catch(Exception ex){
					Debug.Log($"Error receiving TCP data: {ex}");
					Disconnect(ClientIndex);
				}
			}

			private void TCPHandleData(byte[] packet){
				int packetlength = 0;

				//append the latest packet/partial packet to the current packet reader
				_packetReader.AppendData(packet);

				// read first int of data (should be the length of the packet sent)
				if(_packetReader.UnreadLength >= sizeof(int)){
					packetlength = _packetReader.PeekInt();
					if(packetlength <= 0) return;
					_packetReader.NextInt();
				}

				while(packetlength > 0 && packetlength <= _packetReader.UnreadLength){
					byte[] packetBytes = _packetReader.NextBytes(packetlength);

					ThreadManager.ExecuteOnMainThread(()=>{
						using(PacketReader pr = new PacketReader(packetBytes)){
							ushort packetType = pr.NextUShort();
							if(PacketHandlers.TryGetValue(packetType, out ServerPacketHandler handler))
								handler(ClientIndex, pr);
						}
					});

					packetlength = 0;
					if(_packetReader.UnreadLength >= sizeof(int)){
						packetlength  = _packetReader.PeekInt();
						if(packetlength <= 0) return;
						_packetReader.NextInt();
					}
				}
			}
		}
	}
}
