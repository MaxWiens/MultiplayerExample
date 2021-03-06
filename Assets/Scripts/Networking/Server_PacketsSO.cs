using UnityEngine;
using NetLib;
using System;

using ServerPTD = NetLib.PacketTypeData<NetLib.ServerPacketHandler>;

[CreateAssetMenu(fileName = "Server_Packets", menuName = "MultiplayerExample/Networking/Server_PacketsSO", order = 0)]
public class Server_PacketsSO : ScriptableObject {
	[SerializeField, NotNull]
	private Server_ServerSO _server = null;
	public event Action<byte,Vector2> PlayerMoved;
	public ushort PlayerJoinedID {get; private set;}
	public ushort PlayerRemovedID {get; private set;}
	public ushort PlayerTransformUpdateID {get; private set;}
	public ushort PlayerScoreID {get; private set;}

	public ushort PickupSpawnedID {get; private set;}
	public ushort PickupRemovedID {get; private set;}

	public ServerPTD[] GetPacketData() => new ServerPTD[]{
		new ServerPTD(PacketNames.PLAYER_JOINED),
		new ServerPTD(PacketNames.PLAYER_REMOVED),
		new ServerPTD(PacketNames.PLAYER_TRANSFORM_UPDATE),
		new ServerPTD(PacketNames.PLAYER_SCORE),
		new ServerPTD(PacketNames.PLAYER_MOVE, PlayerMoveHandler),

		new ServerPTD(PacketNames.PICKUP_SPAWNED),
		new ServerPTD(PacketNames.PICKUP_REMOVED),
	};

	private void OnServerInitalization(){
		PacketRegistry<ServerPacketHandler> r = _server.Registry;
		PlayerJoinedID = r.GetID(PacketNames.PLAYER_JOINED);
		PlayerRemovedID = r.GetID(PacketNames.PLAYER_REMOVED);
		PlayerTransformUpdateID = r.GetID(PacketNames.PLAYER_TRANSFORM_UPDATE);
		PlayerScoreID = r.GetID(PacketNames.PLAYER_SCORE);

		PickupSpawnedID = r.GetID(PacketNames.PICKUP_SPAWNED);
		PickupRemovedID = r.GetID(PacketNames.PICKUP_REMOVED);
	}

	private void OnEnable() {
		_server.Initalized += OnServerInitalization;
	}
	private void OnDisable() {
		_server.Initalized -= OnServerInitalization;
	}

	private void PlayerMoveHandler(byte senderIdx, PacketReader packetReader){
		PlayerMoved?.Invoke(senderIdx, packetReader.NextVector2());
	}
}