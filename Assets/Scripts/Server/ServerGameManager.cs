using UnityEngine;
using System;
using System.Collections.Generic;
using NetLib;

public class ServerGameManager : MonoBehaviour {
	public static ServerGameManager Instance;

	[SerializeField]
	private GameObject _playerPrefab = null;

	[SerializeField]
	private Action _serverUpdate = null;

	private void Awake() {
		if(Instance == null){
			Instance = this;
			DontDestroyOnLoad(this);
		}else{
			Destroy(this);
			return;
		}

		AddHandlers();
	}

	private void AddHandlers(){
		throw new System.NotImplementedException("Finish this");
		//ServerLogic.PacketHandlers.Add( , );
	}

	private Dictionary<byte,ManagedEntity> _players = new Dictionary<byte,ManagedEntity>();

	public void StartServer(int port, ushort _maxPlayers) {
		throw new System.NotImplementedException("Finish this");
		//NetLib.ServerLogic.Start(port, _maxPlayers, AddPlayer, out Action update);
		//_serverUpdate = update;
	}

	private void Update(){
		if(_serverUpdate != null)
			_serverUpdate();
	}

	private void OnApplicationQuit() {
		// close server
		throw new System.NotImplementedException("Finish this");
	}

	public void AddPlayer(byte playerIdx) => _players.Add(playerIdx, Instantiate(_playerPrefab, new Vector3(0,1,0), Quaternion.identity).GetComponent<ManagedEntity>());

	public ManagedEntity GetPlayer(byte playerIdx) => _players[playerIdx];
}
