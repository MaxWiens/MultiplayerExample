using UnityEngine;
using NetLib;
using System;
using UnityEditor;

[CreateAssetMenu(fileName = "Client_Client", menuName = "MultiplayerExample/Networking/Client_ClientSO", order = 0)]
public class Client_ClientSO : ScriptableObject {
	public event Action Initalized;
	public event Action StartedConnection;
	public event Action Connected;
	public event Action<Client.DisconnectReason,string> Disconnected;
	public float TimeoutSeconds => _timeoutSeconds;

	public bool IsConnected {get; private set;}
	public PacketRegistry<ClientPacketHandler> Registry {get; private set;}

	[SerializeField]
	private float _timeoutSeconds = 10f;
	[SerializeField, NotNull]
	private Client_PacketsSO _packets = null;
	[SerializeField, NotNull]
	private GameObject _updaterPrefab = null;
	private Client _client = null;
	private FixedUpdater _updater = null;

	private void OnEnable() {
		#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += OnPlayModeChanged;
		#endif
		ResetSO();
	}

	private void ResetSO() {
		IsConnected = false;
		if(_client != null){
			_client.Disconnected -= OnDisconnect;
			_client.ConnectionFinalized -= OnConnectionFinalized;
		}
		_client = null;
	}

	private void OnDisable() {
		#if UNITY_EDITOR
		EditorApplication.playModeStateChanged -= OnPlayModeChanged;
		#endif
		if(_client != null){
			Disconnect(Client.DisconnectReason.UserDisconnected, "Closed application or Client ScriptableObject Disabled");
		}
	}

	public void Initalize(){
		if(_client != null){
			Debug.Log("Destroying Old Client");
			_client.Disconnected -= OnDisconnect;
		}
		Debug.Log("Creating Client");
		_client = new Client(_packets.GetPacketData());
		Registry = _client.Registry;
		_client.Disconnected += OnDisconnect;
		_client.ConnectionFinalized += OnConnectionFinalized;
		Initalized?.Invoke();
	}

	public void Connect(System.Net.IPAddress ipAddress, int port){
		if(!IsConnected){
			if(_client == null){
				Initalize();
			}
			StartedConnection?.Invoke();
			Action updateAction = _client.Connect(ipAddress, port);
			if(_updater==null){
				GameObject updaterInstance = Instantiate(_updaterPrefab);
				DontDestroyOnLoad(updaterInstance);
				_updater = updaterInstance.GetComponent<FixedUpdater>();
			}
			_updater.UpdateAction = updateAction;
			_updater.enabled = true;
			//start listening for
		}
	}

	public void Disconnect(Client.DisconnectReason reason, string description){
		if(_client != null){
			_client.Disconnect(reason, description);
		}
	}

	private void OnConnectionFinalized(){
		Debug.Log("Client: Connection Finalized");
		IsConnected = true;
		Connected?.Invoke();
	}

	public byte ClientIdx => _client.ClientIdx;

	private void OnDisconnect(Client.DisconnectReason reason, string description){
		Debug.Log($"Disconnected: {description}");
		Disconnected?.Invoke(reason, description);
		_updater.enabled = false;
		_updater.UpdateAction = null;
	}

	public void SendTCP(byte[] packetData)
		=> _client?.SendTCP(packetData);
	public void SendUDP(byte[] packetData)
		=> _client?.SendUDP(packetData);

	#if UNITY_EDITOR
	private void OnPlayModeChanged(PlayModeStateChange obj){
		if(obj == PlayModeStateChange.ExitingPlayMode){
			OnDisable();
		}
	}
	#endif
}