using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using System.Collections;

[CreateAssetMenu(fileName = "SceneManagerSO", menuName = "MultiplayerExample/SceneManagerSO", order = 0)]
public class SceneManagerSO : ScriptableObject {
	[SerializeField, NotNull]
	private GameObject _coroutineBehaviourPrefab = null;

	private void OnEnable() {
		#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += OnPlayModeChanged;
		#endif
		GameObject instance = Instantiate(_coroutineBehaviourPrefab);
		DontDestroyOnLoad(instance);
	}

	private void OnDisable() {

	}

	#if UNITY_EDITOR
	private void OnPlayModeChanged(PlayModeStateChange obj){
		if(obj == PlayModeStateChange.ExitingPlayMode){
			OnDisable();
		}
	}
	#endif

	public void LoadSceneUnloadCurrent(string scenename){
		SceneManager.LoadScene(scenename, LoadSceneMode.Additive);
		//SceneManager.UnloadSceneAsync();
	}

	public void LoadScene(string scenename){
		SceneManager.LoadScene(scenename, LoadSceneMode.Additive);

	}

	private IEnumerator UnloadCurrentSceneCoroutine(){
		yield break;
	}
}