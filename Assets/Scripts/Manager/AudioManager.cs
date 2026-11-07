using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public string soundName;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Tooltip("Centang jika audio ini harus di-loop (misal suara jalan)")]
    public bool loop = false;
}

public class AudioManager : MonoBehaviour
{
    // =========================================================
    // SINGLETON
    // =========================================================

    public static AudioManager Instance;

    // =========================================================
    // VOLUME
    // =========================================================

    private float _globalSFXVolume = 1f;
    private float _globalMusicVolume = 1f;
    private float _currentLoopBaseVolume = 1f;
    private float _currentMusicBaseVolume = 1f;

    // =========================================================
    // AUDIO SOURCES
    // =========================================================

    [Header("Audio Sources")]

    [Tooltip("AudioSource untuk memutar efek suara (SFX)")]
    public AudioSource sfxSource;

    [Tooltip("AudioSource untuk memutar musik (Music/BGM)")]
    public AudioSource musicSource;

    // AudioSource khusus untuk SFX looping
    private AudioSource _loopSfxSource;

    // =========================================================
    // AUDIO LISTS
    // =========================================================

    [Header("Audio Lists")]

    [Tooltip("Daftar audio clip untuk SFX")]
    public List<SoundData> sfxList =
        new List<SoundData>();

    [Tooltip("Daftar audio clip untuk Musik")]
    public List<SoundData> musicList =
        new List<SoundData>();

    // =========================================================
    // CURRENT MUSIC
    // =========================================================

    private string _currentMusicName;

    public string CurrentMusicName =>
        _currentMusicName;

    private string[] _playlist;
    private int _playlistIndex;
    private bool _playlistActive;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        // =====================================================
        // SINGLETON
        // =====================================================

        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            // =================================================
            // LOAD VOLUME
            // =================================================

            _globalSFXVolume =
                PlayerPrefs.GetFloat(
                    "SFXVolume",
                    1f
                );

            _globalMusicVolume =
                PlayerPrefs.GetFloat(
                    "MusicVolume",
                    1f
                );

            if (sfxSource != null)
            {
                sfxSource.volume = 1f;
            }

            if (musicSource != null)
            {
                musicSource.volume =
                    _globalMusicVolume;
            }

            // =================================================
            // LOOP SFX SOURCE
            // =================================================

            _loopSfxSource =
                gameObject.AddComponent<AudioSource>();

