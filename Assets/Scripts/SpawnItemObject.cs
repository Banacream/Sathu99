using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSpawnInfo
{
    public GameObject itemPrefab; // Prefab ของไอเท็ม
    public int numberOfItems; // จำนวนไอเท็มที่ต้องการ spawn
}

public class SpawnItemObject : MonoBehaviour
{
    public List<ItemSpawnInfo> itemSpawnInfos; // รายการของข้อมูลการ spawn ไอเท็ม
    public List<Transform> spawnPositions; // รายการของตำแหน่งที่สามารถ spawn ไอเท็มได้

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        List<Transform> availablePositions = new List<Transform>(spawnPositions); // คัดลอกตำแหน่งที่สามารถ spawn ได้
        Dictionary<Transform, GameObject> occupiedPositions = new Dictionary<Transform, GameObject>(); // เก็บตำแหน่งที่ถูกใช้ไปแล้วและไอเท็มที่ spawn ในตำแหน่งนั้น

        foreach (ItemSpawnInfo spawnInfo in itemSpawnInfos)
        {
            for (int i = 0; i < spawnInfo.numberOfItems; i++)
            {
                Transform spawnPosition = null;

                // หาตำแหน่งที่เหมาะสมสำหรับการ spawn
                if (occupiedPositions.ContainsValue(spawnInfo.itemPrefab))
                {
                    // ถ้าไอเท็มชนิดเดียวกันถูก spawn ในตำแหน่งใดตำแหน่งหนึ่งแล้ว ให้ใช้ตำแหน่งนั้น
                    foreach (var entry in occupiedPositions)
                    {
                        if (entry.Value == spawnInfo.itemPrefab)
                        {
                            spawnPosition = entry.Key;
                            break;
                        }
                    }
                }
                else
                {
                    // ถ้าไอเท็มชนิดนี้ยังไม่ถูก spawn ให้หาตำแหน่งใหม่
                    if (availablePositions.Count > 0)
                    {
                        int randomIndex = Random.Range(0, availablePositions.Count);
                        spawnPosition = availablePositions[randomIndex];
                        availablePositions.RemoveAt(randomIndex);
                    }
                    else
                    {
                        Debug.LogWarning("No available positions left for item: " + spawnInfo.itemPrefab.name);
                        break;
                    }
                }

                if (spawnPosition != null)
                {
                    // สุ่มตำแหน่งภายในรัศมีของตำแหน่งที่เลือก
                    Vector3 randomPosition = spawnPosition.position + Random.insideUnitSphere * 10f; // รัศมี 1 หน่วย
                    randomPosition.y = spawnPosition.position.y; // ตั้งค่า y ให้ตรงกับตำแหน่งที่เลือก

                    Instantiate(spawnInfo.itemPrefab, randomPosition, Quaternion.identity);

                    // บันทึกตำแหน่งที่ใช้ไปแล้วและไอเท็มที่ spawn ในตำแหน่งนั้น
                    if (!occupiedPositions.ContainsKey(spawnPosition))
                    {
                        occupiedPositions.Add(spawnPosition, spawnInfo.itemPrefab);
                    }
                }
            }
        }
    }

}
