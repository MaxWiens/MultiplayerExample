using UnityEngine;
using UnityEngine.UI;

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
	private int _maxPlayers = 10;
	
	private void Start() {
		_connectButton.onClick.AddListener(Connect);
		_hostButton.onClick.AddListener(StartServer);
	}

	public void StartServer(){
		if(System.Int32.TryParse(_hostPort.text, out int port))
			Server.Start(_maxPlayers, port);
		else
			Debug.Log("Bad Server Port");
	}

	public void Connect(){
		if(System.Int32.TryParse(_connectPort.text, out int port))
			Client.Instance.Connect(_connectIP.text, port);
		else
			Debug.Log("Bad Client Port");
	}
}