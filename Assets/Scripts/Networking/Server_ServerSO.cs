using System.Collections.Generic;
using UnityEngine;
using NetLib;
using System;
using UnityEditor;


[CreateAssetMenu(fileName = "Server_Server", menuName = "MultiplayerExample/Networking/Server_ServerSO", order = 0)]
public class Server_ServerSO : ScriptableObject {

	public event Action Initalized;
	public event Action Started;
	public event Action Stopping;
	public event Action<byte> PlayerJoined;
	public event Action<byte> PlayerLeft;
	public bool IsRunning {get; private set;} = false;
	public PacketRegistry<ServerPacketHandler> Registry {get; private set;}

	[SerializeField, NotNull]
	private Server_PacketsSO _packets = null;
	[SerializeField, NotNull]
	private GameObject _updaterPrefab = null;
	private FixedUpdater _updater = null;
	private Server _server = null;


	#if UNITY_EDITOR
	private void OnEnable() {
		EditorApplication.playModeStateChanged += OnPlayModeStateChange;
	}
	#endif

	private void OnDisable(){
		#if UNITY_EDITOR
		EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
		#endif
		StopServer();
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
			_server.ClientConnected += OnClientConnected;
			_server.ClientDisconnected += OnClientDisconnect;
			Started?.Invoke();
		}
	}

	private void OnClientConnected(byte playerID){
		PlayerJoined?.Invoke(playerID);
	}

	private void OnClientDisconnect(byte playerID){
		PlayerLeft?.Invoke(playerID);
	}
	public IEnumerable<byte> ClientIdxs => _server.ClientIdxs;

	public void StopServer(){
		if(IsRunning){
			Debug.Log("stopping server");
			// stop the server
			Stopping?.Invoke();
		}
		ResetSO();
	}

	private void ResetSO(){
		IsRunning = false;
		if(_server != null){
			_server.ClientConnected -= OnClientConnected;
			_server.ClientDisconnected -= OnClientDisconnect;
			_server = null;
		}
		if(_updater != null){
			_updater.UpdateAction = null;
			_updater.enabled = false;
		}
	}

	public void SendTCP(byte clientID, byte[] packetData)
		=> _server?.SendTCP(clientID, packetData);

	public void SendTCPAll(byte[] packetData)
		=> _server?.SendTCPAll(packetData);

	public void SendTCPAll(byte[] packetData, byte excludeClientID)
		=> _server?.SendTCPAll(packetData, excludeClientID);

	public void SendUDP(byte clientID, byte[] packetData)
		=> _server?.SendUDP(clientID, packetData);

	public void SendUDPAll(byte[] packetData)
		=> _server?.SendUDPAll(packetData);

	#if UNITY_EDITOR
	private void OnPlayModeStateChange(PlayModeStateChange obj){
		if(obj == PlayModeStateChange.ExitingPlayMode){
			OnDisable();
		}
	}
	#endif
}