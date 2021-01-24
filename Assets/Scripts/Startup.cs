using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.InputSystem;

static class Startup {

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Initalize() {

		// check if running headless server
		if(Application.isBatchMode){
			// game is being run headless so start server immediately
			SceneManager.LoadScene("ServerCollectGameScene");
		}else{
			//
		}
	}
}