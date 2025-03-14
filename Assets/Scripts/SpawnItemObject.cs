using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ItemSpawnInfo
{
    public GameObject itemPrefab; // Prefab ของไอเท็ม
    public int numberOfItems; // จำนวนไอเท็มที่จะ spawn
}

[System.Serializable]
public class ZoneSpawnInfo
{
    public string zoneName; // ชื่อโซน
    public List<ItemSpawnInfo> itemSpawnInfos; // รายการของไอเท็มที่จะ spawn ในโซนนี้
    public List<Transform> spawnPositions; // รายการตำแหน่งที่จะ spawn ในโซนนี้
}

public class SpawnItemObject : MonoBehaviour
{
    public List<ZoneSpawnInfo> zoneSpawnInfos; // รายการของโซนที่จะ spawn ไอเท็ม

    void Start()
    {
        foreach (ZoneSpawnInfo zoneSpawnInfo in zoneSpawnInfos)
        {
            SpawnItemsInZone(zoneSpawnInfo);
        }
    }

    void SpawnItemsInZone(ZoneSpawnInfo zoneSpawnInfo)
    {
        List<Transform> availablePositions = new List<Transform>(zoneSpawnInfo.spawnPositions); // คัดลอกตำแหน่งที่จะ spawn
        Dictionary<Transform, GameObject> occupiedPositions = new Dictionary<Transform, GameObject>(); // เก็บตำแหน่งที่ถูกใช้ไปแล้ว

        foreach (ItemSpawnInfo spawnInfo in zoneSpawnInfo.itemSpawnInfos)
        {
            int itemsPerPosition = Mathf.CeilToInt((float)spawnInfo.numberOfItems / availablePositions.Count); // จำนวนไอเท็มต่อจุด
            int itemsSpawned = 0;

            foreach (Transform spawnPosition in availablePositions)
            {
                if (itemsSpawned >= spawnInfo.numberOfItems)
                {
                    break;
                }

                for (int i = 0; i < itemsPerPosition && itemsSpawned < spawnInfo.numberOfItems; i++)
                {
                    
                        // สร้างตำแหน่งสุ่มรอบๆ ตำแหน่ง spawn
                        Vector3 randomPosition = spawnPosition.position + Random.insideUnitSphere * 40f; // รัศมี 40 หน่วย
                        randomPosition.y = spawnPosition.position.y; // กำหนดค่า y ให้ตรงกับตำแหน่ง spawn

                    NavMeshHit navMeshHit;
                    if (NavMesh.SamplePosition(randomPosition, out navMeshHit, 10f, NavMesh.AllAreas))
                    {
                        randomPosition.y = navMeshHit.position.y; // กำหนดตำแหน่งที่หาได้จาก NavMesh

                        // ตรวจสอบว่าตำแหน่งที่ spawn ไม่ลอยกับพื้น
                        RaycastHit hitR;
                        if (Physics.Raycast(randomPosition, Vector3.down, out hitR, Mathf.Infinity))
                        {
                            randomPosition.y = hitR.point.y + 0.3f; // กำหนดตำแหน่งที่หาได้จาก Raycast
                        }

                        Instantiate(spawnInfo.itemPrefab, randomPosition, Quaternion.identity);

                        // บันทึกตำแหน่งที่ถูกใช้ไปแล้ว
                        if (!occupiedPositions.ContainsKey(spawnPosition))
                        {
                            occupiedPositions.Add(spawnPosition, spawnInfo.itemPrefab);
                        }

                        itemsSpawned++;
                    }
                }
            }
        }
    }

}
