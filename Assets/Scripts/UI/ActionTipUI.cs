using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class ActionTipUI : MonoBehaviour
{
    [System.Serializable]
    public class TipEntry
    {
        public string tipID;                  // 唯一 ID
        public TMP_Text tipText;              // 提示文字
        public RectTransform backgroundRect;  // 背景
        public float displayDuration = 2f;    // 默认显示时间

        [Header("背景边距")]
        public Vector2 backgroundPadding = new Vector2(20f, 10f); // 背景比文字多出的空间

        [Header("文字外观")]
        public Color normalColor = Color.white;
        public bool enableGradient = false;
        public Color topColor = Color.yellow;
        public Color bottomColor = Color.red;
        public bool enableShadow = true;
        public Color shadowColor = Color.black;
        public Vector2 shadowOffset = new Vector2(1f, -1f);

        [HideInInspector]
        public Shadow shadow;
    }

    public List<TipEntry> tips = new List<TipEntry>();

    [Header("淡入淡出")]
    public float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;

    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();

    public static ActionTipUI Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // 初始化所有提示
        foreach (var tip in tips)
        {
            if (tip.tipText != null)
            {
                tip.tipText.gameObject.SetActive(false);

                // 设置阴影
                tip.shadow = tip.tipText.GetComponent<Shadow>();
                if (tip.enableShadow && tip.shadow == null)
                    tip.shadow = tip.tipText.gameObject.AddComponent<Shadow>();

                ApplyAppearance(tip);
            }

            if (tip.backgroundRect != null)
                tip.backgroundRect.gameObject.SetActive(false);
        }
    }

    private void ApplyAppearance(TipEntry tip)
    {
        tip.tipText.color = tip.normalColor;

        if (tip.enableGradient)
        {
            tip.tipText.colorGradient = new VertexGradient(tip.topColor, tip.topColor, tip.bottomColor, tip.bottomColor);
        }

        if (tip.enableShadow && tip.shadow != null)
        {
            tip.shadow.effectColor = tip.shadowColor;
            tip.shadow.effectDistance = tip.shadowOffset;
        }
    }

    public void ShowTip(string tipID, string text = null, float duration = -1f)
    {
        TipEntry tip = tips.Find(t => t.tipID == tipID);
        if (tip == null) return;

        if (duration < 0) duration = tip.displayDuration;
        if (!string.IsNullOrEmpty(text)) tip.tipText.text = text;

        tip.tipText.gameObject.SetActive(true);
        tip.backgroundRect.gameObject.SetActive(true);

        // 调整背景大小
        Vector2 size = new Vector2(tip.tipText.preferredWidth, tip.tipText.preferredHeight);
        tip.backgroundRect.sizeDelta = size + tip.backgroundPadding;

        // 启动淡入淡出
        if (activeCoroutines.ContainsKey(tipID) && activeCoroutines[tipID] != null)
            StopCoroutine(activeCoroutines[tipID]);

        activeCoroutines[tipID] = StartCoroutine(FadeInOut(tip, duration));
    }

    public void HideTip(string tipID)
    {
        TipEntry tip = tips.Find(t => t.tipID == tipID);
        if (tip == null) return;

        if (activeCoroutines.ContainsKey(tipID) && activeCoroutines[tipID] != null)
            StopCoroutine(activeCoroutines[tipID]);

        activeCoroutines[tipID] = StartCoroutine(FadeOut(tip));
    }

    // 保存当前显示状态并隐藏所有提示，返回显示的 tipID 列表
    public List<string> HideAllTipsAndReturnActive()
    {
        List<string> activeTipIDs = new List<string>();
        foreach (var tip in tips)
        {
            if (tip.tipText != null && tip.tipText.gameObject.activeSelf)
            {
                activeTipIDs.Add(tip.tipID);
                HideTip(tip.tipID);
            }
        }
        return activeTipIDs;
    }

    // 根据 tipID 列表恢复显示
    public void RestoreTips(List<string> tipIDs)
    {
        foreach (var tipID in tipIDs)
        {
            ShowTip(tipID);
        }
    }


    private IEnumerator FadeInOut(TipEntry tip, float duration)
    {
        yield return FadeTo(1f, fadeDuration);
        yield return new WaitForSeconds(duration);
        yield return FadeTo(0f, fadeDuration);

        tip.tipText.gameObject.SetActive(false);
        tip.backgroundRect.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut(TipEntry tip)
    {
        yield return FadeTo(0f, fadeDuration);
        tip.tipText.gameObject.SetActive(false);
        tip.backgroundRect.gameObject.SetActive(false);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
