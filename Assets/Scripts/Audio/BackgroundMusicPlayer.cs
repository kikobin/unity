using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    private const string MusicResourcePath = "Audio/background_music";

    private static BackgroundMusicPlayer instance;

    [Header("Playback")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField, Range(0f, 1f)] private float volume = 0.35f;
    [SerializeField] private bool playOnStart = true;

    private AudioSource musicSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null)
        {
            return;
        }

        BackgroundMusicPlayer existing = FindObjectOfType<BackgroundMusicPlayer>();
        if (existing != null)
        {
            instance = existing;
            instance.InitializeIfNeeded();
            return;
        }

        GameObject bootstrapObject = new GameObject(nameof(BackgroundMusicPlayer));
        instance = bootstrapObject.AddComponent<BackgroundMusicPlayer>();
        instance.InitializeIfNeeded();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        InitializeIfNeeded();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void InitializeIfNeeded()
    {
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.volume = Mathf.Clamp01(volume);

        if (backgroundMusic == null)
        {
            backgroundMusic = Resources.Load<AudioClip>(MusicResourcePath);
        }

        if (backgroundMusic == null)
        {
            Debug.LogWarning(
                $"BackgroundMusicPlayer: clip not found at Resources/{MusicResourcePath}. " +
                "Add an AudioClip there to enable background music.");
            return;
        }

        if (musicSource.clip != backgroundMusic)
        {
            musicSource.clip = backgroundMusic;
        }

        if (playOnStart && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
}
