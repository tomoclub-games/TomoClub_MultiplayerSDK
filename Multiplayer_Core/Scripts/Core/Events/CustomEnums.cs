namespace TomoClub.Core
{
	public enum PlayerType { Master_Spectator, Master_Player, Spectator, Player }
	public enum RoomState { Null, Unassigned_Arenas, Assigned_Arenas };
	public enum TeamName { Red, Blue, None };

	public enum BuildType { Classroom_Mod, Classroom_Player, Classroom_Common, Standard }

	public enum GameStates { Null, MainMenu, RoomLobby, InGame }

	public enum GameType { InterArena, IntraArena };

	public enum AuthenticationType { Device_Based, Name_Based }

	public enum HostingProvider { Simmer, Itch };
}
