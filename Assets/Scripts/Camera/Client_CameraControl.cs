using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client_CameraControl : MonoBehaviour {
	[SerializeField]
	private InputManagerSO _inputManager = null;
	[SerializeField, Tooltip("Inverse of follow strength")]
	private float _dapenTime = 0.1f;
	[SerializeField]
	private float _maxAngle = 90f;
	[SerializeField]
	private float _minAngle = -5f;

	private Vector2 _rotation;
	private Vector2 _rotationDelta;

	private Vector2 _velocity;

	private void OnEnable() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = true;
		_inputManager.CameraRotated += OnCameraRotate;
	}

	private void OnDisable() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = false;
		_inputManager.CameraRotated -= OnCameraRotate;
	}

	private void Update() {
		if(_rotationDelta.y+_rotation.y < _minAngle){
			_rotationDelta.y = _minAngle - _rotation.y;
		}else if(_rotationDelta.y+_rotation.y > _maxAngle){
			_rotationDelta.y = _maxAngle - _rotation.y;
		}

		_rotationDelta = Vector2.SmoothDamp(_rotationDelta, Vector2.zero, ref _velocity, _dapenTime);
		_rotation += _rotationDelta;
		_rotation.x %= 360f;
		_rotation.y %= 360f;
		transform.Rotate(Vector3.up, _rotationDelta.x, Space.World);
		transform.Rotate(Vector3.right, _rotationDelta.y, Space.Self);
	}

	private void OnCameraRotate(Vector2 direction){
		if(direction.magnitude != 0)
			_rotationDelta = direction*_inputManager.Sensitivity;
	}
}