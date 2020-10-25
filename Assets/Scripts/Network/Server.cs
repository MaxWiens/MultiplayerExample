using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Collections.Generic;

public static class Server{
	public static int Port {get; private set;}

	public static int MaxPlayers {get; private set;}
	public static bool IsStarted {get; private set;}

	private static Dictionary<int,Client> _clients = new Dictionary<int, Client>();

	private static TcpListener _tcpListener;


	public static void Start(int maxPlayers, int port) {
		Port = port;
		MaxPlayers = maxPlayers;

		Debug.Log($"Starting Server on port {Port}");
		InitServerData();

		_tcpListener = new TcpListener(IPAddress.Any, Port);
		_tcpListener.Start();
		_tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

		IsStarted = true;
	}

	private static void TCPConnectCallback(IAsyncResult result){
		TcpClient client = _tcpListener.EndAcceptTcpClient(result);
		_tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

		Debug.Log($"Incomming connection from {client.Client.RemoteEndPoint}");
		
		for(int i=0; i < MaxPlayers; i++){
			if(_clients[i].TCPConnection.Socket == null){
				_clients[i].TCPConnection.Connect(client);
				return;
			}
		}
		
		Debug.Log("Failed to connect");
	}

	private static void InitServerData(){
		for(int i=0; i < MaxPlayers; i++){
			_clients.Add(i, new Client(i));
		}
	}


	private class Client {
		
		private readonly int _id;
		public TCP TCPConnection;
		public UDP UDPConnection;
		
		public Client(int id){
			_id = id;
			TCPConnection = new TCP(_id);
			UDPConnection = new UDP();
		}


		public class TCP {
			public TcpClient Socket;
			private readonly int _id;
			private NetworkStream _stream;
			private byte[] _recvBuffer;

			public TCP(int id) => _id = id;

			public void Connect(TcpClient socket){
				Socket = socket;
				Socket.ReceiveBufferSize = Network.DATA_BUFFER_SIZE;
				Socket.SendBufferSize = Network.DATA_BUFFER_SIZE;

				_stream = socket.GetStream();

				_recvBuffer = new byte[Network.DATA_BUFFER_SIZE];

				_stream.BeginRead(_recvBuffer, 0, Network.DATA_BUFFER_SIZE, RecvCallback, null);
			}

			private void RecvCallback(IAsyncResult result) {
				try{
					int bytelen = _stream.EndRead(result);
					if(bytelen <= 0){
						//disconnect
						return;
					}
					byte[] data = new byte[bytelen];
					Array.Copy(_recvBuffer, data, bytelen);
					
					// handle data
					_stream.BeginRead(_recvBuffer, 0, Network.DATA_BUFFER_SIZE, RecvCallback, null);
				} catch(Exception ex) when (ex is System.IO.IOException || ex is ObjectDisposedException) {
					Debug.Log($"Error reciving TCP data {ex}");
					// close connection

				}
			}
			

		}
		
		public class UDP {
			
		}
	}
}