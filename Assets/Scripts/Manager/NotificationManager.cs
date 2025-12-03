using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("设置")]
    public GameObject notificationPrefab;
    public Transform notificationContainer;

    [Tooltip("最大同时显示数量，防止刷屏太高挡住视线")]
    public int maxMessageCount = 5;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Show(string content)
    {
        GameObject newTip = Instantiate(notificationPrefab, notificationContainer);

        newTip.GetComponent<NotificationUI>().Setup(content);

        //限制数量（如果超过上限，销毁最上面的那个）
        if (notificationContainer.childCount > maxMessageCount)
        {
            Destroy(notificationContainer.GetChild(0).gameObject);
        }
    }
}