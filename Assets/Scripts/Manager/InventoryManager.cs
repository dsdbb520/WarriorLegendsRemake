using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // 改用新的 InventoryItem 类
    public List<InventoryItem> items = new List<InventoryItem>();
    public ItemDataSO TestItem;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InventoryManager.Instance.AddItem(TestItem, 5);
    }

    // 添加物品
    public void AddItem(ItemDataSO data, int amount)
    {
        // 检查是否可堆叠且已存在
        if (data.stackable)
        {
            InventoryItem existingItem = items.Find(x => x.itemData == data);
            if (existingItem != null)
            {
                existingItem.amount += amount;
                return;
            }
        }

        // 新增物品
        InventoryItem newItem = new InventoryItem { itemData = data, amount = amount };
        items.Add(newItem);
    }

    // 使用物品的核心逻辑
    public void UseItem(InventoryItem item)
    {
        if (item.amount <= 0) return;

        // 获取玩家引用
        Character player = PlayerManager.Instance.GetComponent<Character>();

        // 根据类型执行效果
        switch (item.itemData.itemType)
        {
            case ItemType.Useable:
                //假设useValue是回血量
                player.Heal(item.itemData.useValue);
                NotificationManager.Instance.Show("回复了 " + item.itemData.useValue + "点HP！");
                item.amount--; // 消耗一个
                break;

            case ItemType.Equipment:
                //TODO: 装备逻辑（后续实现）
                Debug.Log($"装备了 {item.itemData.itemName}");
                break;
        }

        //如果数量为0，从背包移除
        if (item.amount <= 0)
        {
            items.Remove(item);
        }

        //通知 UI 刷新（如果 BackpackPanel 是开着的）
        //更好的做法是用事件，这里简单直接调用
        FindObjectOfType<BackpackPanel>()?.UpdateItems();
    }
}