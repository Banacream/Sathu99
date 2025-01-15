using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSlotSync : MonoBehaviour
{
    [System.Serializable]
    public class HandleSlotPair
    {
        public HandleSlot slot1;
        public HandleSlot slot2;
    }

    [SerializeField]
    private List<HandleSlotPair> slotPairs = new List<HandleSlotPair>();

    private void Start()
    {
        ValidateSlots();
    }

    private void ValidateSlots()
    {
        foreach (var pair in slotPairs)
        {
            if (pair.slot1 == null || pair.slot2 == null)
            {
                Debug.LogError("Please assign both HandleSlots in the pair!");
                return;
            }
        }
    }

    // เมทอดสำหรับ sync ข้อมูลระหว่าง slots
    public void SyncSlots(HandleSlot sourceSlot, DataItem newItem, int amount)
    {

        // หา pair ที่มี sourceSlot
        foreach (var pair in slotPairs)
        {
            if (pair.slot1 == sourceSlot)
            {
                // sync ไปยัง slot2
                pair.slot2.SetThisSlot(newItem, amount);
                break;
            }
        }
    }
}

