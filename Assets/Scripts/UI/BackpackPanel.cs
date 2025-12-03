using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BackpackPanel : MonoBehaviour
{
    public GameObject itemSlotPrefab;      // 物品条目预设
    public Transform itemListParent;       // 物品列表父物体
    public GameObject backpackPanel;       // 背包面板本身
    public ItemTooltip tooltip;

    private bool previousCanMove;
    private bool previousCanJump;
    private bool previousCanAttack;
    private bool previousCanInteract;
    private bool previousCanTask;
    private bool previousCanDodge;

    // 保存打开面板前显示的 Tip ID
    private List<string> activeTipIDs;

    private void Awake()
    {
        backpackPanel.SetActive(false); // 初始隐藏背包
    }


    public void ToggleBackpack()
    {
        if (PlayerActionManager.Instance == null || !PlayerActionManager.Instance.canBackpack) return;

        bool isActive = backpackPanel.activeSelf;
        gameObject.SetActive(!isActive); // 切换显示状态

        if (!isActive)
        {
            UpdateItems(); // 打开时刷新物品列表
            if (tooltip != null) tooltip.Hide(); //关闭之前的物品详情
            if (ActionTipUI.Instance != null)   // 隐藏 Tip
                activeTipIDs = ActionTipUI.Instance.HideAllTipsAndReturnActive();
            // 保存之前状态
            previousCanMove = PlayerActionManager.Instance.canMove;
            previousCanJump = PlayerActionManager.Instance.canJump;
            previousCanAttack = PlayerActionManager.Instance.canAttack;
            previousCanInteract = PlayerActionManager.Instance.canInteract;
            previousCanDodge = PlayerActionManager.Instance.canDodge;
            previousCanTask = PlayerActionManager.Instance.canTask;

            // 打开面板时禁用除 canTask 外的操作
            PlayerActionManager.Instance.EnableOnlyAction("backpack");

        }
        else
        {
            // 恢复之前的操作状态
            PlayerActionManager.Instance.canMove = previousCanMove;
            PlayerActionManager.Instance.canJump = previousCanJump;
            PlayerActionManager.Instance.canAttack = previousCanAttack;
            PlayerActionManager.Instance.canInteract = previousCanInteract;
            PlayerActionManager.Instance.canTask = previousCanTask;
            PlayerActionManager.Instance.canDodge = previousCanDodge;

            if (tooltip != null) tooltip.Hide();//关闭物品详情
            // 恢复 Tip
            if (ActionTipUI.Instance != null && activeTipIDs != null)
                ActionTipUI.Instance.RestoreTips(activeTipIDs);
        }
    }

    //更新背包内物品的显示
    public void UpdateItems()
    {
        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        //遍历新的 items 列表
        foreach (var item in InventoryManager.Instance.items)
        {
            var itemSlot = Instantiate(itemSlotPrefab, itemListParent);
            var itemNameText = itemSlot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = itemSlot.transform.Find("ItemIcon").GetComponent<Image>();
            var itemCountText = itemSlot.transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();

            //赋值 UI
            itemNameText.text = item.itemData.itemName;
            itemIcon.sprite = item.itemData.itemIcon;
            itemCountText.text = item.amount > 1 ? item.amount.ToString() : "";

            Button button = itemSlot.GetComponent<Button>();
            if (button != null)
            {
                //点击时弹窗
                button.onClick.AddListener(() =>
                {
                    if (tooltip != null)
                    {
                        tooltip.Show(item);
                    }
                });
            }
        }
    }
}
