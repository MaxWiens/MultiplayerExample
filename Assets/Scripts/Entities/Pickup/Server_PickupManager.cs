using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class Server_PickupManager : MonoBehaviour {
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
			AnnounceNewItem(pickup);
		}
	}

	private void AnnounceNewItem(GameObject managedObject){
		// send to all players
	}

	public void DeactivateManagedObject(GameObject managedObject) {
		if(!_pickupPool.PutBack(managedObject))
			Debug.LogError("Player not managed by this manager");
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