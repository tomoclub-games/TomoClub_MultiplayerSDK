using Photon.Pun;
using UnityEngine;

namespace TomoClub.SampleGame
{
	public class ExampleRemoteProcedureCalls : MonoBehaviourPun
	{
		public static ExampleRemoteProcedureCalls Instance;

		[Header("Managers")]
		[SerializeField] ExampleArenaManager arenaManager;
		[SerializeField] LeaderboardManager leaderboardManager;

		private void Awake() => GlobalInstance();

		private void GlobalInstance()
		{
			if (Instance == null) Instance = this;
			else gameObject.SetActive(false);
		}

		public void UpdateArenaPauseStateOnNetwork(int arenaNo, int updateState)
		{
			this.photonView.RPC(nameof(UpdateArenaPauseStatusOnClient), RpcTarget.All, arenaNo, updateState);
		}

		public void UpdateArenaTimerOnNetwork(int arenaNo, int currentTime)
		{
			this.photonView.RPC(nameof(UpdateArenaTimerOnClient), RpcTarget.All, arenaNo, currentTime);
		}

		public void UpdateArenaTimerCompletedOnNetwork(int arenaNo)
		{
			this.photonView.RPC(nameof(UpdateArenaTimerCompletedOnClient), RpcTarget.All, arenaNo);
		}

		[PunRPC]
		private void UpdateArenaTimerCompletedOnClient(int arenaNo)
		{
			leaderboardManager.CalculateWinnerOnTimerEnd(arenaNo);
			arenaManager.UpdateArenaToCompletedState(arenaNo);
		}

		[PunRPC]
		private void UpdateArenaTimerOnClient(int arenaNo, int currentTime)
		{
			arenaManager.UpdateArenaTimerOnClient(arenaNo, currentTime);
		}

		[PunRPC]
		private void UpdateArenaPauseStatusOnClient(int arenaNo, int updateState)
		{
			arenaManager.UpdatePlayerPauseOnClient(arenaNo, updateState);
		}

	} 
}
