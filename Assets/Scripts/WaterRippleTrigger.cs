using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRippleTrigger : MonoBehaviour
{
    public ParticleSystem rippleEffect; // ลาก Particle System มาวางใน Inspector
    public float rippleInterval = 0.5f; // ระยะเวลาระหว่างการสร้างรอยกระเพื่อม

    private float rippleTimer = 0f; // ตัวจับเวลา

    private void Update()
    {
        rippleTimer += Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่าเป็นผู้เล่นหรือไม่
        {
            rippleEffect.transform.position = other.transform.position; // ตั้งตำแหน่งของ Particle Effect ให้ตรงกับตำแหน่งของผู้เล่น
            rippleEffect.Play(); // เล่น Particle Effect
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่าเป็นผู้เล่นหรือไม่
        {
            if (rippleTimer >= rippleInterval)
            {
                CreateRipple(other.transform.position);
                rippleTimer = 0f; // รีเซ็ตตัวจับเวลา
            }
        }
    }

    private void CreateRipple(Vector3 position)
    {
        ParticleSystem ripple = Instantiate(rippleEffect, position, Quaternion.identity);
        ripple.Play();
        Destroy(ripple.gameObject, ripple.main.duration); // ทำลาย Particle System หลังจากเล่นเสร็จ
    }
}
