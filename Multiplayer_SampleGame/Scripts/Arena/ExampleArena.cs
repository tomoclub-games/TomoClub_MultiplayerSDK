using TomoClub.Arenas;
using TomoClub.Core;

namespace TomoClub.SampleGame
{
	/// <summary>
	/// This is an extension to base arena to override any of its functionality
	/// </summary>
	public class ExampleArena : BaseArena
	{
		private ArenaScore arenaScore;
		public ArenaScore ArenaScore => arenaScore;

		private ModeratorCameraMovement moderatorCameraMovement;
		public ModeratorCameraMovement ModeratorCameraMovement => moderatorCameraMovement;

		protected override void Awake()
		{
			base.Awake();
			arenaScore = GetComponent<ArenaScore>();
			moderatorCameraMovement = GetComponentInChildren<ModeratorCameraMovement>();

		}
		protected override void OnTimerUpdate(int currentTime)
		{
			ExampleRemoteProcedureCalls.Instance.UpdateArenaTimerOnNetwork(arenaNo, currentTime);
		}

		protected override void OnTimerCompleted()
		{
			ExampleRemoteProcedureCalls.Instance.UpdateArenaTimerCompletedOnNetwork(arenaNo);
		}

		public override void AssignArenaCamera()
		{
			base.AssignArenaCamera();
			bool canSpectate = LocalPlayer.Instance.inGamePlayerType == PlayerType.Master_Spectator || LocalPlayer.Instance.inGamePlayerType == PlayerType.Spectator;
			moderatorCameraMovement.UpdateCameraFollow(true && canSpectate);
		}

		public override void DeassignArenaCamera()
		{
			base.DeassignArenaCamera();
			bool canSpectate = LocalPlayer.Instance.inGamePlayerType == PlayerType.Master_Spectator || LocalPlayer.Instance.inGamePlayerType == PlayerType.Spectator;
			moderatorCameraMovement.UpdateCameraFollow(true && canSpectate);
			moderatorCameraMovement.UpdateCameraFollow(false);
		}

	} 
}

