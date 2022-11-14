using UnityEngine;
using DG.Tweening;
using TMPro;

public class ToastMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI screenLogTextElement;
    [SerializeField] float toastAnimTime = 0.65f;
    [SerializeField] float toastStayTime = 2f;
    
    private RectTransform rectTransform;
    private float movementFactor;
    private float safety = 10f;
    private Sequence toastAnimation;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        movementFactor = rectTransform.rect.height + safety;

        //Initially dont show the toast message
        ResetRect();
    }

    private void ResetRect()
    {
        rectTransform.position = new Vector3(rectTransform.position.x, -movementFactor, rectTransform.position.y);
    }

    private void AnimateToast()
    {
        toastAnimation = DOTween.Sequence();
        toastAnimation.Append(rectTransform.DOAnchorPosY(0, toastAnimTime).SetEase(Ease.OutCubic));
        toastAnimation.Append(rectTransform.DOAnchorPosY(-movementFactor, toastAnimTime).SetEase(Ease.InCubic).SetDelay(toastStayTime));
        toastAnimation.OnComplete(() => toastAnimation = null);
    }

    public void ShowToastMessage(string text)
    {
        screenLogTextElement.text = text;
        if (toastAnimation != null && toastAnimation.IsPlaying()) toastAnimation.Kill();
        AnimateToast();
        
    }
}
