﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Game Manager
/// </summary>
public class Game : MonoBehaviour {
	public static Game Instance;
	public static Actions Input;

	private void Awake() {
		if(Instance == null)
			Instance = this;
		else{
			Destroy(this);
			return;
		}

		Input = new Actions();
		DontDestroyOnLoad(this);
	}

	private void OnApplicationQuit() {
		// disconnect from server
		// stop server
		Network.Client.Instance.Disconnect();
		//Network.Server.
		throw new System.NotImplementedException("Finish this");
	}
}
