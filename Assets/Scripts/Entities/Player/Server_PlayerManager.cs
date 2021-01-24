using UnityEngine;
using UnityEditor;
using NetLib;
public class Server_PlayerManager : MonoBehaviour {
	[SerializeField, NotNull]
	private GameObjectPoolSO _managedPlayerPool = null;
	[SerializeField, NotNull]
	private Transform _spawnPoint = null;
	[SerializeField, NotNull]
	private Server_ServerSO _server = null;

	private ushort SpawnPlayerID;
	public const string PLAYER_SPAWN_PACKET_NAME = "MultiplayerExample_PlayerSpawn";
	private void OnEnable() {
		SpawnPlayerID = SpawnPlayerID = _server.Registry.GetID(PLAYER_SPAWN_PACKET_NAME);
	}

	public bool SpawnPlayer(PlayerData playerData) {
		GameObject player = _managedPlayerPool.Get();
		if(player == null) return false;
		// set player position;
		player.transform.position = _spawnPoint.position;
		// set playerdata
		playerData.Object = player;
		player.GetComponent<IEntity>().Data = playerData;
		// set manager callbacks
		player.GetComponent<IManaged>().SetCallbacks(DeactivatePlayer);
		// set score to zero
		player.GetComponent<IScorable>().Value = 0;

		// send new player info
		using (PacketBuilder p = new PacketBuilder(SpawnPlayerID)){
			p.Write(new Vector2(_spawnPoint.position.x, _spawnPoint.position.z));
			_server.SendTCPAll(p.Build());
		}
		return true;
	}

	public void UpdatePositions(){
		Vector2 v = new Vector2();
		foreach(var p in _managedPlayerPool.ActiveObjects){
			v.Set(p.transform.position.x, p.transform.position.z);
			// update positions and send to players
			using (PacketBuilder pb = new PacketBuilder(SpawnPlayerID)){
				pb.Write(v);
				_server.SendUDPAll(pb.Build());
			}
		}
	}

	public void DeactivatePlayer(GameObject player){
		if(!_managedPlayerPool.PutBack(player)){
			Debug.LogWarning("Player not managed by this manager");
		}
		//send data
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(Server_PlayerManager))]
public class Server_PlayerManager_Editor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if(Application.IsPlaying(target)){
			if(GUILayout.Button("Spawn Player"))
				((Server_PlayerManager)target).SpawnPlayer(new PlayerData(-1, System.DateTime.Now));
		}
	}
}
#endif