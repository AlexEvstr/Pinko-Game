using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    private AudioSource _soundManager;
    [SerializeField] private AudioClip _moveSound;
    [SerializeField] private AudioClip _tapOnPieceSound;
    [SerializeField] private AudioClip _loseSound;
    [SerializeField] private AudioClip _winSound;

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

    public void PlayLoseSound()
    {
        _soundManager.PlayOneShot(_loseSound);
    }

    public void PlayWinSound()
    {
        _soundManager.PlayOneShot(_winSound);
    }
}
