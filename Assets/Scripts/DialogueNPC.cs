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

    public AudioSource audioSource; // ���� AudioSource ����Ѻ������§
    public AudioClip dialogueSound; // ���� AudioClip ����Ѻ���§

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

        // ��Ǩ�ͺ��Ң�ͤ����١�ʴ�����������ѧ
        int dialogueShownCount = PlayerPrefs.GetInt(DialogueNPCShownKey, 0);
        if (dialogueShownCount < 1) // �ʴ�������Թ 2 ����
        {
            tutorialPanel.SetActive(false);
            StartCoroutine(ShowDialogueAfterDelay(5f)); // �ʴ���ͤ�����ѧ�ҡ 5 �Թҷ�
            PlayerPrefs.SetInt(DialogueNPCShownKey, dialogueShownCount + 1); // ������ǹѺ
            PlayerPrefs.Save(); // �ѹ�֡ʶҹ�ŧ� PlayerPrefs
        }
    }
    private IEnumerator ShowDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(4f); // �� 3 �Թҷ�
        PlaySound(); // ������§��ѧ�ҡ 3 �Թҷ�
        yield return new WaitForSeconds(delay- 4f); // �����ҷ���˹�
        dialoguePanel.SetActive(true); // �ʴ� Dialogue Panel
        StartCoroutine(TypeDialogue()); // �����������ͤ������е���ѡ��
    }

    private void PlaySound()
    {
        if (audioSource != null && dialogueSound != null)
        {
            audioSource.PlayOneShot(dialogueSound); // ������§
        }
    }

    private IEnumerator TypeDialogue()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter; // ��������ѡ�÷��е��
            yield return new WaitForSeconds(wordSpeed); // �����ҵ�� wordSpeed
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
