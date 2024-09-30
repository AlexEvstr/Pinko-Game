using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    private AudioSource _soundManager;
    [SerializeField] private AudioClip _moveSound;
    [SerializeField] private AudioClip _tapOnPieceSound;

    private void Start()
    {
        _soundManager = GetComponent<AudioSource>();
    }

    public void PlayMoveSound()
    {
        _soundManager.PlayOneShot(_moveSound);
    }

    public void TapOnPieceSound()
    {
        _soundManager.PlayOneShot(_tapOnPieceSound);
    }
}
