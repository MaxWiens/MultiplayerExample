using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using System.Net;

public class MainMenuManager : MonoBehaviour {

	[SerializeField]
	private Button _connectButton;
	[SerializeField]
	private Button _hostButton;

	[SerializeField]
	private TMPro.TMP_InputField _connectIP;
	[SerializeField]
	private TMPro.TMP_InputField _connectPort;
	[SerializeField]
	private TMPro.TMP_InputField _hostPort;
	[SerializeField]
	private ushort _maxPlayers = 10;

	[SerializeField]
	private GameObject _connectHost;

	[SerializeField]
	private TMPro.TMP_Text _connectMessage;

	private void Start() {
		_connectButton.onClick.AddListener(Connect);
		_hostButton.onClick.AddListener(StartServer);
	}

	public void StartServer(){
		if(System.Int32.TryParse(_hostPort.text, out int port))
			Network.Server.Start(port, _maxPlayers);
		else
			Debug.Log("Bad Server Port");
	}

	public void Connect(){
		bool issue = false;
		if(!IPAddress.TryParse(_connectIP.text, out IPAddress ipAddress)){
			Debug.Log("Bad IP Address");
			issue = true;
		}
		if(!System.Int32.TryParse(_connectPort.text, out int port)){
			Debug.Log("Bad Client Port");
			issue = true;
		}
		if(issue) return;
		StartCoroutine(ConnectingCoroutine(Network.Client.Instance.Connect(ipAddress, port)));

	}

	private IEnumerator ConnectingCoroutine(Task<bool> t){

		_connectHost.SetActive(false);

		for(;;){
			if(t.IsCompleted){
				if(t.Result){
					// Connected Successfully
					// transition to next scene

				}else{
					// Failure to connect (timeout)
					StartCoroutine(FailedToConnectCoroutine("Timeout"));
				}
				yield break;
			}
			yield return null;
		}
	}

	private IEnumerator FailedToConnectCoroutine(string failureReason){
		// display message
		SetConnectMessage($"Failed to Connect: {failureReason}");
		// return to connect/host menu
		_connectHost.SetActive(true);
		yield break;
	}

	private void SetConnectMessage(string message) => _connectMessage.text = message;
}