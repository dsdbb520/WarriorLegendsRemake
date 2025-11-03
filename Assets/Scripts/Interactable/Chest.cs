using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour, IInteractable
{
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

    public void Interact()   //在宝箱旁边按下F键将进入这段函数
    {
        if (!isOpened)
        {
            StartCoroutine(OpenChestCoroutine());
        }
    }

    private IEnumerator OpenChestCoroutine()
    {
        Debug.Log("宝箱即将打开");
        // 延迟一段时间再切换 Sprite
        yield return new WaitForSeconds(openDelay);

        spriteRenderer.sprite = openSprite;
        isOpened = true;

        Debug.Log("宝箱已打开！");
        // TODO: 掉落奖励逻辑
    }
}
