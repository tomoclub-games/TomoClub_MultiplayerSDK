using Photon.Pun;
using UnityEngine;
using TomoClub.Core;

namespace TomoClub.Arenas
{
	public enum ArenaState { Paused, Running, Completed }

	public class BaseArenaManager<T> : MonoBehaviour where T: BaseArena
	{
		[Header("Arenas")]
		[SerializeField] Vector3[] arenaPositions;
		[SerializeField] GameObject arenaPrefab;

		[Header("Notes")]
		[SerializeField] GameObject Panel_Notes;
		[SerializeField] GameObject Button_Notes;

		[Header("Play/Pause UI")]
		[SerializeField] GameObject Panel_Pause;
		[SerializeField] GameObject PlayPauseButtonHolder;
		[SerializeField] GameObject PlayPauseDropdown;
		[SerializeField] GameObject Popup_PlayPauseArenas;
		[SerializeField] ArenaTogglePauseButton allArenasTogglePauseButton;
		[SerializeField] Sprite[] playPauseSprites;
		[SerializeField] ArenaTogglePauseButton[] arenaTogglePauseButtons;

		[Header("Spectator Tools")]
		[SerializeField] GameObject Panel_Spectator;
		[SerializeField] GameObject Spectator_UI;
		[SerializeField] GameObject Panel_WaitingForResultCompilation;
		[SerializeField] SpectatorArenaButtons[] Buttons_Spectator;

		[Header("End Panel")]
		[SerializeField] GameObject Panel_EndPanel;
		[SerializeField] GameObject Panel_EndMaster;
		[SerializeField] GameObject Panel_EndNormal;

		protected T[] arenas;
		protected int currentArenaNo = -1;
		protected int sessionGameTime;

		protected virtual void Awake()
		{
			Init();
			GenerateArenas();
			StartArenas();
		}

		protected virtual void OnEnable()
		{
			UtilEvents.SetAndStartTimer += SwitchTimerControl;
		}

		protected virtual void OnDisable()
		{
			UtilEvents.SetAndStartTimer -= SwitchTimerControl;
		}


		protected virtual void Init()
		{
			arenas = new T[MultiplayerManager.Instance.occupiedArenas];

			//Pause Panel Init
			for (int i = 0; i < arenaTogglePauseButtons.Length; i++)
			{
				arenaTogglePauseButtons[i].SetSprite(playPauseSprites[0]);
				arenaTogglePauseButtons[i].SetButtonState(i < MultiplayerManager.Instance.occupiedArenas);
				arenaTogglePauseButtons[i].SetButtonHolder(i < MultiplayerManager.Instance.occupiedArenas);
			}

			allArenasTogglePauseButton.SetSprite(playPauseSprites[0]);

			Panel_Pause.SetActive(false);
			Popup_PlayPauseArenas.SetActive(false);
			PlayPauseDropdown.SetActive(MultiplayerManager.Instance.occupiedArenas > 1);
			PlayPauseButtonHolder.SetActive(false);

			//Spectator Panel Init
			for (int i = 0; i < Buttons_Spectator.Length; i++)
			{
				Buttons_Spectator[i].SetSpecButtonColor(Color.red);
				Buttons_Spectator[i].SetButtonHolder(i < MultiplayerManager.Instance.occupiedArenas);
			}

			Panel_WaitingForResultCompilation.SetActive(false);
			Spectator_UI.SetActive(false);
			Panel_Spectator.SetActive(false);

			//Notes Panel
			Panel_Notes.SetActive(false);
			Button_Notes.SetActive(LocalPlayer.Instance.isClassroomModerator);

			//End Panel
			Panel_EndMaster.SetActive(false);
			Panel_EndNormal.SetActive(false);

			//Game Settings
			sessionGameTime = MultiplayerManager.Instance.gameSessionTime;
		}

		protected virtual void GenerateArenas()
		{
			//New Arenas Object
			var arenaParentObject = new GameObject("Arenas");
			arenaParentObject.transform.position = Vector3.zero;
			arenaParentObject.transform.rotation = Quaternion.identity;

			for (int i = 0; i < MultiplayerManager.Instance.occupiedArenas; i++)
			{
				var arenaObject = Instantiate(arenaPrefab, arenaPositions[i], Quaternion.identity, arenaParentObject.transform);
				arenaObject.gameObject.name = "Arena " + (i + 1);
				arenas[i] = arenaObject.GetComponent<T>();
				arenas[i].InitializeArena(i + 1, sessionGameTime);
			}
		}

