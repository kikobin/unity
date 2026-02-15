using System;
using System.Collections.Generic;
using UnityEngine;

public enum SfxType
{
    Shoot = 0,
    Hit = 1,
    Die = 2,
    UIClick = 3
}

[Serializable]
public struct SfxEntry
{
    public SfxType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Scene Setup")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private bool dontDestroyOnLoad;

    [Header("SFX")]
    [SerializeField] private SfxEntry[] sfxEntries = new SfxEntry[0];

    private readonly Dictionary<SfxType, SfxEntry> sfxMap = new Dictionary<SfxType, SfxEntry>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            Debug.LogError("AudioManager.Awake: AudioSource is missing. SFX playback is disabled.", this);
        }

        RebuildSfxMap();
    }

    private void OnValidate()
    {
        for (int i = 0; i < sfxEntries.Length; i++)
        {
            SfxEntry entry = sfxEntries[i];
            entry.volume = Mathf.Clamp01(entry.volume);
            sfxEntries[i] = entry;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlaySfx(SfxType type)
    {
        if (sfxSource == null)
        {
            Debug.LogError($"AudioManager.PlaySfx: AudioSource is missing. Requested '{type}'.", this);
            return;
        }

        if (!TryGetEntry(type, out SfxEntry entry))
        {
            Debug.LogWarning($"AudioManager.PlaySfx: Clip for '{type}' is not assigned.", this);
            return;
        }

        if (entry.clip == null)
        {
            Debug.LogWarning($"AudioManager.PlaySfx: Clip for '{type}' is null in Inspector.", this);
            return;
        }

        float volume = entry.volume <= 0f ? 1f : entry.volume;
        sfxSource.PlayOneShot(entry.clip, volume);
    }

    private bool TryGetEntry(SfxType type, out SfxEntry entry)
    {
        if (sfxMap.Count != sfxEntries.Length)
        {
            RebuildSfxMap();
        }

        return sfxMap.TryGetValue(type, out entry);
    }

    private void RebuildSfxMap()
    {
        sfxMap.Clear();

        for (int i = 0; i < sfxEntries.Length; i++)
        {
            SfxEntry entry = sfxEntries[i];
            sfxMap[entry.type] = entry;
        }
    }
}
