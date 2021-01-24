using UnityEngine;
using System.Collections.Generic;
using System;

public class Client_PlayerManager : MonoBehaviour {
	public EntityData LocalPlayerData {get; private set;}
	[SerializeField, NotNull]
	private Client_ClientSO _client = null;
	[SerializeField, NotNull]
	private Client_PacketsSO _packets = null;
	[SerializeField, NotNull]
	private GameObjectPoolSO _clientPlayerPool = null;
	[SerializeField, NotNull]
	private GameObject _localPlayerPrefab = null;
	[SerializeField, NotNull]
	private Follow _cameraFollow = null;

	private	Dictionary<byte,EntityData> _players = new Dictionary<byte,EntityData>();

	private void OnEnable() {
		_packets.PlayerJoined += OnPlayerJoined;
		_packets.PlayerTransformUpdate += OnPlayerTransformUpdate;
	}

	private void OnDisable() {
		_packets.PlayerJoined -= OnPlayerJoined;
		_packets.PlayerTransformUpdate -= OnPlayerTransformUpdate;
	}

	private void Start() {
		var q = _packets.PlayerJoinedQueue;
		byte id;
		Vector2 v;
		while(q.Count > 0){
			(id,v) = q.Dequeue();
			OnPlayerJoined(id,v);
		}
	}

	public void OnPlayerJoined(byte playerIdx, Vector2 position){
		EntityData data = new EntityData(playerIdx);
		if(LocalPlayerData == null && playerIdx == _client.ClientIdx){
			GameObject player = Instantiate(_localPlayerPrefab, new Vector3(position.x,0f,position.y), Quaternion.identity);
			data.Object = player;

			player.transform.position = new Vector3(position.x,1f,position.y);
			player.GetComponent<IEntity>().Data = data;
			player.GetComponent<IScorable>().Value = 0;
			LocalPlayerData = data;
			_players.Add(playerIdx, data);
			_cameraFollow.Target = player.transform;
			_cameraFollow.enabled = true;
			player.SetActive(true);
		}else if(!_players.ContainsKey(playerIdx)){
			_players.Add(playerIdx, data);
			SpawnRemotePlayer(data, position);
		}
	}

	public bool SpawnRemotePlayer(EntityData playerData, Vector2 position) {
		GameObject player = _clientPlayerPool.Get();
		if(player == null) return false;
		// set player position;
		player.transform.position = new Vector3(position.x,1f,position.y);
		// set playerdata
		playerData.Object = player;
		player.GetComponent<IEntity>().Data = playerData;
		// set manager callbacks
		player.GetComponent<IManaged>().SetCallbacks(UnmanagePlayer);
		// set score to zero
		player.GetComponent<IScorable>().Value = 0;
		return true;
	}

	private void OnPlayerTransformUpdate(byte playerIdx, Vector2 position, float rotation){
		if(_players.TryGetValue(playerIdx,out EntityData data)){
			SetPlayerTransform(data, position, rotation);
		}
	}

	public void SetPlayerTransform(EntityData playerData, Vector2 position, float rotation){
		playerData.Object.transform.position = new Vector3(position.x, 1f, position.y);
		playerData.Object.transform.eulerAngles = new Vector3(0f, rotation, 0f);
	}

	private void UnmanagePlayer(GameObject player){
		if(!_clientPlayerPool.PutBack(player)){
			Debug.LogWarning("Player not managed by this manager");
		}
	}
}