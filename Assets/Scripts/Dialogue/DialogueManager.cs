using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : SingletonMono<DialogueManager>
{
    [Header("UI组件")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;
    public GameObject dialoguePanel;
    public Image backgroundImage;
    public float typingSpeed = 0.03f;
    public float fadeDuration = 0.3f;

    private string[] currentLines;
    private int currentIndex;
    private bool isTyping;
    private bool skipTyping;
    private CanvasGroup canvasGroup;
    private System.Action onDialogueFinishedCallback;

    // Track coroutine references so we can stop before restarting
    private Coroutine _displayCoroutine;
    private Coroutine _fadeCoroutine;

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = dialoguePanel.AddComponent<CanvasGroup>();
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string npcName, string[] lines, System.Action callback = null)
    {
        if (lines == null || lines.Length == 0) return;

        // Stop any in-progress coroutines before starting fresh
        StopAllDialogueCoroutines();

        RectTransform rect = dialoguePanel.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x,
            npcName == "PlayerSelf" ? 84f : 0f);

        onDialogueFinishedCallback = callback;
        nameText.text = npcName;
        currentLines = lines;
        currentIndex = 0;
        isTyping = false;
        skipTyping = false;

        dialoguePanel.SetActive(true);
        _fadeCoroutine = StartCoroutine(FadeInPanel());
        _displayCoroutine = StartCoroutine(DisplayLine());
    }

    private void StopAllDialogueCoroutines()
    {
        if (_displayCoroutine != null) { StopCoroutine(_displayCoroutine); _displayCoroutine = null; }
        if (_fadeCoroutine != null)    { StopCoroutine(_fadeCoroutine);    _fadeCoroutine = null; }
    }

    private IEnumerator FadeInPanel()
    {
        float t = 0f;
        canvasGroup.alpha = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        _fadeCoroutine = null;
    }

    private IEnumerator FadeOutPanel()
    {
        float t = 0f;
        canvasGroup.alpha = 1f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        dialoguePanel.SetActive(false);
        _fadeCoroutine = null;

        onDialogueFinishedCallback?.Invoke();
        onDialogueFinishedCallback = null;
    }

    private IEnumerator DisplayLine()
    {
        isTyping = true;
        skipTyping = false;

        string line = currentLines[currentIndex];
        contentText.text = "";

        foreach (char c in line)
        {
            if (skipTyping) { contentText.text = line; break; }
            contentText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        skipTyping = false;

        yield return new WaitUntil(() => Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);

        _displayCoroutine = null;
        NextLine();
    }

    private void NextLine()
    {
        if (currentIndex < currentLines.Length - 1)
        {
            currentIndex++;
            _displayCoroutine = StartCoroutine(DisplayLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        currentLines = null;
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeOutPanel());
    }

    private void Update()
    {
        if (isTyping && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            skipTyping = true;
    }
}
