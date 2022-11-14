using TMPro;
using UnityEngine;
using TomoClub.Core;

public class LobbyUI : MonoBehaviour
{
    [Header("Lobby UI")]
    [SerializeField] TextMeshProUGUI Text_CurrentRoomName;

    private void Awake()
    {
        Text_CurrentRoomName.text = MultiplayerManager.Instance.currentRoomName;

    }

    public void CopyPlayerGameLink()
    {
        string gameLink = SessionData.hostingProvider == HostingProvider.Simmer ? SessionData.simmerLink : SessionData.itchLink;
        GUIUtility.systemCopyBuffer = gameLink + $" Room ID: {MultiplayerManager.Instance.currentRoomName}";
    }

    //Open Common Settings Menu on click
    public void SettingsButton()
    {
        if (PersistantUI.Instance != null)
            PersistantUI.Instance.ShowSettingsPopup();
    }

}
