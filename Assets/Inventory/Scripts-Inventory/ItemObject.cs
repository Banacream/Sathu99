using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemObject : MonoBehaviour
{
    public DataItem item;
    public int amount = 1;
    public TextMeshProUGUI amountText;

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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetMouseButtonDown(0))
        {
            //Add Item
            other.GetComponent<PlayerMove>().inventory.AddItem(item, amount);
            Destroy(gameObject);
        }
    }

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
