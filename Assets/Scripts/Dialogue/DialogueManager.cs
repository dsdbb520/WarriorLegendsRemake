using UnityEngine;
using System.Collections;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI引用")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;
    public GameObject dialoguePanel;
    public float typingSpeed = 0.03f; // 每个字出现间隔时间

    private string[] currentLines;
    private int currentIndex;
    private bool isTyping;
    private bool skipTyping;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartDialogue(string npcName, string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return;

        dialoguePanel.SetActive(true);
        nameText.text = npcName;
        currentLines = lines;
        currentIndex = 0;
        StartCoroutine(DisplayLine());
    }

    private IEnumerator DisplayLine()
    {
        isTyping = true;
        skipTyping = false;

        string line = currentLines[currentIndex];
        contentText.text = "";

        // 逐字显示
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

        // 等待玩家按下空格继续下一句
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

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
        dialoguePanel.SetActive(false);
        currentLines = null;
    }

    private void Update()
    {
        // 玩家在打字中按下空格，可以跳过当前句子的逐字显示
        if (isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            skipTyping = true;
        }
    }
}
