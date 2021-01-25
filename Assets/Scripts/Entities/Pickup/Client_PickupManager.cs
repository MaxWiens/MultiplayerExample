using UnityEngine;
using System;
using System.Collections.Generic;
public class Client_PickupManager : MonoBehaviour {
	[SerializeField, NotNull]
	private GameObjectPoolSO _pickupPool = null;
	[SerializeField, NotNull]
	private Client_PacketsSO _packets = null;

	private Dictionary<byte,EntityData> _pickups = new Dictionary<byte, EntityData>();

	private void OnEnable() {
		_packets.PickupSpawned += OnPickupSpawned;
		_packets.PickupRemoved += OnPickupRemoved;
	}

	private void OnDisable() {
		_packets.PickupSpawned -= OnPickupSpawned;
		_packets.PickupRemoved -= OnPickupRemoved;
	}
	private void Start() {
		var q = _packets.PlayerJoinedQueue;
		byte id;
		Vector2 v;
		while(q.Count > 0){
			(id,v) = q.Dequeue();
			OnPickupSpawned(id,v);
		}
	}

	private void OnPickupRemoved(byte pickupId){
		if(_pickups.TryGetValue(pickupId, out EntityData data)){
			data.Object.SetActive(false);
		}
	}

	private void OnPickupSpawned(byte pickupid, Vector2 position){
		if(!_pickups.ContainsKey(pickupid)){
			EntityData data = new EntityData(pickupid);
			SpawnPickup(position, data);
		}
	}

	public GameObject SpawnPickup(Vector2 location, EntityData entityData){
		GameObject pickup = _pickupPool.Get();
		if(pickup != null){
			entityData.Object = pickup;
			pickup.transform.position = new Vector3(location.x, 1f, location.y);
			pickup.GetComponent<IManaged>().SetCallbacks(UnmanagePickup);
			pickup.GetComponent<IEntity>().Data = entityData;
			_pickups.Add(entityData.ID, entityData);
		}
		return pickup;
	}

	private void UnmanagePickup(GameObject managedObject){
		if(!_pickupPool.PutBack(managedObject))
			Debug.LogError("Pickup not managed by this manager");
		_pickups.Remove(managedObject.GetComponent<IEntity>().Data.ID);
	}
}