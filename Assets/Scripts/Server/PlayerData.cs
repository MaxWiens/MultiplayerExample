using System;
using UnityEngine;

public class PlayerData : EntityData {
	public readonly DateTime JoinTime;

	public PlayerData(int id, DateTime joinTime) : base(id){
		JoinTime = joinTime;
	}
}