using System.Collections.Generic;
using UnityEngine;
using NetLib;
using System;
[CreateAssetMenu(fileName = "Server_Server", menuName = "MultiplayerExample/Networking/Server_ServerSO", order = 0)]
public class Server_ServerSO : ScriptableObject {

	public event Action Initalized;
	public event Action Started;
	public event Action Stopped;
	public bool IsRunning {get; private set;}
	public PacketRegistry<ServerPacketHandler> Registry {get; private set;}

	[SerializeField, NotNull]
	private Server_PacketsSO _packets = null;
	[SerializeField, NotNull]
	private GameObject _updaterPrefab = null;
	private FixedUpdater _updater = null;
	private Server _server = null;

	private void OnDisable(){
		if(IsRunning){
			StopServer();
		}
	}

	public void Initalize(){
		Debug.Log("Creating Server");
		_server = new Server(_packets.GetPacketData());
		Registry = _server.Registry;
		Initalized?.Invoke();
	}

	public void StartServer(int port, ushort maxClients){
		if(!IsRunning){
			if(_server == null){
				Initalize();
			}
			Debug.Log("Starting Server");
			Action serverTicker = _server.Start(port, maxClients);
			IsRunning = true;
			if(_updater == null){
				GameObject updaterInstance = Instantiate(_updaterPrefab);
				DontDestroyOnLoad(updaterInstance);
				_updater = updaterInstance.GetComponent<FixedUpdater>();
			}
			_updater.UpdateAction = serverTicker;
			_updater.enabled = true;
			Started?.Invoke();
		}
	}

	public void StopServer(){
		if(IsRunning){
			// stop the server
			_updater.enabled = false;
			_updater.UpdateAction = null;
			Stopped?.Invoke();
		}
	}

	public void SendTCP(byte clientID, byte[] packetData)
		=> _server?.SendTCP(clientID, packetData);

	public void SendTCPAll(byte[] packetData)
		=> _server?.SendTCPAll(packetData);

	public void SendUDP(byte clientID, byte[] packetData)
		=> _server?.SendUDP(clientID, packetData);

	public void SendUDPAll(byte[] packetData)
		=> _server?.SendUDPAll(packetData);
}