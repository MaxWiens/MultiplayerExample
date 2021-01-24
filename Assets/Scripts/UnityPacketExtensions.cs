using UnityEngine;
using NetLib;

public static class UnityPacketExtensions {
	public static void Write(this PacketBuilder pb, Vector2 data){
		pb.Write(data.x);
		pb.Write(data.y);
	}
	public static void Write(this PacketBuilder pb, Vector3 data){
		pb.Write(data.x);
		pb.Write(data.y);
		pb.Write(data.z);
	}

	public static Vector3 NextVector3(this PacketReader pr)
		=> new Vector3(pr.NextFloat(), pr.NextFloat(), pr.NextFloat());

	public static Vector2 NextVector2(this PacketReader pr)
		=> new Vector2(pr.NextFloat(), pr.NextFloat());
}