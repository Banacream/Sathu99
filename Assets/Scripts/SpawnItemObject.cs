using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ItemSpawnInfo
{
    public GameObject itemPrefab; // Prefab �ͧ�����
    public int numberOfItems; // �ӹǹ���������ͧ��� spawn
}

public class SpawnItemObject : MonoBehaviour
{
    public List<ItemSpawnInfo> itemSpawnInfos; // ��¡�âͧ�����š�� spawn �����
    public List<Transform> spawnPositions; // ��¡�âͧ���˹觷������ö spawn �������

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        List<Transform> availablePositions = new List<Transform>(spawnPositions); // �Ѵ�͡���˹觷������ö spawn ��
        Dictionary<Transform, GameObject> occupiedPositions = new Dictionary<Transform, GameObject>(); // �纵��˹觷��١������������������ spawn 㹵��˹觹��

        foreach (ItemSpawnInfo spawnInfo in itemSpawnInfos)
        {
            for (int i = 0; i < spawnInfo.numberOfItems; i++)
            {
                Transform spawnPosition = null;

                // �ҵ��˹觷�������������Ѻ��� spawn
                if (occupiedPositions.ContainsValue(spawnInfo.itemPrefab))
                {
                    // ����������Դ���ǡѹ�١ spawn 㹵��˹�㴵��˹�˹������ �������˹觹��
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
                    // ����������Դ����ѧ���١ spawn ����ҵ��˹�����
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
                    // �������˹���������բͧ���˹觷�����͡
                    Vector3 randomPosition = spawnPosition.position + Random.insideUnitSphere * 30f; // ����� 1 ˹���
                    randomPosition.y = spawnPosition.position.y; // ��駤�� y ���ç�Ѻ���˹觷�����͡
                     NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomPosition, out hit, 10f, NavMesh.AllAreas))
                    {
                        randomPosition.y = hit.position.y + 0.3f; // กำหนดตำแหน่งที่หาได้จาก NavMesh
                    }

                    Instantiate(spawnInfo.itemPrefab, randomPosition, Quaternion.identity);

                    // �ѹ�֡���˹觷�������������������� spawn 㹵��˹觹��
                    if (!occupiedPositions.ContainsKey(spawnPosition))
                    {
                        occupiedPositions.Add(spawnPosition, spawnInfo.itemPrefab);
                    }
                }
            }
        }
    }

}