		protected virtual void StartArenas()
		{
			for (int i = 0; i < MultiplayerManager.Instance.occupiedArenas; i++)
			{
				arenas[i].StartArena();
			}

			UpdatePlayerTypeState(LocalPlayer.Instance.defaultPlayerType);

			UtilEvents.ShowToastMessage("Game Started!");
		}

		public virtual void UpdateNotesMenu(bool status) => Panel_Notes.SetActive(status);

		public virtual void TogglePausePlayDropdown() => Popup_PlayPauseArenas.SetActive(!Popup_PlayPauseArenas.activeSelf);

		public virtual void InGameSettingsButton()
		{
			if (PersistantUI.Instance != null)
				PersistantUI.Instance.ShowSettingsPopup();
		}

		#region Arena To Look At
		//Set Current Arena To look at
		public void SetCurrentArenaToLookAt(int arenaNo)
		{
			if (arenaNo < 1 || arenaNo > arenas.Length)
			{
				Debug.LogError($"Can only set arenaNo between 0 and {arenas.Length + 1}");
				return;
			}

			if (currentArenaNo == arenaNo) return;


			if (currentArenaNo > 0)
			{
				Buttons_Spectator[currentArenaNo - 1].SetSpecButtonColor(Color.red);
				arenas[currentArenaNo - 1].DeassignArenaCamera();
			}

			Buttons_Spectator[arenaNo - 1].SetSpecButtonColor(Color.green);
			arenas[arenaNo - 1].AssignArenaCamera();
			currentArenaNo = arenaNo;

		}

		private int ActiveArenaToLookAt()
		{
			for (int i = 0; i < arenas.Length; i++)
			{
				int arenaNo = i + 1;
				if (arenas[i].ArenaState != ArenaState.Completed) return arenaNo;
			}

			return 1;
		}

		#endregion

		#region Arena Timers

		public virtual void UpdateArenaTimerOnClient(int arenaNo, int currentTime)
		{
			arenas[arenaNo - 1].UpdateTimerData(currentTime);
		}

		//Only master client gets this option
		public virtual void OnPlayPauseArena(int arenaNo)
		{
			ArenaState currentArenaState = arenas[arenaNo - 1].ArenaState;
			if (currentArenaState == ArenaState.Completed) return;

			//Toggle State of Arena
			int updatedState = 1 - (int)currentArenaState;
			//Update the arena timer on the master client
			UpdateTimerState(arenaNo, (ArenaState)updatedState);

			//Send Updated ArenaState to all players
			UpdateArenaPauseStateOnNetwork(arenaNo, (ArenaState)updatedState);
		}

		protected virtual void UpdateArenaPauseStateOnNetwork(int arenaNo, ArenaState updatedState)
		{
			//
		}

		public virtual void OnPlayPauseAllArenas()
		{
			int updatedState = AllArenasPaused() ? 1 : 0;
			int currentArenaNo;
			for (int i = 0; i < arenas.Length; i++)
			{
				currentArenaNo = i + 1;
				if (arenas[i].ArenaState == ArenaState.Completed) continue;
				UpdateTimerState(currentArenaNo, (ArenaState)updatedState);

				//Send Updated ArenaState to all players
				UpdateArenaPauseStateOnNetwork(currentArenaNo, (ArenaState)updatedState);

			}

		}

		public void UpdatePlayerPauseOnClient(int arenaNo, int updatedState)
		{
			ArenaState stateToChangeTo = (ArenaState)updatedState;
			arenas[arenaNo - 1].UpdateArenaStateData(stateToChangeTo);

			//Update Images Here (// State 0-> pause state -> sprite should be play sprite, State 1-> play state -> sprite should be pause sprite)
			Sprite allArenasPauseSprite = AllArenasPaused() ? playPauseSprites[1] : playPauseSprites[0];
			allArenasTogglePauseButton.SetSprite(allArenasPauseSprite);
			arenaTogglePauseButtons[arenaNo - 1].SetSprite(playPauseSprites[1 - updatedState]);

			//If its your arenaNo then pause/play the game
			if (arenaNo == LocalPlayer.Instance.arenaNo) UpdatePlayerArenaState((ArenaState)updatedState);
		}

		private bool AllArenasPaused()
		{
			for (int i = 0; i < arenas.Length; i++)
			{
				if (arenas[i].ArenaState == ArenaState.Running) return false;
			}
			return true;
		}

		//Switches timer control to new master client
		protected virtual void SwitchTimerControl()
		{
			foreach (var arena in arenas) arena.ContinueTimer();
		}

		#endregion


