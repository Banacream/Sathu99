using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Create new Item", order = 4)]
public class DataItem : ScriptableObject
{
    public Sprite icon;
    public string id;
    public string itemName;
    public string description;
    public int maxStack;
    public float weight;

    [Header("In Game Object")]
    public GameObject gamePrefab;

    [Header("Item Type")]
    public ItemType itemType; // ประเภทของไอเทม

    // Enum สำหรับประเภทของไอเทม
    public enum ItemType
    {
        Food,   // อาหาร
        Tool,   // เครื่องมือ
        Material, // วัตถุดิบ
    }
}