            _loopSfxSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!_playlistActive || musicSource == null)
            return;

        if (!musicSource.isPlaying)
            PlayNextInPlaylist();
    }

    // =========================================================
    // PLAY SFX
    // =========================================================

    public void PlaySFX(string name)
    {
        Debug.Log(
            $"PlaySFX dipanggil: {name}"
        );

        SoundData s =
            sfxList.Find(
                x => x.soundName == name
            );

        if (s != null)
        {
            Debug.Log(
                $"SFX ditemukan: {s.soundName}"
            );

            // =================================================
            // LOOPING SFX
            // =================================================

            if (s.loop)
            {
                bool sameClip =
                    _loopSfxSource.clip == s.clip;

                _currentLoopBaseVolume = s.volume;

                _loopSfxSource.volume =
                    s.volume *
                    _globalSFXVolume;

                _loopSfxSource.loop = true;

                if (!sameClip)
                {
                    _loopSfxSource.clip = s.clip;
                    _loopSfxSource.Play();
                }
                else if (!_loopSfxSource.isPlaying)
                {
                    _loopSfxSource.Play();
                }
            }

            // =================================================
            // ONE SHOT SFX
            // =================================================

            else
            {
                if (sfxSource == null)
                {
                    Debug.LogWarning(
                        "[AudioManager] SFX Source belum di-assign!"
                    );

                    return;
                }

                sfxSource.PlayOneShot(
                    s.clip,
                    s.volume *
                    _globalSFXVolume
                );
            }
        }
        else
        {
            Debug.LogWarning(
                $"[AudioManager] SFX '{name}' tidak ditemukan!"
            );
        }
    }

    // =========================================================
    // STOP LOOPING SFX
    // =========================================================

    public void StopSFX()
    {
        if (_loopSfxSource != null)
        {
            _loopSfxSource.Stop();
            _loopSfxSource.loop = false;
        }
    }

    // =========================================================
    // STOP ALL SFX
    // =========================================================

    public void StopAllSFX()
    {
        if (sfxSource != null)
            sfxSource.Stop();

        if (_loopSfxSource != null)
            _loopSfxSource.Stop();
    }

    // =========================================================
    // PLAY MUSIC
    // =========================================================

    public void PlayMusic(string name)
    {
        // =====================================================
        // AUDIO MANAGER CHECK
        // =====================================================

        if (musicSource == null)
        {
            Debug.LogWarning(
                "[AudioManager] Music Source belum di-assign!"
            );

            return;
        }

        // =====================================================
        // CARI MUSIC
        // =====================================================

        SoundData s =
            musicList.Find(
                x => x.soundName == name
            );

        if (s == null)
        {
            Debug.LogWarning(
                $"[AudioManager] Music '{name}' tidak ditemukan!"
            );

            return;
        }

        _playlistActive = false;
        musicSource.loop = true;

        // =====================================================
        // MUSIC SUDAH SAMA
        // =====================================================

        if (_currentMusicName == name &&
            musicSource.clip == s.clip &&
            musicSource.isPlaying)
        {
            // Jangan restart music
            return;
        }

        // =====================================================
        // GANTI MUSIC
        // =====================================================

        _currentMusicName = name;
        _currentMusicBaseVolume = s.volume;

        musicSource.clip = s.clip;

        musicSource.volume =
            s.volume *
            _globalMusicVolume;

        musicSource.Play();

        Debug.Log(
            $"[AudioManager] Playing Music: {name}"
        );
    }

    // =========================================================
    // PLAY PLAYLIST
    // =========================================================

    public void PlayPlaylist(params string[] names)
    {
        if (musicSource == null ||
            names == null ||
            names.Length == 0)
        {
            return;
        }

        if (_playlistActive &&
            musicSource.isPlaying &&
            System.Array.IndexOf(names, _currentMusicName) >= 0)
        {
            _playlist = names;
            return;
        }

        _playlist = names;
        _playlistIndex = -1;
        _playlistActive = true;

        PlayNextInPlaylist();
    }

    private void PlayNextInPlaylist()
    {
        _playlistIndex =
            (_playlistIndex + 1) % _playlist.Length;

        string targetName = _playlist[_playlistIndex];

        SoundData s =
            musicList.Find(
                x => x.soundName == targetName
            );

        if (s == null)
        {
            Debug.LogWarning(
                $"[AudioManager] Music '{targetName}' tidak ditemukan!"
            );

            _playlistActive = false;

            return;
        }

        _currentMusicName = s.soundName;
        _currentMusicBaseVolume = s.volume;

        musicSource.clip = s.clip;

        musicSource.volume =
            s.volume *
            _globalMusicVolume;

        musicSource.loop = false;
        musicSource.Play();

        Debug.Log(
            $"[AudioManager] Playing Music: {targetName}"
        );
    }

    // =========================================================
    // STOP MUSIC
    // =========================================================

    public void StopMusic()
    {
        _playlistActive = false;

        if (musicSource != null)
        {
            musicSource.Stop();
        }

        _currentMusicName = "";
    }

    // =========================================================
    // SFX VOLUME
    // =========================================================

    public void SetSFXVolume(float volume)
    {
        _globalSFXVolume = volume;

        PlayerPrefs.SetFloat(
            "SFXVolume",
            volume
        );

        if (_loopSfxSource != null)
        {
            _loopSfxSource.volume =
                _currentLoopBaseVolume * volume;
        }
    }

    // =========================================================
    // MUSIC VOLUME
    // =========================================================

    public void SetMusicVolume(float volume)
    {
        _globalMusicVolume = volume;

        PlayerPrefs.SetFloat(
            "MusicVolume",
            volume
        );

        if (musicSource != null)
        {
            musicSource.volume =
                _currentMusicBaseVolume * volume;
        }
    }
}