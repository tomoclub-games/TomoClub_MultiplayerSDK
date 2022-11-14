using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RemoteProcedureCalls : MonoBehaviourPun
{
    public static RemoteProcedureCalls Instance;

    [Header("Managers")]
    [SerializeField] ArenaManager arenaManager;

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

    /// <summary>
    /// On Arena Timer Completed Logic on this client
    /// </summary>
    /// <param name="arenaNo"></param>
    [PunRPC]
    private void UpdateArenaTimerCompletedOnClient(int arenaNo)
    {
        arenaManager.UpdateArenaToCompletedState(arenaNo);
    }
    /// <summary>
    /// On Update Arena Timer Logic On This Client
    /// </summary>
    [PunRPC]
    private void UpdateArenaTimerOnClient(int arenaNo, int currentTime)
    {
        arenaManager.UpdateArenaTimerOnClient(arenaNo, currentTime);
        
    }

    /// <summary>
    /// On Update Arena Pause Status On This Client
    /// </summary>
    [PunRPC]
    private void UpdateArenaPauseStatusOnClient(int arenaNo, int updateState)
    {
        arenaManager.UpdatePlayerPauseOnClient(arenaNo, updateState);
    }
}
