using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalSpawnInfo
{
    public GameObject animalPrefab; // Prefab ของสัตว์
    public int numberOfAnimals; // จำนวนสัตว์ที่ต้องการ spawn
    public Transform spawnCenterObject;
    public float spawnRadius; // รัศมีในการ spawn สัตว์
}
public class SpawnAnimal : MonoBehaviour
{
    public List<AnimalSpawnInfo> animalSpawnInfos; // รายการของข้อมูลการ spawn สัตว์

    void Start()
    {
        SpawnAnimals();
    }

    void SpawnAnimals()
    {
        foreach (AnimalSpawnInfo spawnInfo in animalSpawnInfos)
        {
            Vector3 spawnCenter = spawnInfo.spawnCenterObject != null ? spawnInfo.spawnCenterObject.position : Vector3.zero;

            for (int i = 0; i < spawnInfo.numberOfAnimals; i++)
            {
                Vector3 randomPosition = spawnCenter + Random.insideUnitSphere * spawnInfo.spawnRadius;
                randomPosition.y = 0; // ตั้งค่า y เป็น 0 เพื่อให้สัตว์อยู่บนพื้น
                Instantiate(spawnInfo.animalPrefab, randomPosition, Quaternion.identity);
            }
        }
    }
}
