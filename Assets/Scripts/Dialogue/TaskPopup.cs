using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskPopup : MonoBehaviour
{
    [Header("UI 引用")]
    public TextMeshProUGUI contentText;
    public Button acceptButton;
    public Button rejectButton;
    public Button closeButton;

    private System.Action onAccept;
    private System.Action onReject;

    public void Setup(string message, System.Action acceptCallback, System.Action rejectCallback)
    {
        contentText.text = message;
        onAccept = acceptCallback;
        onReject = rejectCallback;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        // 按钮绑定
        acceptButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

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
            onReject?.Invoke(); // 关闭等同于拒绝
            Destroy(gameObject);
        });
    }
}
