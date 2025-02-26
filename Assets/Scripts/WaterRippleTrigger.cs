using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRippleTrigger : MonoBehaviour
{
    public ParticleSystem rippleEffect; // �ҡ Particle System ���ҧ� Inspector
    public float rippleInterval = 0.5f; // �������������ҧ������ҧ��¡�������

    private float rippleTimer = 0f; // ��ǨѺ����

    private void Update()
    {
        rippleTimer += Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ��Ǩ�ͺ����繼������������
        {
            rippleEffect.transform.position = other.transform.position; // ��駵��˹觢ͧ Particle Effect ���ç�Ѻ���˹觢ͧ������
            rippleEffect.Play(); // ��� Particle Effect
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // ��Ǩ�ͺ����繼������������
        {
            if (rippleTimer >= rippleInterval)
            {
                CreateRipple(other.transform.position);
                rippleTimer = 0f; // ���絵�ǨѺ����
            }
        }
    }

    private void CreateRipple(Vector3 position)
    {
        ParticleSystem ripple = Instantiate(rippleEffect, position, Quaternion.identity);
        ripple.Play();
        Destroy(ripple.gameObject, ripple.main.duration); // ����� Particle System ��ѧ�ҡ�������
    }
}
