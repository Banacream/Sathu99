using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    // Start is called before the first frame update
    public static SpawnItem Instance;

    public List<ItemObject> itemObjects;
    public float minRadius = 2.0f;
    public float maxRadius = 10.0f;

    public Transform itemPlayer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnItemFromPlayer(DataItem item , int amount)
    {
        if(item.gamePrefab == null)
        {
            //Debug error
            return;
        }

        Vector2 randPos = Random.insideUnitCircle.normalized * minRadius;
        Vector3 offset = new Vector3(randPos.x, 0, randPos.y);
        GameObject spawnItem = Instantiate(item.gamePrefab , itemPlayer.position + offset, Quaternion.identity);

        spawnItem.GetComponent<ItemObject>().SetAmount(amount);
    }
    public void SpawnItemByGUI(int SpawnAmount = 1)
    {
        for(int i = 0; i < SpawnAmount; i++)
        {
            int ind = Random.Range(0, itemObjects.Count);
            float distance = Random.Range(minRadius, maxRadius);
            Vector2 randPoint = Random.insideUnitCircle.normalized * distance;
            Vector3 offset = new Vector3(randPoint.x, 0, randPoint.y);
            ItemObject itemObjectSpawn = Instantiate(itemObjects[ind], itemPlayer.position + offset, Quaternion.identity);
            itemObjectSpawn.RandomAmount();
        }
    }



    public void SpawnItemAtPosition(GameObject itemPrefab, Vector3 position)
    {
        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Item prefab is null.");
        }
    }



    private void OnGUI()
    {
        //if (GUILayout.Button("Spawn a Random Item"))
        //{
        //    SpawnItemByGUI();
        //}
    }

}
