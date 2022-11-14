using UnityEngine;


namespace TomoClub.Core
{
	public class LoadGameLevel : MonoBehaviour
	{
		private void Awake()
		{
			if (LocalPlayer.Instance.defaultPlayerType == PlayerType.Master_Player || LocalPlayer.Instance.defaultPlayerType == PlayerType.Master_Spectator)
				Photon.Pun.PhotonNetwork.LoadLevel(Constants.GameScene);
		}
	}

}