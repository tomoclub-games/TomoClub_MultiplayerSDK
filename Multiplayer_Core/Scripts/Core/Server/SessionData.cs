using Photon.Realtime;

namespace TomoClub.Core
{
	public static class SessionData
	{
		//Game Details
		public static string GameName { get; set; }
		public static bool connectionEstablished = false;
		public static BuildType BuildType;
		public static AuthenticationType AuthenticationType;
		public static bool TestMode;

		public static GameType GameType;
		public static TypedLobby gameLobby;

		//Publishing Details
		public static HostingProvider hostingProvider;
		public static string simmerLink;
		public static string itchLink;
		public static bool showSplashScreen;
		public static float splashScreenTTL;

		//Game States
		public static GameStates previousGameState = GameStates.Null;
		public static GameStates currentGameState = GameStates.MainMenu;


	} 
}






