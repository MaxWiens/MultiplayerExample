using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using System.Net;

public class MainMenuManager : MonoBehaviour {
	[SerializeField, NotNull]
	private Button _connectButton = null;
	[SerializeField, NotNull]
	private Button _hostButton = null;

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

	private void OnEnable() {
		_connectButton.onClick.AddListener(Connect);
		_hostButton.onClick.AddListener(StartServer);
	}

	private void OnDisable() {
		_connectButton.onClick.RemoveListener(Connect);
		_hostButton.onClick.RemoveListener(StartServer);
	}

	public void StartServer(){
		if(System.Int32.TryParse(_hostPort.text, out int port)){
			_serverSO.StartServer(port, _maxPlayers);
		}
		else
			DisplayMessageCoroutine("Bad Server Port");
	}

	public void Connect(){
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
		StartCoroutine(ConnectingCoroutine(Game.Instance.Connect(ipAddress, port)));
	}

	private IEnumerator ConnectingCoroutine(Task<bool> t){
		for(;;){
			if(t.IsCompleted){
				if(!t.Result){
					// Failure to connect (timeout)
					StartCoroutine(DisplayMessageCoroutine("Failed to connect: Timeout"));
				}
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