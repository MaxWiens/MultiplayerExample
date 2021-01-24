using UnityEngine;
using NetLib;
using System;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "Client_Client", menuName = "MultiplayerExample/Networking/Client_ClientSO", order = 0)]
public class Client_ClientSO : ScriptableObject {
	public event Action Initalized;
	public event Action StartedConnection;
	public event Action Connected;
	public event Action<Client.DisconnectReason,string> Disconnected;

	public bool IsConnected {get; private set;}
	public PacketRegistry<ClientPacketHandler> Registry {get; private set;}
	[SerializeField, NotNull]
	private GameObject _updaterPrefab = null;
	[SerializeField, NotNull]
	private Client_PacketsSO _packets = null;
	private Client _client = null;
	private FixedUpdater _updater = null;

	private void OnEnable() {
		if(_client != null){
			_client.Disconnected += OnDisconnect;
		}
	}
	private void OnDisable() {
		if(_client != null){
			_client.Disconnected -= OnDisconnect;
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
		Initalized?.Invoke();
	}

	public void Connect(System.Net.IPAddress ipAddress, int port){
		if(!IsConnected){
			StartedConnection?.Invoke();
			Action updateAction = _client.Connect(ipAddress, port);
			if(updateAction==null){
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
		_client.Disconnect(reason, description);
	}

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
}