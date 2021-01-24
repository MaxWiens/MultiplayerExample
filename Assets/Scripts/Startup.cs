using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.InputSystem;

static class Startup {

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Initalize() {
		NetLib.Globals.DebugLog = Debug.Log;
		// check if running headless server
		if(Application.isBatchMode){
			// game is being run headless so start server immediately
			SceneManager.LoadScene("Server_CollectGameScene");
		}else{
			//
		}
	}
}