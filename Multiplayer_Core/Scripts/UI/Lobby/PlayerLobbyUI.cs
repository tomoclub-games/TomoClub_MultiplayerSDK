using UnityEngine;
using TMPro;
using System.Collections.Generic;
using TomoClub.Core;
using Photon.Realtime;

public class PlayerLobbyUI : MonoBehaviour
{
    [Header("Lobby UI")]
    [SerializeField] GameObject Panel_PlayerLobby;
    [SerializeField] GameObject Popup_PlayerLobbyList;
    [SerializeField] GameObject Popup_PlayerArenaList;

    [Header("Lobby List")]
    [SerializeField] TextMeshProUGUI Text_LobbyPlayerListTitle;
    [SerializeField] GameObject[] LobbyPlayers;

    [Header("Arena List")]
    [SerializeField] GameObject[] ArenaPlayers;
    [SerializeField] GameObject Button_Arena;
    [SerializeField] TextMeshProUGUI Text_ArenaPlayersTitle;
    [SerializeField] TextMeshProUGUI Text_ArenaPlayersButtonTitle;


    [Header("Tutorial")]
    [SerializeField] GameObject TutorialPrefab;
    [SerializeField] Transform TutorialParent;


    private TextMeshProUGUI[] playerLobbyListText;
    private TextMeshProUGUI[] playerArenaListText;

    private void Awake() => Init();

    private void OnEnable()
    {
        MultiplayerMesseges.OnUpdateLobbyList += UpdatePlayerLobbyList;
        MultiplayerMesseges.OnUpdateMyArenaList += UpdatePlayerArenaList;
    }

    private void OnDisable()
    {
        MultiplayerMesseges.OnUpdateLobbyList -= UpdatePlayerLobbyList;
        MultiplayerMesseges.OnUpdateMyArenaList -= UpdatePlayerArenaList;
    } 
    
    private void Init()
    {
        //Player Lobby List Init
        playerLobbyListText = new TextMeshProUGUI[LobbyPlayers.Length];

        for (int i = 0; i < LobbyPlayers.Length; i++)
        {
            playerLobbyListText[i] = LobbyPlayers[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            LobbyPlayers[i].SetActive(false);
        }

        Popup_PlayerLobbyList.SetActive(false);


        //Player Arena List Init
        playerArenaListText = new TextMeshProUGUI[ArenaPlayers.Length];

        for (int i = 0; i < ArenaPlayers.Length; i++)
        {
            playerArenaListText[i] = ArenaPlayers[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            ArenaPlayers[i].SetActive(false);
        }

        Popup_PlayerArenaList.SetActive(false);
        Button_Arena.SetActive(LocalPlayer.Instance.arenaNo > 0);

        //Instantiate Tutorial
        var tutorial = Instantiate(TutorialPrefab, TutorialParent.position, Quaternion.identity, TutorialParent);
        tutorial.GetComponent<TutorialManager>().SetTutorialBG(0f);

        Panel_PlayerLobby.SetActive(LocalPlayer.Instance.isClassroomPlayer);
        gameObject.SetActive(LocalPlayer.Instance.isClassroomPlayer);

    }

    //Open/Close LobbyPlayerList
    public void TogglePlayerLobbyList(bool status) => Popup_PlayerLobbyList.SetActive(status);

    //Open/Close ArenaPlayerList
    public void TogglePlayerArenaList(bool status) => Popup_PlayerArenaList.SetActive(status);

    private void UpdatePlayerArenaList(List<Player> currentArenaPlayers)
    {

        Button_Arena.SetActive(LocalPlayer.Instance.arenaNo >= 1);

        if (LocalPlayer.Instance.arenaNo < 1) return; 

        for (int i = 1; i < ArenaPlayers.Length; i++)
        {
            ArenaPlayers[i].SetActive(false);
        }

        for (int i = 0; i < currentArenaPlayers.Count; i++)
        {
            ArenaPlayers[i].SetActive(true);
            playerArenaListText[i].text = currentArenaPlayers[i].NickName;
        }

        Text_ArenaPlayersTitle.text = $"Arena {LocalPlayer.Instance.arenaNo} <size=40><#6A6A6A> Total Players: {currentArenaPlayers.Count}";
        Text_ArenaPlayersButtonTitle.text = $"Arena {LocalPlayer.Instance.arenaNo}";

    }

    private void UpdatePlayerLobbyList(List<Player> allPlayers)
    {
        Text_LobbyPlayerListTitle.text = $"Lobby <size=40><#6A6A6A> Total Players: {allPlayers.Count}";
        for (int i = 0; i < LobbyPlayers.Length; i++)
        {
            if (i < allPlayers.Count)
            {
                bool isModerator = allPlayers[i].CustomProperties[Constants.Player.ArenaNo] != null && (int)allPlayers[i].CustomProperties[Constants.Player.ArenaNo] == 0;
                LobbyPlayers[i].SetActive(true);
                playerLobbyListText[i].text = isModerator ? allPlayers[i].NickName + "(Mod)" : allPlayers[i].NickName;

            }
            else
                LobbyPlayers[i].SetActive(false);
        }
        
    }

}
