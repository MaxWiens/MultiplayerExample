using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Network {
	public class Client {
		public enum ClientState : byte {
			Disconnected,
			Connecting,
			Connected
		}

		public byte ClientIdx;
		public static Client Instance {
			get{
				if(_instance == null)
					return (_instance = new Client());
				else return _instance;
			}
		}

		public static ClientState State { get; private set; }

		public static Dictionary<ushort, ClientPacketHandler> PacketHandlers;

		private static Client _instance;

		private const int TIMEOUT_SECONDS = 10000;

		private Client(){
			PacketHandlers = new Dictionary<ushort, ClientPacketHandler>(){
				{(ushort)ServerPackets.welcome, ClientHandler.Welcome},
				//{(ushort)ServerPackets.serverFull, null},
			};
			State = ClientState.Disconnected;
		}

		public Task<bool> Connect(IPAddress ip, int port){
			_tcpSocket = new TcpClient{
				ReceiveBufferSize = Constants.DATA_BUFFER_SIZE,
				SendBufferSize = Constants.DATA_BUFFER_SIZE
			};

			// connect to udp
			_endPoint = new IPEndPoint(ip, port);
			Socket sock =  _tcpSocket.Client;

			// _tcpSocket.Client.LocalEndPoint
			// IPEndPoint asdf = new IPEndPoint()
			// IPEndPoint _ipendpoint = (sock.LocalEndPoint);

			// EndPoint p = _tcpSocket.Client.LocalEndPoint;
			// if(p == null){
			// 	UnityEngine.Debug.Log("p is null");
			// }
			// _udpSocket = new UdpClient(((IPEndPoint)p).Port);
			// _udpSocket.Connect(_endPoint);
			// _udpSocket.BeginReceive(UDPReceiveCallback, null);


			// connect to tcp
			_tcpRecvBuffer = new byte[Constants.DATA_BUFFER_SIZE];
			State = ClientState.Connecting;
			_tcpSocket.BeginConnect(ip, port, (IAsyncResult result)=>{
					_tcpSocket.EndConnect(result);
					if(!_tcpSocket.Connected) return;
					_stream = _tcpSocket.GetStream();
					_stream.BeginRead(_tcpRecvBuffer, 0, Constants.DATA_BUFFER_SIZE, TCPReceiveCallback, null);
				}, _tcpSocket);



			return ConnectingAsync();
		}

		private async Task<bool> ConnectingAsync(){
			Task timeoutTask = Task.Delay(TIMEOUT_SECONDS);
			for(;;){
				if(State == ClientState.Connected){
					return true;
				}
				if(timeoutTask.IsCompleted)
					return false;
				await Task.Yield();
			}
		}

		public void FinalizeConnection(byte clientIdx){
			ClientIdx = clientIdx;
			State = ClientState.Connected;
		}

		public void Disconnect(){
			if(State == ClientState.Disconnected)
				return;

			_tcpSocket.Close();
			_udpSocket.Close();

			_stream.Dispose(); // needed?
			_stream = null;

			_packetReader.Dispose(); // needed?
			_packetReader = null;

			_tcpRecvBuffer = null;
			State = ClientState.Disconnected;

			UnityEngine.Debug.Log("Disconnected from server");
		}

		//*********************
		//******TCP Stuff******
		//*********************
		private TcpClient _tcpSocket;
		private NetworkStream _stream;
		private byte[] _tcpRecvBuffer;
		private PacketReader _packetReader = new PacketReader();

		public void SendTCP(byte[] packet){
			if(State == ClientState.Connected){
				try{
					_stream.BeginWrite(packet, 0, packet.Length, null, null);
				}catch(Exception ex){
					UnityEngine.Debug.Log($"Error sending TCP message: {ex}");
				}
			}else{
				UnityEngine.Debug.Log($"Error sending TCP message: not connected to server");
			}
		}

		private void TCPReceiveCallback(IAsyncResult result){
			try{
				int readLength = _stream.EndRead(result);
				if(readLength <=0){
					Disconnect();
					return;
				}
				byte[] data = new byte[readLength];
				Array.Copy(_tcpRecvBuffer, data, readLength);

				TCPHandleData(data);

				_stream.BeginRead(_tcpRecvBuffer, 0, Constants.DATA_BUFFER_SIZE, TCPReceiveCallback, null);
			}catch(Exception e){
				UnityEngine.Debug.Log($"Exception while reciving tcp {e.ToString()}");
				Disconnect();
			}
		}

		public void TCPHandleData(byte[] data){
			int packetlength = 0;
			//append the latest packet/partial packet to the current packet reader
			_packetReader.AppendData(data);

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
						if(PacketHandlers.TryGetValue(packetType, out ClientPacketHandler handler))
							handler(pr);
						else UnityEngine.Debug.Log($"Client: no packethandler for {packetType}");
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

		//*********************
		//******UDP Stuff******
		//*********************
		private UdpClient _udpSocket;
		private IPEndPoint _endPoint;

		public void SendUDP(byte[] packet){
			if(State == ClientState.Connected){
				try{
					_udpSocket.BeginSend(packet, packet.Length, null, null);
				}catch(Exception ex){
					UnityEngine.Debug.Log($"Error sending TCP message: {ex}");
				}
			}else{
				UnityEngine.Debug.Log($"Error sending TCP message: not connected to server");
			}
		}

		private void UDPReceiveCallback(IAsyncResult result){
			try{
				byte[] data = _udpSocket.EndReceive(result, ref _endPoint);
				_udpSocket.BeginReceive(UDPReceiveCallback, null);

				// Did the server send a packet that is too small?
				if(data.Length < sizeof(int)){
					Disconnect();
					return;
				}else{
					UDPHandleData(data);
				}
			}catch{
				Disconnect();
			}
		}

		private void UDPHandleData(byte[] data){
			byte[] packetBody;
			using(PacketReader pr = new PacketReader(data)){
				int packetLength = pr.NextInt();
				packetBody = pr.NextBytes(packetLength);
			}

			ThreadManager.ExecuteOnMainThread(()=>{
				using(PacketReader pr = new PacketReader(packetBody)){
					ushort packetType = pr.NextUShort();
					if(PacketHandlers.TryGetValue(packetType, out ClientPacketHandler handler))
						handler(pr);
				}
			});
		}
	}
}
