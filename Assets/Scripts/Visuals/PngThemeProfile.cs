using UnityEngine;

[CreateAssetMenu(fileName = "PngThemeProfile", menuName = "Game/Visuals/PNG Theme Profile")]
public class PngThemeProfile : ScriptableObject
{
    [Header("Scene Backgrounds")]
    public Sprite mainMenuBackground;
    public Sprite gameBackground;
    public Sprite resultsBackground;

    [Header("UI")]
    public Sprite buttonSprite;
    public Sprite startButtonSprite;
    public Sprite quitButtonSprite;
    public Sprite restartButtonSprite;
    public Sprite mainMenuButtonSprite;
    [SerializeField] private bool useDefaultStyledButtons = true;
    [SerializeField] private bool hideDefaultButtonGraphics = false;
    [SerializeField] private bool hideButtonLabels = true;
    [SerializeField] private float minimumButtonGap = 24f;
    [SerializeField] private float buttonScaleMultiplier = 1f;
    [SerializeField] private float startButtonScaleMultiplier = 1f;
    [SerializeField] private float quitButtonScaleMultiplier = 1f;
    [SerializeField] private float restartButtonScaleMultiplier = 1f;
    [SerializeField] private float mainMenuButtonScaleMultiplier = 1f;

    [Header("Gameplay")]
    public Sprite playerSprite;
    public Sprite enemySprite;
    public Sprite fireballSprite;
    [SerializeField] private bool hideUnassignedGameplayPlaceholders = false;
    [SerializeField] private bool hidePrimitivePlaceholderSprites = true;

    [Header("Gameplay Scale")]
    [SerializeField] private bool preserveOriginalWorldSize = true;
    [SerializeField] private float playerScaleMultiplier = 1f;
    [SerializeField] private float enemyScaleMultiplier = 1f;
    [SerializeField] private float fireballScaleMultiplier = 1f;

    [Header("Runtime")]
    [SerializeField] private float dynamicRefreshInterval = 0.5f;

    public bool PreserveOriginalWorldSize => preserveOriginalWorldSize;
    public bool UseDefaultStyledButtons => useDefaultStyledButtons;
    public bool HideDefaultButtonGraphics => hideDefaultButtonGraphics;
    public bool HideButtonLabels => hideButtonLabels;
    public float MinimumButtonGap => Mathf.Clamp(minimumButtonGap, 0f, 300f);
    public bool HideUnassignedGameplayPlaceholders => hideUnassignedGameplayPlaceholders;
    public bool HidePrimitivePlaceholderSprites => hidePrimitivePlaceholderSprites;
    public float ButtonScaleMultiplier => Mathf.Clamp(buttonScaleMultiplier, 0.1f, 5f);
    public float StartButtonScaleMultiplier => Mathf.Clamp(startButtonScaleMultiplier, 0.1f, 5f);
    public float QuitButtonScaleMultiplier => Mathf.Clamp(quitButtonScaleMultiplier, 0.1f, 5f);
    public float RestartButtonScaleMultiplier => Mathf.Clamp(restartButtonScaleMultiplier, 0.1f, 5f);
    public float MainMenuButtonScaleMultiplier => Mathf.Clamp(mainMenuButtonScaleMultiplier, 0.1f, 5f);
    public float PlayerScaleMultiplier => Mathf.Clamp(playerScaleMultiplier, 0.05f, 10f);
    public float EnemyScaleMultiplier => Mathf.Clamp(enemyScaleMultiplier, 0.05f, 10f);
    public float FireballScaleMultiplier => Mathf.Clamp(fireballScaleMultiplier, 0.05f, 10f);
    public float DynamicRefreshInterval => Mathf.Clamp(dynamicRefreshInterval, 0.1f, 2f);
}
