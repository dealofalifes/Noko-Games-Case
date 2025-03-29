using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource _GeneralSoundsSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _CollectItemClip;
    [SerializeField] private AudioClip _DropItemClip;
    [SerializeField] private AudioClip _ProduceItemClip;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayCollectItemSound()
    {
        _GeneralSoundsSource.PlayOneShot(_CollectItemClip);
    }

    public void PlayCollectItemSound(AudioSource _mySource)
    {
        _mySource.PlayOneShot(_CollectItemClip);
    }

    public void PlayDropItemSound()
    {
        _GeneralSoundsSource.PlayOneShot(_DropItemClip);
    }

    public void PlayDropItemSound(AudioSource _mySource)
    {
        _mySource.PlayOneShot(_DropItemClip);
    }

    public void PlayProduceItemSound()
    {
        _GeneralSoundsSource.PlayOneShot(_ProduceItemClip);
    }

    public void PlayProduceItemSound(AudioSource _mySource)
    {
        _mySource.PlayOneShot(_ProduceItemClip);
    }
}
