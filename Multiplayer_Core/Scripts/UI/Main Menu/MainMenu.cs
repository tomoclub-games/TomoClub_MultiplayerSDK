using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Random = System.Random;
using System.Linq;
using TomoClub.Core;

	public class MainMenu : MonoBehaviour
	{
		[Header("Main Menu - Updations")]
		[SerializeField] TextMeshProUGUI gameNameText;
		[SerializeField] TextMeshProUGUI dateText;
		[SerializeField] TextMeshProUGUI playerNameText;

		[Header("Dual Build")]
		[SerializeField] GameObject Popup_DualBuild;

		[Header("Player-Name Input")]
		[SerializeField] GameObject Popup_Username;
		[SerializeField] TMP_InputField Username_InputField;

		[Header("Create_Room")]
		[SerializeField] Button Button_CreateRoom;
		[SerializeField] GameObject Popup_CreateRoom;

		[Header("Join_Room")]
		[Tooltip("Sets the type of join room UI: list of available rooms vs entering the room id")]
		[SerializeField] private bool showRoomList;
		[SerializeField] Button Button_JoinRoom;
		[SerializeField] GameObject Popup_JoinRoom_List;
		[SerializeField] GameObject Popup_JoinRoom_Input;

		[Header("Audio")]
		[SerializeField] private AudioClip errorAudioClip;

		private static Random random;

		private void Start()
		{
			MainMenu_Init();
			UpdateMainMenuData();

			if (SessionData.previousGameState == GameStates.MainMenu)
			{
				if (SessionData.BuildType == BuildType.Classroom_Common) MainMenu_PickBuild(true);
				else LoginToGame();
			}
			else
			{
				LocalPlayer.Instance.ResetLocalPlayerData();
				LocalPlayer.Instance.StartKickOutTimer();
			}


			ServerMesseges.OnConnectedToPhoton += MakeLobbyButtonsInteractable;
			UtilEvents.OnKickOutOver += MakeLobbyButtonsInteractable;
		}

		private void OnDestroy()
		{
			ServerMesseges.OnConnectedToPhoton -= MakeLobbyButtonsInteractable;
			UtilEvents.OnKickOutOver -= MakeLobbyButtonsInteractable;

		}

		private void MakeLobbyButtonsInteractable()
		{
			Button_CreateRoom.interactable = !LocalPlayer.Instance.timedOut;
			Button_JoinRoom.interactable = !LocalPlayer.Instance.timedOut;
		}


		//Set username if it exists else popup username panel
		private void LoginToGame()
		{
			if (string.IsNullOrEmpty(LocalPlayer.Instance.playerName)) Popup_Username.SetActive(true);
			else
			{
				OnUpdateUsername();
				ServerMesseges.EstablishConnectionToServer?.Invoke();
			}

		}

		//Initial main menu data updates { date, game name, player name }
		private void UpdateMainMenuData()
		{
			//Update Game Name
			gameNameText.text = SessionData.GameName;

			//Update Current Date
			DateTime currentDate = DateTime.Today;
			dateText.text = currentDate.ToShortDateString();

			//Update User Name        
			playerNameText.text = $"Hey {LocalPlayer.Instance.playerName} !";

		}

		//Initializatoins for the main menu
		private void MainMenu_Init()
		{
			Button_CreateRoom.gameObject.SetActive(!LocalPlayer.Instance.isClassroomPlayer);
			Button_JoinRoom.gameObject.SetActive(!LocalPlayer.Instance.isClassroomModerator);

			Button_CreateRoom.interactable = false;
			Button_JoinRoom.interactable = false;


		}

		//On click confirm username button
		public void ConfirmUsername()
		{
			if (string.IsNullOrEmpty(Username_InputField.text))
			{
				SoundMessages.PlaySFX?.Invoke(errorAudioClip);
			}
			else
			{
				LocalPlayer.Instance.playerName = Username_InputField.text;
				OnUpdateUsername();
				//Establish Connection 
				Popup_Username.SetActive(false);
				ServerMesseges.EstablishConnectionToServer?.Invoke();
			}

		}

		private void OnUpdateUsername()
		{
			//Authenticate Player
			AuthenticatePlayer();
			//Set Player Name UI 
			playerNameText.text = $"Hey { LocalPlayer.Instance.playerName } !";
			PhotonNetwork.NickName = LocalPlayer.Instance.playerName;
		}

		private void AuthenticatePlayer()
		{
			if (LocalPlayer.Instance.player.UserId == null) return;

			switch (SessionData.AuthenticationType)
			{
				case AuthenticationType.Device_Based:
					if (string.IsNullOrEmpty(LocalPlayer.Instance.userID))
						LocalPlayer.Instance.userID = GenerateRandomUniqueUserKey();
					PhotonNetwork.AuthValues = new AuthenticationValues(LocalPlayer.Instance.userID);
					break;
				case AuthenticationType.Name_Based:
					PhotonNetwork.AuthValues = new AuthenticationValues(LocalPlayer.Instance.playerName);
					break;
				default:
					Debug.LogError("Not possible, Something went terribly wrong");
					break;
			}
		}

		private string GenerateRandomUniqueUserKey()
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			int randomLength = UnityEngine.Random.Range(7, 10);
			string key = new string(Enumerable.Repeat(chars, randomLength)
				.Select(s => s[random.Next(s.Length)]).ToArray());

			return key;
		}


		//On click open/close create room popup
		public void MainMenu_CreateRoom(bool action)
		{
			if (!SessionData.connectionEstablished || LocalPlayer.Instance.timedOut)
			{
				//Error sound 
				return;
			}
			Popup_CreateRoom.SetActive(action);
		}

		//On click open/close join room popup
		public void MainMenu_JoinRoom(bool action)
		{
			if (!SessionData.connectionEstablished || LocalPlayer.Instance.timedOut)
			{
				//Error sound 
				return;
			}

			Popup_JoinRoom_List.SetActive(action && showRoomList);
			Popup_JoinRoom_Input.SetActive(action && !showRoomList);
		}

		public void RedirectToWebsite()
		{
			Application.OpenURL("https://tomoclub.org/");
		}

		public void ToggleUsernamePopup() => Popup_Username.SetActive(true);


		private void MainMenu_PickBuild(bool status)
		{
			Popup_DualBuild.SetActive(status);
		}

		public void Select_ModBuild()
		{
			MainMenu_PickBuild(false);
			LoginToGame();
			SessionData.BuildType = BuildType.Classroom_Mod;
			LocalPlayer.Instance.arenaNo = -1;
			Button_JoinRoom.gameObject.SetActive(false);
		}

		public void Select_PlayerBuild()
		{
			MainMenu_PickBuild(false);
			LoginToGame();
			SessionData.BuildType = BuildType.Classroom_Player;
			LocalPlayer.Instance.arenaNo = -1;
			Button_CreateRoom.gameObject.SetActive(false);
		}
	} 

