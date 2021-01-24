using UnityEngine;
using System;
public class Client_PickupManager : ScriptableObject {
	[SerializeField, NotNull]
	private GameObjectPoolSO _pickupPool = null;
	//[SerializeField, NotNull]
	//private

	private void OnEnable() {
		//+= SpawnPickup;
	}

	private void OnDisable() {
		//-= SpawnPickup;
	}

	public GameObject SpawnPickup(Vector2 location, EntityData entityData){
		GameObject pickup = _pickupPool.Get();
		pickup.transform.position = new Vector3(location.x, 0f, location.y);
		pickup.GetComponent<IManaged>().SetCallbacks(DeactivateManagerObject);
		pickup.GetComponent<IEntity>().Data = entityData;
		return pickup;
	}

	public static void ReadPickupSpawn(byte[] packetData){

	}


	public void RemovePickup(GameObject pickup){
		_pickupPool.PutBack(pickup);
	}

	public void DeactivateManagerObject(GameObject managedObject){
		if(!_pickupPool.PutBack(managedObject))
			Debug.LogError("Pickup not managed by this manager");
	}
}