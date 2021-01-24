using UnityEngine;

public class Server_CollectingGameManager : MonoBehaviour {
	[SerializeField, NotNull]
	private Server_PlayerManager _playerManager;
	[SerializeField, NotNull]
	private Server_PickupManager _pickupManager;
}