using UnityEngine;
using NetLib;
using System;
using System.Collections.Generic;
using ClientPTD = NetLib.PacketTypeData<NetLib.ClientPacketHandler>;
[CreateAssetMenu(fileName = "Client_Packets", menuName = "MultiplayerExample/Networking/Client_PacketsSO", order = 0)]
public class Client_PacketsSO : ScriptableObject {
	public Queue<(byte,Vector2)> PlayerJoinedQueue = new Queue<(byte, Vector2)>();
	public event Action<byte,Vector2> PlayerJoined;
	public event Action<byte,Vector2,float> PlayerTransformUpdate;
	public event Action<byte,int> PlayerScoreChanged;

	public Queue<(byte,Vector2)> PickupSpawnedQueue = new Queue<(byte, Vector2)>();
	public event Action<byte,Vector2> PickupSpawned;
	public event Action<byte> PickupRemoved;

	public ushort PlayerMoveID {get; private set;}

	[SerializeField, NotNull]
	private Client_ClientSO _client = null;

	public ClientPTD[] GetPacketData() => new ClientPTD[]{
		new ClientPTD(PacketNames.PLAYER_JOINED, PlayerJoinedHandler),
		new ClientPTD(PacketNames.PLAYER_REMOVED, PlayerRemovedHandler),
		new ClientPTD(PacketNames.PLAYER_TRANSFORM_UPDATE, PlayerTransformUpdateHandler),
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
		byte playerID = packetReader.NextByte();
		Vector2 position = packetReader.NextVector2();
		if(PlayerJoined == null){
			PlayerJoinedQueue.Enqueue((playerID, position));
		}else{
			PlayerJoined.Invoke(playerID, position);
		}
	}

	private void PlayerRemovedHandler(PacketReader packetReader){
		throw new NotImplementedException();
	}

	private void PlayerTransformUpdateHandler(PacketReader packetReader){
		if(PlayerTransformUpdate != null){
			byte playerid = packetReader.NextByte();
			Vector2 pos = packetReader.NextVector2();
			float lookRotation = packetReader.NextFloat();
			PlayerTransformUpdate(playerid, pos, lookRotation);
		}
	}

	private void PlayerScoreHandler(PacketReader packetReader){
		if(PlayerScoreChanged != null){
			byte playerId = packetReader.NextByte();
			int score = packetReader.NextInt();
			PlayerScoreChanged(playerId, score);
		}
	}

	// Pickup
	private void PickupSpawnedHandler(PacketReader packetReader){
		byte pickupId = packetReader.NextByte();
		Vector2 position = packetReader.NextVector2();
		if(PickupSpawned != null){
			PickupSpawned(pickupId, position);
		}else{
			PickupSpawnedQueue.Enqueue((pickupId,position));
		}
	}
	private void PickupRemovedHandler(PacketReader packetReader){
		PickupRemoved?.Invoke(packetReader.NextByte());
	}
}