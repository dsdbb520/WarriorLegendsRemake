using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class NotificationUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float displayDuration = 2.0f; //显示多久
    public float fadeDuration = 0.5f;    //淡入淡出时间

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        //初始完全透明且不阻挡射线
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void Setup(string message)
    {
        messageText.text = message;
        StartCoroutine(FadeProcess());
    }

    //淡入淡出
    private IEnumerator FadeProcess()
    {
        yield return FadeTo(1f);

        yield return new WaitForSeconds(displayDuration);

        yield return FadeTo(0f);

        Destroy(gameObject);
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}