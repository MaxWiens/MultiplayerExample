using UnityEngine;

public class EntityData {
	public readonly byte ID = 0;
	public GameObject Object = null;

	public EntityData(byte id){
		ID = id;
	}
	public EntityData(byte id, GameObject obj){
		ID = id;
		Object = obj;
	}
}