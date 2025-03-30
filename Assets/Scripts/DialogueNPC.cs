using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueNPC : MonoBehaviour
{
    private const string DialogueNPCShownKey = "DialogueNPC_ShownKey";
    public GameObject dialoguePanel;
    public GameObject tutorialPanel;
    public GameObject nextButton;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue;
    private int index;

    public float wordSpeed;
    public bool playerIsClose;

    public AudioSource audioSource; // เพิ่ม AudioSource สำหรับเล่นเสียง
    public AudioClip dialogueSound; // เพิ่ม AudioClip สำหรับเสียง

    void Start()
    {
       
    }




    private void Update()
    {
        if(dialogueText.text == dialogue[index])
        {
            nextButton.SetActive(true);
        }

    }


    public void ShowDialogue()
    {
        dialoguePanel.SetActive(false);

        // ตรวจสอบว่าข้อความถูกแสดงไปแล้วหรือยัง
        int dialogueShownCount = PlayerPrefs.GetInt(DialogueNPCShownKey, 0);
        if (dialogueShownCount < 1) // แสดงได้ไม่เกิน 2 ครั้ง
        {
            tutorialPanel.SetActive(false);
            StartCoroutine(ShowDialogueAfterDelay(5f)); // แสดงข้อความหลังจาก 5 วินาที
            PlayerPrefs.SetInt(DialogueNPCShownKey, dialogueShownCount + 1); // เพิ่มตัวนับ
            PlayerPrefs.Save(); // บันทึกสถานะลงใน PlayerPrefs
        }
    }
    private IEnumerator ShowDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(4f); // รอ 3 วินาที
        PlaySound(); // เล่นเสียงหลังจาก 3 วินาที
        yield return new WaitForSeconds(delay- 4f); // รอเวลาที่กำหนด
        dialoguePanel.SetActive(true); // แสดง Dialogue Panel
        StartCoroutine(TypeDialogue()); // เริ่มพิมพ์ข้อความทีละตัวอักษร
    }

    private void PlaySound()
    {
        if (audioSource != null && dialogueSound != null)
        {
            audioSource.PlayOneShot(dialogueSound); // เล่นเสียง
        }
    }

    private IEnumerator TypeDialogue()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter; // เพิ่มตัวอักษรทีละตัว
            yield return new WaitForSeconds(wordSpeed); // รอเวลาตาม wordSpeed
        }
    }

    public void NextLine()
    {
        nextButton.SetActive(false);

        if(index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text="";
            StartCoroutine(TypeDialogue());
        }
        else
        {
            zeroText();
        }
    }

    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }
}
