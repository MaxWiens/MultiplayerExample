using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections;
using System;


/// <summary>
/// Client Game Manager
/// </summary>
public class Game : MonoBehaviour {
	public static Game Instance;
	// public Actions Input;
	private static Action _clientUpdate = null;
	public static bool IsConnected {get; private set;} = false;

	private void Awake() {
		if(Instance == null){
			Instance = this;
			//Input = new Actions();
			DontDestroyOnLoad(this);
		}else{
			Destroy(this);
			return;
		}

		NetLib.Globals.DebugLog = Debug.Log;

		// register handlers for client and server
	}

	private void OnApplicationQuit() {
		// disconnect from server
		// stop server
		//NetLib.Client.Instance.Disconnect();
		//Network.Server.
		throw new System.NotImplementedException("Finish this");
	}

	public Task<bool> Connect(System.Net.IPAddress ipAddress, int port){
		throw new System.NotImplementedException("Finish this");
		// Task<bool> connectTask = NetLib.Client.Instance.Connect(ipAddress, port, out Action updateAction);
		// _clientUpdate = updateAction;

		// return connectTask;
	}

	private void Update(){
		if(IsConnected){
			_clientUpdate();
		}
	}

	private IEnumerator ConnectingCoroutine(Task<bool> t){
		for(;;){
			if(t.IsCompleted){
				if(t.Result){
					// Connected Successfully
					// transition to next scene
					Debug.Log("connected successfully");
					SceneManager.LoadScene("CollectGameScene");
				}else{
					// Failure to connect (timeout)
					Debug.Log("failed to conenct (timeout)");
				}
				yield break;
			}
			yield return null;
		}
	}

	public void Disconnect(){

	}

}
