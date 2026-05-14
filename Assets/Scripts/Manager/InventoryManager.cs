using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : SingletonMono<InventoryManager>
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(ItemDataSO data, int amount)
    {
        if (data == null) return;

        if (data.stackable)
        {
            var existing = items.Find(x => x.itemData == data);
            if (existing != null) { existing.amount += amount; return; }
        }

        items.Add(new InventoryItem { itemData = data, amount = amount });
    }

    public void UseItem(InventoryItem item)
    {
        if (item.amount <= 0) return;

        var player = PlayerManager.Instance?.GetComponent<Character>();
        if (player == null) return;

        switch (item.itemData.itemType)
        {
            case ItemType.Useable:
                player.Heal(item.itemData.useValue);
                NotificationManager.Instance?.Show($"恢复了 {item.itemData.useValue} 点HP");
                item.amount--;
                break;

            case ItemType.Equipment:
                Debug.Log($"装备了 {item.itemData.itemName}");
                break;
        }

        if (item.amount <= 0)
            items.Remove(item);

        FindObjectOfType<BackpackPanel>()?.UpdateItems();
    }
}
