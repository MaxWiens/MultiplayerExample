using UnityEngine;
using UnityEditor;
using NetLib;
using System.Collections.Generic;
using System;

public class Server_PlayerManager : MonoBehaviour {
	[SerializeField, NotNull]
	private GameObjectPoolSO _managedPlayerPool = null;
	[SerializeField, NotNull]
	private Transform _spawnPoint = null;
	[SerializeField, NotNull]
	private Server_ServerSO _server = null;
	[SerializeField, NotNull]
	private Server_PacketsSO _packets = null;

	private	Dictionary<byte,EntityData> _players = new Dictionary<byte,EntityData>();

	private void OnEnable() {
		_server.PlayerJoined += OnPlayerJoined;
		_server.PlayerLeft += OnPlayerLeave;
		_packets.PlayerMoved += OnPlayerMove;
	}

	private void OnDisable() {
		_server.PlayerJoined -= OnPlayerJoined;
		_server.PlayerLeft -= OnPlayerLeave;
		_packets.PlayerMoved -= OnPlayerMove;
	}

	private void Start() {
		foreach(byte id in _server.ClientIdxs){
			EntityData playerData = new EntityData(id);
			_players.Add(id, playerData);
			if(!SpawnPlayer(playerData)){
				Debug.LogError("Unable to spawn player, pool is full!");
			}
		}
	}

	private void OnPlayerMove(byte playerIdx, Vector2 moveVec){
		if(_players.TryGetValue(playerIdx, out EntityData data)){
			data.Object.GetComponent<Server_PlayerControl>().Move(moveVec);
		}
	}

	private void OnPlayerJoined(byte playerIdx){
		if(!_players.ContainsKey(playerIdx)){
			EntityData playerData = new EntityData(playerIdx);
			_players.Add(playerIdx, playerData);
			if(!SpawnPlayer(playerData)){
				Debug.LogError("Unable to spawn player, pool is full!");
			}
		}
	}

	private void OnPlayerLeave(byte playerIdx){
		if(_players.TryGetValue(playerIdx, out EntityData playerData)){
			RemovePlayer(playerData);
		}
	}

	public bool SpawnPlayer(EntityData playerData) {
		GameObject player = _managedPlayerPool.Get();
		if(player == null) return false;
		// set player position;
		player.transform.position = _spawnPoint.position;
		// set playerdata
		playerData.Object = player;
		player.GetComponent<IEntity>().Data = playerData;
		// set manager callbacks
		player.GetComponent<IManaged>().SetCallbacks(UnmanagePlayer);
		// set score to zero
		Score s = player.GetComponent<Score>();
		s.Value = 0;
		s.ScoreChanged += SendScore;


		foreach(var pl in _players.Values){
			if(pl.ID != playerData.ID){
				using (PacketBuilder p = new PacketBuilder(_packets.PlayerJoinedID)){
					p.Write(pl.ID);
					Vector3 v = pl.Object.transform.position;
					p.Write(new Vector2(v.x, v.z));
					_server.SendTCP(playerData.ID, p.Build());
				}
			}
		}

		// send new player info
		using (PacketBuilder p = new PacketBuilder(_packets.PlayerJoinedID)){
			p.Write(playerData.ID);
			p.Write(new Vector2(_spawnPoint.position.x, _spawnPoint.position.z));
			_server.SendTCPAll(p.Build());
		}
		return true;
	}

	private void SendScore(Score s){
		using (PacketBuilder p = new PacketBuilder(_packets.PlayerScoreID)){
			p.Write(s.GetComponent<IEntity>().Data.ID);
			p.Write(s.Value);
			_server.SendUDPAll(p.Build());
		}
	}

	private void FixedUpdate() {
		UpdateTransforms();
	}

	public void UpdateTransforms(){
		Vector2 v = new Vector2();
		Vector3 vp;
		foreach(var p in _players.Values){
			vp = p.Object.transform.position;
			v.Set(vp.x, vp.z);
			// update positions and send to players
			using (PacketBuilder pb = new PacketBuilder(_packets.PlayerTransformUpdateID)){
				pb.Write(p.ID);
				pb.Write(v);
				pb.Write(p.Object.transform.eulerAngles.y);
				_server.SendUDPAll(pb.Build());
			}
		}
	}

	public void RemovePlayer(EntityData playerData){
		if(_players.ContainsValue(playerData)){
			using (PacketBuilder pb = new PacketBuilder(_packets.PlayerRemovedID)){
				pb.Write(playerData.ID);
				_server.SendUDPAll(pb.Build());
			}
			UnmanagePlayer(playerData.Object);
		}else{
			Debug.LogWarning("attempt to remove player not managed by player manager");
		}
	}

	private void UnmanagePlayer(GameObject player){
		if(!_managedPlayerPool.PutBack(player)){
			player.GetComponent<Score>().ScoreChanged -= SendScore;
			Debug.LogWarning("Player not managed by this manager");
		}
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(Server_PlayerManager))]
public class Server_PlayerManager_Editor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if(Application.IsPlaying(target)){
			if(GUILayout.Button("Spawn Player"))
				((Server_PlayerManager)target).SpawnPlayer(new EntityData(0));
		}
	}
}
#endif