using UnityEngine;
using System.Collections;
public class CoroutineBehaviour : MonoBehaviour {
	public IEnumerator Coroutine;
	private void OnEnable() {
		StartCoroutine(Coroutine);
	}
	private void OnDisable() {
		StopCoroutine(Coroutine);
	}
}