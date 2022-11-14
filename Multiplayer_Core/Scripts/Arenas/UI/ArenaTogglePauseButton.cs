using UnityEngine.UI;
using UnityEngine;


namespace TomoClub.Arenas
{
	public class ArenaTogglePauseButton : MonoBehaviour
	{
		[SerializeField] Button arenaTogglePauseButton;
		[SerializeField] Image arenaToggePauseImage;
		[SerializeField] GameObject buttonHolder;

		public void SetButtonState(bool isActive) => arenaTogglePauseButton.interactable = isActive;
		public void SetSprite(Sprite sprite) => arenaToggePauseImage.sprite = sprite;
		public void SetButtonHolder(bool isActive) => buttonHolder.SetActive(isActive);

	} 
}
