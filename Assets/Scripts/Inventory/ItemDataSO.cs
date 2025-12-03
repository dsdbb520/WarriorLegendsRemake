using UnityEngine;

public enum ItemType { Useable, Equipment, Quest } // 物品类型枚举

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;

    [TextArea] 
    public string description; //描述

    public bool stackable = true;         //是否可堆叠

    [Header("物品数据")]
    public int useValue;  //回血量、增加的攻击力等
}