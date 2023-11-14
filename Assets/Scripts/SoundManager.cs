using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singleton

    public static SoundManager              instance;

    #endregion

    #region Inspector Variables

    [SerializeField] private AudioSource    bgSource;
    [SerializeField] private AudioSource    clickSource;

    [SerializeField] private AudioClip      bgMusic;
    [SerializeField] private AudioClip      clickSound;

    [SerializeField] private Texture2D      musicOnTexture;
    [SerializeField] private Texture2D      musicOffTexture;
    [SerializeField] private Texture2D      sfxOnTexture;
    [SerializeField] private Texture2D      sfxOffTexture;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        bgSource.PlayOneShot(bgMusic);
    }

    #endregion

    #region Public Functions

    public void PlayClickSound()
    {
        clickSource.PlayOneShot(clickSound);
    }

    public Texture2D ToggleSFX()
    {
        clickSource.volume = clickSource.volume == 0f ? 1f : 0f;

        return clickSource.volume == 0f ? sfxOffTexture : sfxOnTexture;
    }

    public Texture2D ToggleMusic()
    {
        bgSource.volume = bgSource.volume == 0f ? 1f : 0f;

        return bgSource.volume == 0f ? musicOffTexture : musicOnTexture;
    }

    #endregion
}
