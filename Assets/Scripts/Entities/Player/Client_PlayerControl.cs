using UnityEngine;

public class Client_PlayerControl : MonoBehaviour {
	[SerializeField, NotNull]
	private InputManagerSO _input = null;
	[SerializeField]
	private Transform _playerCamera = null;

	private Vector3 _previousInputMoveVec;

	private void OnEnable() {
		_input.Moved += OnInputMoveVectorChange;
	}

	private void OnDisable() {
		_input.Moved -= OnInputMoveVectorChange;
	}

	private void Update() {
		Vector3 v = _playerCamera.forward;
		v.y = 0;
		v = Quaternion.LookRotation(v.normalized) * _previousInputMoveVec;
		// send v to server
	}

	public void OnInputMoveVectorChange(Vector2 inputMoveVec){
		_previousInputMoveVec.Set(inputMoveVec.x,0f,inputMoveVec.y);
		_previousInputMoveVec.Normalize();
	}
}