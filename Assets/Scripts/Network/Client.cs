using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public sealed class Client {
	private static Client _instance;
	public static Client Instance {
		get{
			if(_instance == null)
				return (_instance = new Client());
			else return _instance;
		}
	}

	private TCP TCPConnection;

	public Client(){
		if(_instance == null){
			_instance = this;
		}
		TCPConnection = new TCP();
	}

	public void Connect(string ip, int port){
		TCPConnection.Connect(ip, port);
	}

	private class TCP {
		private TcpClient _socket;
		private NetworkStream _stream;
		private byte[] _recvBuffer;

		public void Connect(string ip, int port){
			_socket = new TcpClient{
				ReceiveBufferSize = Network.DATA_BUFFER_SIZE,
				SendBufferSize = Network.DATA_BUFFER_SIZE
			};
			_recvBuffer = new byte[Network.DATA_BUFFER_SIZE];
			_socket.BeginConnect(ip, port, ConnectCallback, _socket);
		}

		private void ConnectCallback(IAsyncResult result){
			_socket.EndConnect(result);
			if(!_socket.Connected)
				return;
			_stream = _socket.GetStream();
			_stream.BeginRead(_recvBuffer, 0, Network.DATA_BUFFER_SIZE, ReceiveCallback, null);
		}
		private void ReceiveCallback(IAsyncResult result){
			try{
				int bytelen = _stream.EndRead(result);
				if(bytelen <= 0){
					// disconnect
					return;
				}
				byte[] data = new byte[bytelen];
				Array.Copy(_recvBuffer, data, bytelen);
				// handle data

				_stream.BeginRead(_recvBuffer, 0, Network.DATA_BUFFER_SIZE, ReceiveCallback, null);
			}catch{
				// disconnect
			}
		}
	}

	private class UDP {
	}
}