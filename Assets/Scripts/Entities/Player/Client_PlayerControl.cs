using UnityEngine;
using NetLib;
public class Client_PlayerControl : MonoBehaviour {
	[SerializeField, NotNull]
	private InputManagerSO _input = null;
	[SerializeField, NotNull]
	private Client_ClientSO _client = null;
	[SerializeField, NotNull]
	private Client_PacketsSO _packets = null;
	[SerializeField]
	private Transform _playerCamera = null;

	private Vector3 _inputMoveVec;

	private void OnEnable() {
		_input.Moved += OnInputMoveVectorChange;
	}
	private void OnDisable() {
		_input.Moved -= OnInputMoveVectorChange;
	}
	private void Start() {
		if(_playerCamera == null && (_playerCamera = Camera.main.transform) == null){
			Debug.LogError("PlayerControl could not find camera");
		}
	}

	private Vector3 _prev;
	private void FixedUpdate() {
		Vector3 v = _playerCamera.forward;
		v = Quaternion.LookRotation(v.normalized) * _inputMoveVec;

		if(v == _prev) return;
		_prev = v;
		// send v to server
		using(PacketBuilder packetBuilder = new PacketBuilder(_packets.PlayerMoveID)){
			packetBuilder.Write(new Vector2(v.x,v.z));
			_client.SendUDP(packetBuilder.Build());
		}
	}

	public void OnInputMoveVectorChange(Vector2 inputMoveVec){
		_inputMoveVec.Set(inputMoveVec.x,0f,inputMoveVec.y);
		_inputMoveVec.Normalize();
	}
}