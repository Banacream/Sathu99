using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----------- Audio Source -----------")]
    [SerializeField] AudioSource musicSource01;
    [SerializeField] AudioSource musicSource02;
    [SerializeField] AudioSource footStepsmusicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("------------ Audio Clip -----------")]
    public AudioClip background01;
    public AudioClip background02;
    public AudioClip attackPlayer;
    public AudioClip playerDamage;
    public AudioClip footSteps;

    private void Start()
    {
        musicSource01.clip = background01;
        musicSource02.clip = background02;
        //footStepsmusicSource.clip = footSteps;
        musicSource01.Play();
        musicSource02.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
    public void PlayfootStepsmusicSource(AudioClip clip)
    {
        footStepsmusicSource.clip = footSteps;
        footStepsmusicSource.Play();
    }

    public bool IsPlaying()
    {
        return footStepsmusicSource.isPlaying;
    }
    public void StopSFX()
    {
        footStepsmusicSource.Stop();
    }
}
