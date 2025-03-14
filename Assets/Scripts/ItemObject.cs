using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemObject : MonoBehaviour
{
    public DataItem item;
    public int amount = 1;
    public TextMeshProUGUI amountText;
    public float pickupRange = 2.0f;

    private void Start()
    {
        UpdateAmountText();
    }

    public void SetAmount(int newAmount)
    {
        amount = newAmount;
        amountText.text = amount.ToString();
    }
    public void RandomAmount()
    {
        amount = Random.Range(1,item.maxStack + 1);
        amountText.text = amount.ToString();
    }

    private void UpdateAmountText()
    {
        if (amountText != null)
        {
            amountText.text = amount.ToString();
        }
    }

    private void OnMouseDown()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= pickupRange)
            {
                PlayerMove playerMove = player.GetComponent<PlayerMove>();
                if (playerMove != null)
                {
                    playerMove.inventory.AddItem(item, amount);
                    Destroy(gameObject); // Remove the item from the scene
                }
                else
                {
                    Debug.LogError("PlayerMove or Inventory component not found on the player.");
                }
            }
            else
            {
                Debug.Log("Player is too far to pick up the item.");
            }
        }
        else
        {
            Debug.LogError("Player not found in the scene.");
        }
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0)) 
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
    //        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
    //        {
    //            if (hit.collider != null && hit.collider.gameObject == gameObject) 
    //            {

    //                GameObject player = GameObject.FindWithTag("Player");
    //                if (player != null)
    //                {
    //                    player.GetComponent<PlayerMove>().inventory.AddItem(item, amount);
    //                    Destroy(gameObject);
    //                }
    //            }
    //        }
    //    }
    //}


    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Player") && Input.GetMouseButtonDown(0))
    //    {
    //        //Add Item
    //        other.GetComponent<PlayerMove>().inventory.AddItem(item, amount);
    //        Destroy(gameObject);
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        //Add Item
    //        other.GetComponent<PlayerMove>().inventory.AddItem(item, amount);
    //        Destroy(gameObject);
    //    }
    //}

}
