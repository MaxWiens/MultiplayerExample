using UnityEngine;

public class Server_PlayerControl : MonoBehaviour {
	[SerializeField]
	private float _speed = 5f;
	private Vector3 v;
	public void Move(Vector2 direction){
		v.x = direction.x;
		v.z = direction.y;
		transform.position += v * (_speed * Time.deltaTime);
	}
}