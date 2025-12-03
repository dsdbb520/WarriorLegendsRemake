using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    [Header("UI 组件")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Button useButton;   //使用/装备按钮
    public Button closeButton; //关闭按钮
    public TextMeshProUGUI useButtonText; //按钮上的文字

    private InventoryItem currentItem; //当前显示的物品

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
        if (useButton != null)
        {
            useButton.onClick.AddListener(OnUseButtonClick);
        }

        //初始隐藏
        gameObject.SetActive(false);
    }

    public void Show(InventoryItem item)
    {
        currentItem = item;
        gameObject.SetActive(true);

        //更新文本
        itemNameText.text = item.itemData.itemName;
        itemDescriptionText.text = item.itemData.description;
        itemIcon.sprite = item.itemData.itemIcon;

        //根据类型控制按钮显示
        switch (item.itemData.itemType)
        {
            case ItemType.Useable:
                useButton.gameObject.SetActive(true);
                useButtonText.text = "使用";
                break;
            case ItemType.Equipment:
                useButton.gameObject.SetActive(true);
                useButtonText.text = "装备";
                break;
            default: //材料等不可直接使用的物品
                useButton.gameObject.SetActive(false);
                break;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnUseButtonClick()
    {
        if (currentItem != null)
        {
            //调用使用逻辑
            InventoryManager.Instance.UseItem(currentItem);
        }
    }
}