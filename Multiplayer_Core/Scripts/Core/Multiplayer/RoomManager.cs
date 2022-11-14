using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace TomoClub.Core
{
    //Handles Room Creation and Joining
    public class RoomManager : MonoBehaviour
    {
        [Header("Create Room - Base UI Data")]
        [Tooltip("Default players value to show on opening create room")]
        [SerializeField] int defaultTotalPlayers = 4;
        [Tooltip("Default arena value to show on opening create room")]
        [SerializeField] int defaultArenas = 2;

        [Header("Create Room - UI Updations")]
        [SerializeField] TextMeshProUGUI totalPlayersText;
        [SerializeField] TextMeshProUGUI arenaPlayersText;
        [SerializeField] TextMeshProUGUI defaultPlayerTypeText;
        [SerializeField] TextMeshProUGUI createErrorText;

        [Header("Join Room - UI Updations")]
        [SerializeField] GameObject noRoomsAvailable;
        [SerializeField] GameObject[] joinRoomObjects;
        [SerializeField] TextMeshProUGUI[] joinErrorText;
        [SerializeField] TMP_InputField joinRoomId;

        [Header("Audio")]
        [SerializeField] AudioClip errorAudioClip;

        private TextMeshProUGUI[] joinRoomObjectTexts;
        private string[] currentRoomNames;

        private bool canCreateRoom = true;
        private bool canJoinRoom = true;

        private Vector2Int totalPlayersRange;
        private Vector2Int totalArenaRange;
        private Vector2Int perArenaPlayersRange;

        private bool goToLobby = false;

        private readonly string roomNoPrefix = "Room No: ";

        private void Start()
        {
            UpdateCreateRoomData();
            JoinRoom_Init();
        }

        private void OnEnable()
        {
            ServerMesseges.OnJoinRoomSuccessful += GoToLobbyMenu;
            ServerMesseges.OnCreateRoomFailed += UpdateCreateRoomUI;
            ServerMesseges.OnJoinRoomFailed += UpdateJoinRoomUI;
            ServerMesseges.OnRoomListUpdated += UpdateRoomListUI;

            ServerMesseges.OnPlayerPropertiesUpdated += LoadLobby;

            LocalPlayerMessages.PlayerTypeUpdated += OnPlayerTypeUpdated;
        }

        private void OnDisable()
        {
            ServerMesseges.OnJoinRoomSuccessful -= GoToLobbyMenu;
            ServerMesseges.OnCreateRoomFailed -= UpdateCreateRoomUI;
            ServerMesseges.OnJoinRoomFailed -= UpdateJoinRoomUI;
            ServerMesseges.OnRoomListUpdated -= UpdateRoomListUI;

            ServerMesseges.OnPlayerPropertiesUpdated -= LoadLobby;

            LocalPlayerMessages.PlayerTypeUpdated -= OnPlayerTypeUpdated;
        }

        //Initial create room setup
        private void UpdateCreateRoomData()
        {
            totalArenaRange = MultiplayerManager.Instance.arenasPerRoomRange;
            totalPlayersRange = MultiplayerManager.Instance.playersPerRoomRange;
            perArenaPlayersRange = MultiplayerManager.Instance.playersPerArenaRange;

			totalPlayersText.text = defaultTotalPlayers.ToString();
            arenaPlayersText.text = defaultArenas.ToString();

            createErrorText.text = "";

        }

        private void OnPlayerTypeUpdated(PlayerType playerType)
		{
            totalPlayersRange.y = playerType == PlayerType.Master_Spectator ? totalPlayersRange.y - 1 : totalPlayersRange.y + 1;
            defaultPlayerTypeText.text = playerType == PlayerType.Master_Spectator ? "Spectator" : "Player";
        }

        private void JoinRoom_Init()
        {
            joinRoomObjectTexts = new TextMeshProUGUI[joinRoomObjects.Length];
            currentRoomNames = new string[joinRoomObjects.Length];
        }

        //Initial join room setup
        private void UpdateJoinRoomData()
        {
            noRoomsAvailable.SetActive(true);

            for (int i = 0; i < joinRoomObjects.Length; i++)
            {
                joinRoomObjects[i].SetActive(false);
                joinRoomObjectTexts[i] = joinRoomObjects[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            }

            foreach (var errorText in joinErrorText)
            {
                errorText.text = "";
            }
            
        }

        //Update message to show on failing to create room
        private void UpdateCreateRoomUI(string message)
        {
            createErrorText.text = message;
            canCreateRoom = true;
        }

        //Update message to show on failing to join room
        private void UpdateJoinRoomUI(string message)
        {
            foreach (var errorText in joinErrorText)
            {
                errorText.text = message;
            }

            canJoinRoom = true;
        }

        #region CREATE ROOM PANEL BUTTON FUNCTIONS

        public void DecrementTotalPlayers()
        {
            createErrorText.text = "";

            if (defaultTotalPlayers - 1 < totalPlayersRange.x)
            {
                createErrorText.text = $"Can't have less than {totalPlayersRange.x} players in a room";
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return;
            }

            if(defaultTotalPlayers - 1 < defaultArenas * perArenaPlayersRange.x)
            {
                createErrorText.text = $"Minimum {perArenaPlayersRange.x} players per arena, try decreasing arenas!";
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return;
            }

            defaultTotalPlayers--;
            totalPlayersText.text = defaultTotalPlayers.ToString();
        }

        public void IncrementTotalPlayers()
        {
            createErrorText.text = "";

            if (defaultTotalPlayers + 1 > totalPlayersRange.y )
            {
                createErrorText.text = $"Can't have more than {totalPlayersRange.y} players in a room";
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return; 
            }

            if(defaultTotalPlayers + 1 > defaultArenas * perArenaPlayersRange.y)
            {
                createErrorText.text = $"Maximum {perArenaPlayersRange.y} players per arena, try increasing arenas!";
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return;
            }


            defaultTotalPlayers++;
            totalPlayersText.text = defaultTotalPlayers.ToString();
        }

        public void DecrementArenaPlayers()
        {
            createErrorText.text = "";

            if (defaultArenas - 1 < totalArenaRange.x)
            {
                createErrorText.text = $"Can't have less than {totalArenaRange.x} arena";
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return;
            }

            if(defaultArenas - 1 < Mathf.CeilToInt((float)defaultTotalPlayers / perArenaPlayersRange.y))
            {
                createErrorText.text = $"Max {perArenaPlayersRange.y} players per arenas, try decreasing total players";
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return;
            }

            defaultArenas--;
            arenaPlayersText.text = defaultArenas.ToString();
        }

        public void IncrementArenaPlayers()
        {
            createErrorText.text = "";

            if (defaultArenas + 1 > totalArenaRange.y)
            {
                createErrorText.text = $"Can't have more than {totalArenaRange.y} arenas";
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return;
            }

            if( (defaultArenas + 1) * perArenaPlayersRange.x > defaultTotalPlayers)
            {
                createErrorText.text = $"Minimum {perArenaPlayersRange.x} players per arena, try increasing players";
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return;
            }

            defaultArenas++;
            arenaPlayersText.text = defaultArenas.ToString();
        }

        public void TogglePlayAs()
		{
            if (LocalPlayer.Instance.defaultPlayerType == PlayerType.Player || LocalPlayer.Instance.defaultPlayerType == PlayerType.Spectator) Debug.LogError("Something went wrong during initiation..");

            PlayerType updatedPlayerType = (PlayerType)(1 - (int)LocalPlayer.Instance.defaultPlayerType);
            LocalPlayer.Instance.defaultPlayerType = updatedPlayerType;

            totalPlayersRange.y = updatedPlayerType == PlayerType.Master_Spectator ? totalPlayersRange.y - 1 : totalPlayersRange.y + 1;
            if (updatedPlayerType == PlayerType.Master_Spectator && defaultTotalPlayers > totalPlayersRange.y)
            {
                defaultTotalPlayers -= 1;
                totalPlayersText.text = defaultTotalPlayers.ToString();
            }
            defaultPlayerTypeText.text = updatedPlayerType == PlayerType.Master_Spectator ? "Spectator" : "Player";
        }

        public void CreateRoom()
        {
            if(!canCreateRoom)
            {
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return;
            }

            //Reset func on valid click
            canCreateRoom = false;
            createErrorText.text = "";

            //Room options {Note: these values are used to keep player disconnection to the absolute minimum}
            RoomOptions roomOptions = new RoomOptions();
            int maxPlayers = LocalPlayer.Instance.defaultPlayerType == PlayerType.Master_Player ? defaultTotalPlayers : defaultTotalPlayers + 1;
            Debug.Log(maxPlayers);
            roomOptions.MaxPlayers = (byte)( maxPlayers);
            roomOptions.CleanupCacheOnLeave = MultiplayerManager.Instance.gameSettings.cleanupCacheOnLeave;
            roomOptions.PlayerTtl = MultiplayerManager.Instance.gameSettings.playerTTL;
            roomOptions.EmptyRoomTtl = MultiplayerManager.Instance.gameSettings.playerTTL;
            roomOptions.PublishUserId = true;

            //Add the essential room properties {no. of arenas }
            Hashtable roomProperties = new Hashtable();
            roomProperties.Add(Constants.Room.AvailableArenas, defaultArenas);
            roomProperties.Add(Constants.Room.OccupiedArenas, defaultArenas);
            roomProperties.Add(Constants.Room.RoomState, (int)RoomState.Unassigned_Arenas);
            roomOptions.CustomRoomProperties = roomProperties;

            //Create a room of random name
            string roomName = GenerateRandomRoomName();
            UtilEvents.ShowToastMessage?.Invoke("Creating Room: " + roomName);
            PhotonNetwork.CreateRoom(roomName, roomOptions, SessionData.gameLobby);
        }

        private string GenerateRandomRoomName()
        {
            int randomRoomNo =  Random.Range(1, 1000);
            return roomNoPrefix + randomRoomNo;

        }

        #endregion

        //Update Room UI when a room is created or destroy
        private void UpdateRoomListUI(List<RoomInfo> roomInfoList)
        {
            UpdateJoinRoomData();

            //If no rooms in cache then return
            if (roomInfoList.Count == 0) return;

            //If any room has availability then show that room else return 
            bool availableRooms = false;
            for (int i = 0; i < roomInfoList.Count; i++)
            {

                if (roomInfoList[i].PlayerCount > 0)
                {
                    availableRooms = true;
                    break;
                }

            }

            if (!availableRooms) return;

            noRoomsAvailable.SetActive(false);
            for (int i = 0; i < roomInfoList.Count; i++)
            {
                if (roomInfoList[i].PlayerCount == 0) continue;

                currentRoomNames[i] = roomInfoList[i].Name;
                joinRoomObjects[i].SetActive(true);
                joinRoomObjectTexts[i].text = roomInfoList[i].Name + $"\n<#E56F47><size= 38> Players: " +
                    $"({roomInfoList[i].PlayerCount}/{roomInfoList[i].MaxPlayers})";
                                       
            }
        }

        //Join a room
        public void JoinRoom(int roomNo)
        {
            if(!canJoinRoom)
            {
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return;
            }

            canJoinRoom = false;

            PhotonNetwork.JoinRoom(currentRoomNames[roomNo]);
        }

        public void JoinRoomThroughText()
        {
            if (!canJoinRoom)
            {
                SoundMessages.PlaySFX?.Invoke(errorAudioClip);
                return;
            }

            if(string.IsNullOrEmpty(joinRoomId.text))
            {
                foreach (var errorText in joinErrorText)
                {
                    errorText.text = "No Room Id Detected. Try Again!";
                }
                
                return;
            }

            canJoinRoom = false;

            PhotonNetwork.JoinRoom(roomNoPrefix + joinRoomId.text);
        }

        private void GoToLobbyMenu()
        {
            //Defines if local client is initiating the lobby
            goToLobby = true;

            if (LocalPlayer.Instance.defaultPlayerType == PlayerType.Master_Spectator)
                MultiplayerManager.Instance.SetModeratorArenaOnNetwork(0);
            else if (LocalPlayer.Instance.defaultPlayerType == PlayerType.Master_Player)
                MultiplayerManager.Instance.SetModeratorArenaOnNetwork(-1);      

        }

        //Creator of room loads the lobby scene, due to auto scene sync all the other memebers will follow
        private void LoadLobby(Player targetPlayer, Hashtable changedProps)
        {
            if (goToLobby) PhotonNetwork.LoadLevel(Constants.LobbyScene);
        }
    }
}

