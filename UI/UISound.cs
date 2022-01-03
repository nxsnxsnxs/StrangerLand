using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UISound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!clickSound) return;
        audioSource.clip = clickSound;
        audioSource.Play();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!hoverSound) return;
        audioSource.clip = hoverSound;
        audioSource.Play();
    }
}
