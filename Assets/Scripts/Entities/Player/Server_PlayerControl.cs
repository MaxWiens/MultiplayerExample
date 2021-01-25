using UnityEngine;

public class Server_PlayerControl : MonoBehaviour {
	[SerializeField, NotNull]
	private Rigidbody _rb = null;
	[SerializeField]
	private float _speed = 5f;
	private Vector3 v = Vector3.zero;
	private bool _isStopping = false;
	private float _stoppingTime = 0f;
	private Vector3 stoppingVelocity = Vector3.zero;
	private Vector3 _moveDirection;
	private void FixedUpdate() {
		if(_moveDirection.magnitude == 0f && _rb.velocity.magnitude > 0){
			_rb.velocity = Vector3.SmoothDamp(_rb.velocity, Vector3.zero, ref stoppingVelocity, _stoppingTime);
		}else{
			_rb.velocity = _moveDirection * _speed;
		}
	}
	public void Move(Vector2 direction){
		_isStopping = false;
		_moveDirection.x = direction.x;
		_moveDirection.z = direction.y;
		_moveDirection.Normalize();
		transform.localRotation = Quaternion.LookRotation(Vector3.down, v);
	}
}