using UnityEngine;

// 挂载在每一个可显示交互标识的物体上
// 仅需在 Inspector 中指定 indicatorPrefab（例如问号/感叹号）
//调用Show显示提示符，Hide隐藏，OnInteracted自动根据逻辑判断是否隐藏
public class InteractionIndicator : MonoBehaviour
{
    [Header("指示符 Prefab")]
    public GameObject indicatorPrefab;

    private GameObject indicatorInstance;
    private bool hasBeenInteracted = false;

    [Header("交互后是否隐藏提示符")]
    public bool autoHideAfterInteract = true;

    [Header("提示符相对位置偏移")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    // 调用：显示提示符
    public void Show()
    {
        if (indicatorPrefab == null) return;
        if (hasBeenInteracted && autoHideAfterInteract) return;

        if (indicatorInstance == null)
        {
            indicatorInstance = Instantiate(indicatorPrefab, transform.position + offset, Quaternion.identity, transform);
            indicatorInstance.SetActive(true);
        }
        else
        {
            indicatorInstance.SetActive(true);
        }
    }

    // 调用：隐藏提示符
    public void Hide()
    {
        if (indicatorInstance != null)
            indicatorInstance.SetActive(false);
    }

    // 调用：物体被交互后（例如宝箱打开）
    public void OnInteracted()
    {
        hasBeenInteracted = true;
        if (autoHideAfterInteract && indicatorInstance != null)
            indicatorInstance.SetActive(false);
    }

    // 可选：重置状态（如果需要重复交互）
    public void ResetIndicator()
    {
        hasBeenInteracted = false;
        if (indicatorInstance == null && indicatorPrefab != null)
            indicatorInstance = Instantiate(indicatorPrefab, transform.position + offset, Quaternion.identity, transform);

        if (indicatorInstance != null)
            indicatorInstance.SetActive(true);
    }
}