		#region On Arena Complete
		public virtual void UpdateArenaToCompletedState(int arenaNo)
		{
			arenas[arenaNo - 1].UpdateArenaStateData(ArenaState.Completed);
			arenaTogglePauseButtons[arenaNo - 1].SetButtonState(false);
			arenas[arenaNo - 1].StopTimer();

			if (arenaNo == LocalPlayer.Instance.arenaNo) UpdatePlayerArenaState(ArenaState.Completed);

			if (AllArenasCompleted())
			{
				Panel_WaitingForResultCompilation.SetActive(false);
				UpdateLeaderboardBasedOnPlayerTypeState(LocalPlayer.Instance.inGamePlayerType);
			}
			else
			{
				if (arenaNo == LocalPlayer.Instance.arenaNo) Panel_WaitingForResultCompilation.SetActive(true);
			}

		}

		private bool AllArenasCompleted()
		{
			for (int i = 0; i < arenas.Length; i++)
			{
				if (arenas[i].ArenaState != ArenaState.Completed) return false;
			}

			return true;
		}

		public virtual void SpectateGame()
		{
			if (LocalPlayer.Instance.inGamePlayerType == PlayerType.Master_Player) UpdatePlayerTypeState(PlayerType.Master_Spectator);
			else if (LocalPlayer.Instance.inGamePlayerType == PlayerType.Player) UpdatePlayerTypeState(PlayerType.Spectator);
		}

		#endregion

		#region Change Arena State 

		protected virtual void UpdateTimerState(int arenaNo, ArenaState arenaState)
		{
			switch (arenaState)
			{
				case ArenaState.Paused:
					arenas[arenaNo - 1].PauseTimer();
					break;
				case ArenaState.Running:
					arenas[arenaNo - 1].PlayTimer();
					break;
				case ArenaState.Completed:
					break;
				default:
					Debug.LogError("Something went really wrong, arenaState can never be null");
					break;
			}
		}

		protected virtual void UpdatePlayerArenaState(ArenaState arenaState)
		{
			switch (arenaState)
			{
				case ArenaState.Paused:
					Panel_Pause.SetActive(true);
					GameEvents.OnPauseGame?.Invoke();
					break;
				case ArenaState.Running:
					Panel_Pause.SetActive(false);
					GameEvents.OnPlayGame?.Invoke();
					break;
				case ArenaState.Completed:
					Panel_Spectator.SetActive(true);
					GameEvents.OnGameSessionEnded?.Invoke();
					break;
				default:
					Debug.LogError("Something went really wrong, arenaState can never be null");
					break;
			}
		}

		protected virtual void UpdatePlayerTypeState(PlayerType changedState)
		{
			LocalPlayer.Instance.inGamePlayerType = changedState;
			switch (changedState)
			{
				case PlayerType.Master_Spectator:
					Panel_Spectator.SetActive(true);
					Panel_WaitingForResultCompilation.SetActive(false);
					Spectator_UI.SetActive(MultiplayerManager.Instance.occupiedArenas > 1);
					PlayPauseButtonHolder.SetActive(true);
					SetCurrentArenaToLookAt(ActiveArenaToLookAt());
					break;
				case PlayerType.Master_Player:
					PlayPauseButtonHolder.SetActive(true);
					SetCurrentArenaToLookAt(LocalPlayer.Instance.arenaNo);
					break;
				case PlayerType.Spectator:
					Panel_Spectator.SetActive(true);
					Spectator_UI.SetActive(MultiplayerManager.Instance.occupiedArenas > 1);
					SetCurrentArenaToLookAt(ActiveArenaToLookAt());
					Panel_WaitingForResultCompilation.SetActive(false);
					break;
				case PlayerType.Player:
					SetCurrentArenaToLookAt(LocalPlayer.Instance.arenaNo);
					break;
				default:
					Debug.LogError("Not possible, something went terribly wrong");
					break;
			}
		}

		protected virtual void UpdateLeaderboardBasedOnPlayerTypeState(PlayerType playerType)
		{
			switch (playerType)
			{
				case PlayerType.Master_Spectator:
					Panel_EndMaster.SetActive(true);
					break;
				case PlayerType.Master_Player:
					Panel_EndMaster.SetActive(true);
					break;
				case PlayerType.Spectator:
					Panel_EndNormal.SetActive(true);
					break;
				case PlayerType.Player:
					Panel_EndNormal.SetActive(true);
					break;
				default:
					break;
			}

			Panel_EndPanel.SetActive(true);
		}

		#endregion

		public virtual void ExitToLobby()
		{
			MultiplayerManager.Instance.ResetRoom();
		}

		public virtual void RestartRound()
		{
			PhotonNetwork.LoadLevel(Constants.TempScene);
		}

	} 
}
