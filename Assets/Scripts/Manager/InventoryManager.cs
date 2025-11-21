using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.Progress;


public class Item
{
    public string itemName;    // 物品名称
    public Sprite itemIcon;    // 物品图标
    public int itemCount;      // 物品数量
}


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<Item> items = new List<Item>(); // 背包中的物品

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 添加物品到背包
    public void AddItem(Item newItem)
    {
        items.Add(newItem);
    }

    // 删除物品
    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }
}
