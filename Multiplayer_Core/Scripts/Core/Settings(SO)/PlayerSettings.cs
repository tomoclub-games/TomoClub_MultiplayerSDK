using UnityEngine;

namespace TomoClub.Core
{
	[CreateAssetMenu(menuName = "Settings/Player_Settings")]
	public class PlayerSettings : ScriptableObject
	{
		[Header("Player Settings")]
		[Tooltip("Moderator Build Initial Player Type")]
		public PlayerType modInitialPlayerType;
		[Tooltip("Player Build Initial Player Type")]
		public PlayerType playerInitialPlayerType;
		[Tooltip("Amount of time in seconds till the player can join back a room after being kicked out")]
		[SerializeField] public int playerTimeout = 300;

	}  
}

