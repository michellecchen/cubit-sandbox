using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip placementClip;

    [SerializeField]
    private AudioClip removalClip;

    [SerializeField]
    private AudioClip selectionClip;

    public void PlayObjectPlacementSFX() {
        _audioSource.clip = placementClip;
        _audioSource.Play();
    }

    public void PlayObjectRemovalSFX() {
        _audioSource.clip = removalClip;
        _audioSource.Play();
    }

    public void PlayObjectSelectionSFX() {
        _audioSource.clip = selectionClip;
        _audioSource.Play();
    }
}
