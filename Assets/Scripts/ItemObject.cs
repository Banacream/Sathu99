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


    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ��Ǩ�Ѻ��ä�ԡ��������
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ���ҧ Ray �ҡ���˹������
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject) // ��Ǩ�ͺ��� Ray ���ѵ�ع��
                {
                    // ���������
                    GameObject player = GameObject.FindWithTag("Player");
                    if (player != null)
                    {
                        player.GetComponent<PlayerMove>().inventory.AddItem(item, amount);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }


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
