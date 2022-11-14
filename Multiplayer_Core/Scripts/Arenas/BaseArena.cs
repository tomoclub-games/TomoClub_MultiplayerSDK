using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

using TomoClub.Core;
using TomoClub.Util;

namespace TomoClub.Arenas
{

	public class BaseArena : MonoBehaviour
	{
		[Header("Camera")]
		[SerializeField] protected Camera arenaCamera;

		[Header("Canvas")]
		[SerializeField] protected bool canvasOverlay = false;
		[SerializeField] protected Canvas arenaCanvas;

		[Header("Arena UI")]
		[SerializeField] protected TextMeshProUGUI arenaTimerText;
		[SerializeField] protected TextMeshProUGUI arenaDataText;
		[SerializeField] private TextMeshProUGUI arenaPlayersText;

		protected int arenaNo;
		protected List<Player> arenaPlayers => MultiplayerManager.Instance.arenaLists[arenaNo - 1].arenaPlayers;

		protected ArenaState arenaState = ArenaState.Paused;
		public ArenaState ArenaState => arenaState;

		protected int arenaTimeLeft;
		protected TimerDown arenaTimer = new TimerDown();

		protected virtual void Awake()
		{
			//Empty but Arena can use 
		}

		protected virtual void Start()
		{
			//Empty but Arena can use 
		}

		protected virtual void OnEnable()
		{
			arenaTimer.TimerCompleted += OnTimerCompleted;
			arenaTimer.TimerUpdatePerSecond += OnTimerUpdate;
		}

		protected virtual void OnDisable()
		{
			arenaTimer.TimerCompleted -= OnTimerCompleted;
			arenaTimer.TimerUpdatePerSecond -= OnTimerUpdate;
		}

		/// <summary>
		/// This methods run every time the arena updates
		/// </summary>
		protected virtual void OnTimerUpdate(int currentTime)
		{

		}

		/// <summary>
		/// This method run every time the arena timer completes
		/// </summary>
		protected virtual void OnTimerCompleted()
		{

		}

		protected virtual void Update()
		{
			// Run this arena's timer only on the masterClient
			if (LocalPlayer.Instance.isMasterClient) arenaTimer.UpdateTimer();
		}

		public virtual void InitializeArena(int arenaNo, int sessionGameTime)
		{
			//Set Canvas to current Camera
			arenaCamera.depth = -1;
			if (canvasOverlay)
			{
				arenaCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
				arenaCanvas.gameObject.SetActive(false);
			}
			else
			{
				arenaCanvas.renderMode = RenderMode.ScreenSpaceCamera;
				arenaCanvas.worldCamera = arenaCamera;

			}


			//Set This Arena's data
			this.arenaNo = arenaNo;
			arenaDataText.text = $"Arena {arenaNo}: {arenaState}";
			arenaPlayersText.text = "";
			for (int i = 0; i < arenaPlayers.Count; i++)
			{
				arenaPlayersText.text += string.IsNullOrEmpty(arenaPlayersText.text) ? arenaPlayers[i].NickName : $", {arenaPlayers[i].NickName}";
			}

			//Set Arena Timer
			UpdateTimerData(sessionGameTime);
			arenaTimer.SetTimer(sessionGameTime);

		}

		public virtual void UpdateTimerData(int currentTime)
		{
			arenaTimeLeft = currentTime;
			arenaTimerText.text = Utilities.CovertTimeToString(arenaTimeLeft);
		}

		/// <summary>
		/// Start this arena
		/// </summary>
		public virtual void StartArena()
		{
			if (LocalPlayer.Instance.isMasterClient) arenaTimer.StartTimer();
			UpdateArenaStateData(ArenaState.Running);
		}

		public virtual void PauseTimer() => arenaTimer.PauseTimer();

		public virtual void PlayTimer() => arenaTimer.PlayTimer();

		public virtual void StopTimer()
		{
			if (arenaTimer.IsRunning()) arenaTimer.PauseTimer();
		}

		public virtual void ContinueTimer() => arenaTimer.SetAndStartTimer(arenaTimeLeft);

		/// <summary>
		/// Assign the player this arena Camera
		/// </summary>
		public virtual void AssignArenaCamera()
		{
			arenaCamera.depth = 1;
			if (canvasOverlay) arenaCanvas.gameObject.SetActive(true);
		}


		/// <summary>
		/// Deassign this arena camera for this player
		/// </summary>
		public virtual void DeassignArenaCamera()
		{
			arenaCamera.depth = -1;
			if (canvasOverlay) arenaCanvas.gameObject.SetActive(false);
		}

		public virtual void UpdateArenaStateData(ArenaState arenaState)
		{
			this.arenaState = arenaState;
			arenaDataText.text = $"Arena {arenaNo}: {arenaState}";
		}


	} 
}
