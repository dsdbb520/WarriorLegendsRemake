using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class UITipTrigger : MonoBehaviour
{
    [Header("提示内容")]
    [TextArea]
    public string tipText;

    [Header("显示区域 ID")]
    public string tipID = "Tip_0";

    [Header("显示时间（秒），如果为-1则使用ActionTipUI默认时间")]
    public float displayDuration = -1f;

    [Header("进入后是否隐藏其他区域提示")]
    public bool hideOtherTips = true;

    [Header("触发范围可视化颜色")]
    public Color gizmoColor = new Color(1f, 1f, 0f, 0.3f); //淡黄色

    [Header("是否可重复触发")]
    public bool canRepeat = false;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!canRepeat && hasTriggered) return;

        hasTriggered = true;

        // 隐藏其他提示
        if (hideOtherTips && ActionTipUI.Instance != null)
        {
            foreach (var tip in ActionTipUI.Instance.tips)
            {
                if (tip.tipID != tipID)
                {
                    ActionTipUI.Instance.HideTip(tip.tipID);
                }
            }
        }

        // 显示当前提示
        if (ActionTipUI.Instance != null)
        {
            ActionTipUI.Instance.ShowTip(tipID, tipText, displayDuration);
        }
    }

    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
    }
}
