using UnityEngine;

public class NotificationManager : SingletonMono<NotificationManager>
{
    [Header("配置")]
    public GameObject notificationPrefab;
    public Transform notificationContainer;

    [Tooltip("最多同时显示的通知数量，防止刷屏")]
    public int maxMessageCount = 5;

    public void Show(string content)
    {
        var newTip = Instantiate(notificationPrefab, notificationContainer);
        newTip.GetComponent<NotificationUI>().Setup(content);

        // Remove oldest notification if over limit
        if (notificationContainer.childCount > maxMessageCount)
            Destroy(notificationContainer.GetChild(0).gameObject);
    }
}
