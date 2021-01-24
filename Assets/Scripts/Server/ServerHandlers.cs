using NetLib;

public static class ServerHandlers{
	public static void AddAllToHandler(){
		//Network.Server.PacketHandlers.Add()
	}

	public static void HandleMovement(byte clientIdx, PacketReader pr){
		pr.NextVector3();
	}

}