using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using System.Net;
using UnityEngine.SceneManagement;
using System;

public class MainMenuManager : MonoBehaviour {
	[SerializeField, NotNull]
	private Button _connectButton = null;
	[SerializeField, NotNull]
	private Button _hostButton = null;
	[SerializeField, NotNull]
	private Button _hostAndPlayButton = null;

	[SerializeField, NotNull]
	private TMPro.TMP_InputField _connectIP = null;
	[SerializeField, NotNull]
	private TMPro.TMP_InputField _connectPort = null;
	[SerializeField, NotNull]
	private TMPro.TMP_InputField _hostPort = null;
	[SerializeField]
	private ushort _maxPlayers = 10;
	[SerializeField, NotNull]
	private GameObject _connectHost = null;
	[SerializeField, NotNull]
	private TMPro.TMP_Text _connectMessage = null;
	[SerializeField, NotNull]
	private Server_ServerSO _serverSO = null;
	[SerializeField, NotNull]
	private Client_ClientSO _clientSO = null;

	private void OnEnable() {
		_connectButton.onClick.AddListener(OnConnectClicked);
		_hostButton.onClick.AddListener(OnHostClicked);
		_hostAndPlayButton.onClick.AddListener(OnHostAndPlayClicked);
	}

	private void OnDisable() {
		_connectButton.onClick.RemoveListener(OnConnectClicked);
		_hostButton.onClick.RemoveListener(OnHostClicked);
		_hostAndPlayButton.onClick.RemoveListener(OnHostAndPlayClicked);
	}

	private void OnHostClicked(){
		if(System.Int32.TryParse(_hostPort.text, out int port)){
			_serverSO.StartServer(port, _maxPlayers);
		}
		else
			DisplayMessageCoroutine("Bad Server Port");
		SceneManager.LoadScene("Server_CollectGameScene", LoadSceneMode.Additive);
	}

	private void OnHostAndPlayClicked(){
		if(System.Int32.TryParse(_hostPort.text, out int port)){
			_serverSO.StartServer(port, _maxPlayers);
		}
		else
			DisplayMessageCoroutine("Bad Server Port");

		_clientSO.Connect(IPAddress.Parse("127.0.0.1"), port);
		StartCoroutine(ConnectingCoroutine(_clientSO.TimeoutSeconds, true));
	}

	private void OnConnectClicked(){
		bool issue = false;
		if(!IPAddress.TryParse(_connectIP.text, out IPAddress ipAddress)){
			StartCoroutine(DisplayMessageCoroutine("Bad IP Address"));
			issue = true;
		}
		if(!System.Int32.TryParse(_connectPort.text, out int port)){
			StartCoroutine(DisplayMessageCoroutine("Bad Client Port"));
			issue = true;
		}
		if(issue) return;

		_connectHost.SetActive(false);
		_clientSO.Connect(ipAddress, port);
		StartCoroutine(ConnectingCoroutine(_clientSO.TimeoutSeconds));
	}

	private IEnumerator ConnectingCoroutine(float timeoutSeconds, bool loadServerScene = false){
		float time = 0f;
		for(;;){
			if(_clientSO.IsConnected){
				if(loadServerScene){
					SceneManager.LoadScene("Server_CollectGameScene");
					SceneManager.LoadScene("Client_CollectGameScene", LoadSceneMode.Additive);
				}else{
					SceneManager.LoadScene("Client_CollectGameScene");
				}
				yield break;
			}
			time += Time.deltaTime;
			if(time >= timeoutSeconds){
				_clientSO.Disconnect(NetLib.Client.DisconnectReason.Custom, "Timeout");
				StartCoroutine(DisplayMessageCoroutine("Failed to connect: Timeout"));
				yield break;
			}
			yield return null;
		}
	}

	private IEnumerator DisplayMessageCoroutine(string message){
		// display message
		_connectMessage.text = message;
		// return to connect/host menu
		_connectHost.SetActive(true);
		yield break;
	}
}