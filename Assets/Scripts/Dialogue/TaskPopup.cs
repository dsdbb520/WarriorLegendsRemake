using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskPopup : MonoBehaviour
{
    [Header("UI 引用")]
    public TextMeshProUGUI titleText;       // 任务标题
    public TextMeshProUGUI contentText;     // 任务描述
    public Image backgroundImage;           // 背景图片
    public Button acceptButton;
    public Button rejectButton;
    public Button closeButton;

    private System.Action onAccept;
    private System.Action onReject;

    public void Setup(string title, string description, System.Action acceptCallback, System.Action rejectCallback)
    {
        if (titleText != null)
            titleText.text = title;

        if (contentText != null)
            contentText.text = description;

        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(true);  // 显示背景

        onAccept = acceptCallback;
        onReject = rejectCallback;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        // 移除旧监听
        acceptButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

        // 按钮绑定
        acceptButton.onClick.AddListener(() =>
        {
            onAccept?.Invoke();
            Destroy(gameObject);
        });

        rejectButton.onClick.AddListener(() =>
        {
            onReject?.Invoke();
            Destroy(gameObject);
        });

        closeButton.onClick.AddListener(() =>
        {
            onReject?.Invoke();
            Destroy(gameObject);
        });
    }
}
