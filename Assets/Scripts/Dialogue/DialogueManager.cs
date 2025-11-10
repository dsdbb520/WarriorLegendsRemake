using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem; // 新输入系统

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI引用")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;
    public GameObject dialoguePanel; // 包含背景和文本
    public Image backgroundImage;    // 对话背景 Image
    public float typingSpeed = 0.03f;
    public float fadeDuration = 0.3f; // 背景淡入淡出时间

    private string[] currentLines;
    private int currentIndex;
    private bool isTyping;
    private bool skipTyping;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = dialoguePanel.AddComponent<CanvasGroup>();

        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string npcName, string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return;

        nameText.text = npcName;
        currentLines = lines;
        currentIndex = 0;

        dialoguePanel.SetActive(true);
        StartCoroutine(FadeInPanel());
        StartCoroutine(DisplayLine());
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
    }

    private IEnumerator DisplayLine()
    {
        isTyping = true;
        skipTyping = false;

        string line = currentLines[currentIndex];
        contentText.text = "";

        foreach (char c in line)
        {
            if (skipTyping)
            {
                contentText.text = line;
                break;
            }

            contentText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        skipTyping = false;

        // 使用新输入系统等待空格键
        yield return new WaitUntil(() => Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);

        NextLine();
    }

    private void NextLine()
    {
        if (currentIndex < currentLines.Length - 1)
        {
            currentIndex++;
            StartCoroutine(DisplayLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        StartCoroutine(FadeOutPanel());
        currentLines = null;
    }

    private void Update()
    {
        // 使用新输入系统检测空格键，跳过打字效果
        if (isTyping && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            skipTyping = true;
    }
}
