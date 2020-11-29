using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour{
	public static ThreadManager Instance {get; private set;}

	private void Awake() {
		if(Instance == null){
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}else
			Destroy(gameObject);
	}

	private void Update() => Network.ThreadManager.Update();
}