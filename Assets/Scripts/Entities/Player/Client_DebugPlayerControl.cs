using UnityEngine;

public class Client_DebugPlayerControl : MonoBehaviour {
	[SerializeField]
	private InputManagerSO _input = null;
	[SerializeField]
	private Transform _playerCamera = null;
	[SerializeField]
	private float _speed = 5f;

	private Vector3 _previousInputMoveVec;

	private void OnEnable() {
		_input.Moved += OnInputMoveVectorChange;
	}

	private void OnDisable() {
		_input.Moved -= OnInputMoveVectorChange;
	}

	private Vector3 _velocity = Vector3.zero;
	[SerializeField]
	private float _dampenTime = 0.1f;
	private bool _isDampening = false;
	private void Update() {
		Vector3 v = _playerCamera.forward;
		v.y = 0;
		v = Quaternion.LookRotation(v.normalized) * _previousInputMoveVec;
		transform.position += v * (_speed * Time.deltaTime);
		if(_isDampening)
			_previousInputMoveVec = Vector3.SmoothDamp(_previousInputMoveVec, Vector3.zero, ref _velocity, _dampenTime);
	}

	public void OnInputMoveVectorChange(Vector2 inputMoveVec){
		if(inputMoveVec.magnitude == 0){
			_isDampening = true;
		}else{
			_previousInputMoveVec.Set(inputMoveVec.x,0f,inputMoveVec.y);
			_previousInputMoveVec.Normalize();
			_isDampening = false;
		}
	}
}