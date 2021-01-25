using UnityEngine;
using UnityEditor;
using NetLib;
using System.Collections.Generic;

public class Server_PickupManager : MonoBehaviour {
	[SerializeField, NotNull]
	private Server_ServerSO _server = null;
	[SerializeField, NotNull]
	private Server_PacketsSO _packets = null;
	[SerializeField, NotNull]
	private GameObjectPoolSO _pickupPool = null;
	[SerializeField]
	private Timer _spawnTimer = new Timer();
	[SerializeField]
	public bool IsGenerating = false;
	[SerializeField, NotNull]
	private Transform _center = null;
	[SerializeField]
	private float _radius  = 0f;
	private Queue<byte> _availableIds = new Queue<byte>();
	private Dictionary<byte,EntityData> _pickups = new Dictionary<byte, EntityData>();

	private void Start() {
		_spawnTimer.Finished += SpawnPickup;
	}

	private void OnDestroy() {
		_spawnTimer.Finished -= SpawnPickup;
	}

	private void Update(){
		if(IsGenerating){
			_spawnTimer.Tick(Time.deltaTime);
		}
	}

	public void SpawnPickup(){
		GameObject pickup = _pickupPool.Get();
		if(pickup == null){
			_spawnTimer.StopUntil(()=>_pickupPool.HasObjectAvailable, true);
		}else{
			Vector2 randomPos = Random.insideUnitCircle*_radius;
			pickup.transform.position = _center.position + new Vector3(randomPos.x,0f,randomPos.y);
			pickup.GetComponent<IManaged>().SetCallbacks(DeactivateManagedObject);
			byte id;
			if(_availableIds.Count > 0){
				id = _availableIds.Dequeue();
			}else{
				id = (byte)_pickups.Count;
			}
			EntityData data = new EntityData(id, pickup);
			pickup.GetComponent<IEntity>().Data = data;
			_pickups.Add(id, data);
			using(PacketBuilder pb = new PacketBuilder(_packets.PickupSpawnedID)){
				pb.Write(id);
				Vector3 v = pickup.transform.position;
				pb.Write(new Vector2(v.x,v.z));
				_server.SendTCPAll(pb.Build());
			}
		}
	}

	public void DeactivateManagedObject(GameObject managedObject) {
		if(managedObject.TryGetComponent<IEntity>(out IEntity entity) && _pickups.Remove(entity.Data.ID)){
			if(!_pickupPool.PutBack(managedObject))
				Debug.LogError("Player not managed by this manager");
			_pickups.Remove(entity.Data.ID);
			_availableIds.Enqueue(entity.Data.ID);
			using(PacketBuilder pb = new PacketBuilder(_packets.PickupRemovedID)){
				pb.Write(entity.Data.ID);
				_server.SendTCPAll(pb.Build());
			}
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Server_PickupManager))]
public class Server_PickupManager_Editor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if(Application.IsPlaying(target)){
			if(GUILayout.Button("Create Pickup"))
				((Server_PickupManager)target).SpawnPickup();
		}
	}
}
#endif