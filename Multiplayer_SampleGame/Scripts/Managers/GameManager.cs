using UnityEngine;
using Photon.Pun;
using System.IO;
using TomoClub.Core;

namespace TomoClub.SampleGame
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance;

		public Transform[] teamASpawnPoint;
		public Transform[] teamBSpawnPoint;

		[HideInInspector]public Camera playerCamera;

		private void Awake()
		{
			if (Instance == null) Instance = this;
		}


		private void Start() => SpawnPlayer(LocalPlayer.Instance.teamNo);

		private void SpawnPlayer(int teamNo)
		{
			bool canPlayGame = LocalPlayer.Instance.inGamePlayerType == PlayerType.Master_Player || LocalPlayer.Instance.inGamePlayerType == PlayerType.Player;
			if (canPlayGame)
			{
				var playerManager = PhotonNetwork.Instantiate(Path.Combine(Identifiers.PhotonPrefabPath, Identifiers.PlayerManager), Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
				playerManager.InstantiatePlayerManager(teamNo);
			}

		}

	} 
}




