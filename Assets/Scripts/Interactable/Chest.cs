using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("持久化设置")]
    public string objectID;//必须独一无二


    public bool isOpened = false;
    public Sprite closedSprite;  // 宝箱关闭的图
    public Sprite openSprite;    // 宝箱开启的图
    private SpriteRenderer spriteRenderer;
    public float openDelay = 0.1f; // 延迟时间，让开箱更自然

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = closedSprite; // 初始状态关闭
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(objectID) && PlayerManager.Instance.IsObjectTriggered(objectID))
        {
            isOpened = true;
            spriteRenderer.sprite = openSprite; //直接显示开箱图
        }
        else
        {
            spriteRenderer.sprite = closedSprite;
        }
    }

    public void Interact()   //在宝箱旁边按下F键将进入这段函数
    {
        if (!isOpened)
        {
            StartCoroutine(OpenChestCoroutine());
        }
        var indicator = GetComponent<InteractionIndicator>();
        if (indicator != null) indicator.OnInteracted();
    }

    private IEnumerator OpenChestCoroutine()
    {
        Debug.Log("宝箱即将打开");
        // 延迟一段时间再切换 Sprite
        yield return new WaitForSeconds(openDelay);

        spriteRenderer.sprite = openSprite;
        isOpened = true;
        if (!string.IsNullOrEmpty(objectID))
        {
            PlayerManager.Instance.TriggerObject(objectID);
        }
        Debug.Log("宝箱已打开！");
        // TODO: 掉落奖励逻辑
    }
}
