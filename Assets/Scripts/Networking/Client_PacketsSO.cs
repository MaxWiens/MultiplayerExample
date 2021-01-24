using UnityEngine;
using NetLib;
using System;
using ClientPTD = NetLib.PacketTypeData<NetLib.ClientPacketHandler>;
[CreateAssetMenu(fileName = "Client_Packets", menuName = "MultiplayerExample/Networking/Client_PacketsSO", order = 0)]
public class Client_PacketsSO : ScriptableObject {
	public ushort PlayerMoveID {get; private set;}

	[SerializeField, NotNull]
	private Client_ClientSO _client = null;

	public ClientPTD[] GetPacketData() => new ClientPTD[]{
		new ClientPTD(PacketNames.PLAYER_JOINED, PlayerJoinedHandler),
		new ClientPTD(PacketNames.PLAYER_REMOVED, PlayerRemovedHandler),
		new ClientPTD(PacketNames.PLAYER_POSITION, PlayerPositionHandler),
		new ClientPTD(PacketNames.PLAYER_SCORE, PlayerScoreHandler),
		new ClientPTD(PacketNames.PLAYER_MOVE),

		new ClientPTD(PacketNames.PICKUP_SPAWNED, PickupSpawnedHandler),
		new ClientPTD(PacketNames.PICKUP_REMOVED, PickupRemovedHandler),
	};

	private void OnClientInitalization(){
		PacketRegistry<ClientPacketHandler> r = _client.Registry;
		PlayerMoveID = r.GetID(PacketNames.PLAYER_MOVE);
	}

	private void OnEnable() {
		_client.Initalized += OnClientInitalization;
	}
	private void OnDisable() {
		_client.Initalized -= OnClientInitalization;
	}

	private void PlayerJoinedHandler(PacketReader packetReader){
		throw new NotImplementedException();
	}
	private void PlayerRemovedHandler(PacketReader packetReader){
		throw new NotImplementedException();
	}
	private void PlayerPositionHandler(PacketReader packetReader){
		throw new NotImplementedException();
	}
	private void PlayerScoreHandler(PacketReader packetReader){
		throw new NotImplementedException();
	}

	// Pickup
	private void PickupSpawnedHandler(PacketReader packetReader){
		throw new NotImplementedException();
	}
	private void PickupRemovedHandler(PacketReader packetReader){
		throw new NotImplementedException();
	}
}