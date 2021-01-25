using UnityEngine;

public class Server_PlayerControl : MonoBehaviour {
	[SerializeField, NotNull]
	private Rigidbody _rb = null;
	[SerializeField]
	private float _speed = 5f;
	private bool _isStopping = false;
	private float _stoppingTime = 0f;
	private Vector3 _stoppingVelocity = Vector3.zero;
	private Vector3 _moveDirection = Vector3.zero;
	private void FixedUpdate() {
		if(!_isStopping && _moveDirection.magnitude == 0f && _rb.velocity.magnitude > 0){
			_isStopping = true;
			_stoppingTime = 0f;
			_stoppingVelocity = Vector3.zero;
		}
		if(_isStopping){
			_stoppingTime += Time.fixedDeltaTime;
			_rb.velocity = Vector3.SmoothDamp(_rb.velocity, Vector3.zero, ref _stoppingVelocity, _stoppingTime);
			if(_stoppingTime >= 1f){
				_isStopping = false;
			}
		}else{
			_rb.velocity = _moveDirection * _speed;
		}
	}
	public void Move(Vector2 direction){
		_isStopping = false;
		_moveDirection.x = direction.x;
		_moveDirection.z = direction.y;
		_moveDirection.Normalize();
		transform.localRotation = Quaternion.LookRotation(Vector3.down, _moveDirection);
	}
}