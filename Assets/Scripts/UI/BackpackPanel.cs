using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BackpackPanel : MonoBehaviour
{
    public GameObject itemSlotPrefab;
    public Transform itemListParent;
    public GameObject backpackPanel;
    public ItemTooltip tooltip;

    private List<string> activeTipIDs;

    private void Awake()
    {
        backpackPanel.SetActive(false);
    }

    public void ToggleBackpack()
    {
        if (PlayerActionManager.Instance == null || !PlayerActionManager.Instance.canBackpack) return;

        bool isActive = backpackPanel.activeSelf;
        gameObject.SetActive(!isActive);

        if (!isActive)
        {
            UpdateItems();
            if (tooltip != null) tooltip.Hide();
            if (ActionTipUI.Instance != null)
                activeTipIDs = ActionTipUI.Instance.HideAllTipsAndReturnActive();

            // Lock everything except the backpack action
            PlayerActionManager.Instance.LockActions("backpack", "backpack");
        }
        else
        {
            PlayerActionManager.Instance.UnlockActions("backpack");
            if (tooltip != null) tooltip.Hide();
            if (ActionTipUI.Instance != null && activeTipIDs != null)
                ActionTipUI.Instance.RestoreTips(activeTipIDs);
        }
    }

    public void UpdateItems()
    {
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        foreach (var item in InventoryManager.Instance.items)
        {
            var itemSlot = Instantiate(itemSlotPrefab, itemListParent);
            itemSlot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = item.itemData.itemName;
            itemSlot.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.itemData.itemIcon;

            var countText = itemSlot.transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
            countText.text = item.amount > 1 ? item.amount.ToString() : "";

            Button button = itemSlot.GetComponent<Button>();
            if (button != null)
            {
                var capturedItem = item;
                button.onClick.AddListener(() => tooltip?.Show(capturedItem));
            }
        }
    }
}
