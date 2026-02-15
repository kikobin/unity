using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PngThemeRuntimeApplier : MonoBehaviour
{
    private const string ThemeResourcePath = "PngThemeProfile";

    private struct RendererBaseline
    {
        public Vector3 localScale;
        public Vector2 spriteSize;
    }

    private struct UiBaseline
    {
        public Vector3 localScale;
    }

    private static bool isBootstrapped;

    private readonly Dictionary<int, RendererBaseline> baselines = new Dictionary<int, RendererBaseline>();
    private readonly Dictionary<int, UiBaseline> uiBaselines = new Dictionary<int, UiBaseline>();

    private bool hasLoggedMissingProfile;
    private PngThemeProfile profile;
    private float nextDynamicRefreshTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (isBootstrapped)
        {
            return;
        }

        isBootstrapped = true;
        GameObject host = new GameObject("PngThemeRuntimeApplier");
        DontDestroyOnLoad(host);
        host.AddComponent<PngThemeRuntimeApplier>();
    }

    private void Awake()
    {
        LoadProfile();
        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyThemeToScene(SceneManager.GetActiveScene());
        nextDynamicRefreshTime = Time.unscaledTime + GetRefreshInterval();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (profile == null)
        {
            LoadProfile();
            return;
        }

        // Projectiles spawn frequently, so apply every frame to avoid visible placeholder flashes.
        ApplyProjectileSprites(profile.fireballSprite, profile.FireballScaleMultiplier);
        // Enemies can spawn with placeholder sprite for a frame; enforce replacement every frame.
        ApplyEnemySprites(profile.enemySprite, profile.EnemyScaleMultiplier);

        if (Time.unscaledTime < nextDynamicRefreshTime)
        {
            return;
        }

        ApplyPlayerSprite(profile.playerSprite, profile.PlayerScaleMultiplier);
        nextDynamicRefreshTime = Time.unscaledTime + GetRefreshInterval();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadProfile();
        ApplyThemeToScene(scene);
        nextDynamicRefreshTime = Time.unscaledTime + GetRefreshInterval();
    }

    private void LoadProfile()
    {
        profile = Resources.Load<PngThemeProfile>(ThemeResourcePath);
        if (profile != null)
        {
            hasLoggedMissingProfile = false;
            return;
        }

        if (!hasLoggedMissingProfile)
        {
            hasLoggedMissingProfile = true;
            Debug.LogWarning(
                "PngThemeRuntimeApplier: PngThemeProfile not found at Resources/PngThemeProfile. " +
                "Create it via Tools/Art/Create PNG Theme Profile.");
        }
    }

    private void ApplyThemeToScene(Scene scene)
    {
        if (profile == null || !scene.IsValid())
        {
            return;
        }

        ApplySceneBackground(scene, ResolveBackgroundForScene(scene.name));
        ApplyButtonSprites(scene);
        ApplyDynamicGameplaySprites();
    }

    private void ApplySceneBackground(Scene scene, Sprite backgroundSprite)
    {
        if (backgroundSprite == null)
        {
            return;
        }

        bool isGameScene = scene.name.Equals("Game", StringComparison.OrdinalIgnoreCase);
        if (isGameScene)
        {
            ApplyWorldBackground(scene, backgroundSprite);
            DisableUiBackgroundCandidates(scene);
            return;
        }

        Image backgroundImage = FindBackgroundImage(scene);
        if (backgroundImage == null)
        {
            backgroundImage = CreateBackgroundImage(scene);
        }

        if (backgroundImage == null)
        {
            return;
        }

        backgroundImage.sprite = backgroundSprite;
        backgroundImage.type = Image.Type.Simple;
        backgroundImage.preserveAspect = false;
        backgroundImage.raycastTarget = false;
    }

    private void ApplyWorldBackground(Scene scene, Sprite sprite)
    {
        Camera cam = ResolveSceneCamera(scene);
        if (cam == null)
        {
            return;
        }

        GameObject bg = FindOrCreateWorldBackground(scene);
        if (bg == null)
        {
            return;
        }

        SpriteRenderer renderer = bg.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = bg.AddComponent<SpriteRenderer>();
        }

        renderer.drawMode = SpriteDrawMode.Simple;
        renderer.sprite = sprite;
        renderer.sortingOrder = -10000;

        Transform t = bg.transform;
        t.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        t.rotation = Quaternion.identity;

        Vector2 spriteSize = GetSpriteSizeOrFallback(sprite);
        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;
        t.localScale = new Vector3(worldWidth / spriteSize.x, worldHeight / spriteSize.y, 1f);
    }

    private static GameObject FindOrCreateWorldBackground(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            GameObject root = roots[i];
            if (root == null)
            {
                continue;
            }

            if (root.name.Equals("WorldBackground", StringComparison.OrdinalIgnoreCase))
            {
                return root;
            }
        }

        GameObject created = new GameObject("WorldBackground");
        SceneManager.MoveGameObjectToScene(created, scene);
        return created;
    }

    private static Camera ResolveSceneCamera(Scene scene)
    {
        if (!scene.IsValid())
        {
            return null;
        }

        Camera[] allCameras = FindObjectsOfType<Camera>();
        for (int i = 0; i < allCameras.Length; i++)
        {
            Camera cam = allCameras[i];
            if (cam == null)
            {
                continue;
            }

            if (cam.gameObject.scene == scene)
            {
                return cam;
            }
        }

        return Camera.main;
    }

    private static void DisableUiBackgroundCandidates(Scene scene)
    {
        List<Image> images = CollectSceneComponents<Image>(scene);
        for (int i = 0; i < images.Count; i++)
        {
            Image image = images[i];
            if (image == null || image.GetComponent<Button>() != null)
            {
                continue;
            }

            string objectName = image.gameObject.name;
            if (objectName.Equals("BG_main", StringComparison.OrdinalIgnoreCase) ||
                objectName.IndexOf("background", StringComparison.OrdinalIgnoreCase) >= 0 ||
                objectName.IndexOf("bg", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                image.enabled = false;
            }
        }
    }

    private void ApplyButtonSprites(Scene scene)
    {
        List<Button> buttons = CollectSceneComponents<Button>(scene);
        List<RectTransform> appliedButtonRects = new List<RectTransform>();
        for (int i = 0; i < buttons.Count; i++)
        {
            Button button = buttons[i];
            if (button == null)
            {
                continue;
            }

            Image image = button.targetGraphic as Image;
            if (image == null)
            {
                image = button.GetComponent<Image>();
            }

            if (image == null)
            {
                continue;
            }

            if (profile.UseDefaultStyledButtons)
            {
                ApplyDefaultButtonStyle(button, image);
                float defaultScale = ResolveButtonScale(button.name);
                ApplyButtonScale(image.rectTransform, defaultScale);
                SetButtonLabelEnabled(button, true);
                appliedButtonRects.Add(image.rectTransform);
                continue;
            }

            Sprite buttonSprite = ResolveButtonSprite(button.name);
            float buttonScale = ResolveButtonScale(button.name);
            if (buttonSprite == null)
            {
                if (profile.HideDefaultButtonGraphics)
                {
                    image.enabled = false;
                }

                continue;
            }

            image.enabled = true;
            image.sprite = buttonSprite;
            image.type = Image.Type.Sliced;

            SpriteState state = button.spriteState;
            state.highlightedSprite = buttonSprite;
            state.pressedSprite = buttonSprite;
            state.selectedSprite = buttonSprite;
            state.disabledSprite = buttonSprite;
            button.spriteState = state;

            ApplyButtonScale(image.rectTransform, buttonScale);
            SetButtonLabelEnabled(button, !profile.HideButtonLabels);
            appliedButtonRects.Add(image.rectTransform);
        }

        ApplyMinimumHorizontalGap(appliedButtonRects);
    }

    private static void ApplyDefaultButtonStyle(Button button, Image image)
    {
        image.enabled = true;
        image.type = image.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
        image.color = new Color(0.18f, 0.22f, 0.30f, 0.9f);

        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.20f, 0.24f, 0.32f, 1f);
        colors.highlightedColor = new Color(0.28f, 0.33f, 0.42f, 1f);
        colors.pressedColor = new Color(0.12f, 0.15f, 0.22f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.25f, 0.25f, 0.25f, 0.55f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;
    }

    private void ApplyDynamicGameplaySprites()
    {
        if (profile == null)
        {
            return;
        }

        ApplyPlayerSprite(profile.playerSprite, profile.PlayerScaleMultiplier);
        ApplyEnemySprites(profile.enemySprite, profile.EnemyScaleMultiplier);
        ApplyProjectileSprites(profile.fireballSprite, profile.FireballScaleMultiplier);
    }

    private void ApplyPlayerSprite(Sprite playerSprite, float multiplier)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        SpriteRenderer renderer = player.GetComponentInChildren<SpriteRenderer>();
        if (playerSprite == null)
        {
            ApplyMissingRendererRule(renderer);
            return;
        }

        ApplyRendererSprite(renderer, playerSprite, multiplier);
    }

    private void ApplyEnemySprites(Sprite enemySprite, float multiplier)
    {
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyHealth enemy = enemies[i];
            if (enemy == null)
            {
                continue;
            }

            SpriteRenderer renderer = enemy.GetComponentInChildren<SpriteRenderer>();
            if (enemySprite == null)
            {
                ApplyMissingRendererRule(renderer);
                continue;
            }

            ApplyRendererSprite(renderer, enemySprite, multiplier);
        }
    }

    private void ApplyProjectileSprites(Sprite fireballSprite, float multiplier)
    {
        Projectile2D[] projectiles = FindObjectsOfType<Projectile2D>();
        for (int i = 0; i < projectiles.Length; i++)
        {
            Projectile2D projectile = projectiles[i];
            if (projectile == null)
            {
                continue;
            }

            SpriteRenderer renderer = projectile.GetComponentInChildren<SpriteRenderer>();
            if (fireballSprite == null)
            {
                ApplyMissingRendererRule(renderer);
                continue;
            }

            ApplyRendererSprite(renderer, fireballSprite, multiplier);
        }
    }

    private void ApplyRendererSprite(SpriteRenderer renderer, Sprite sprite, float multiplier)
    {
        if (renderer == null || sprite == null)
        {
            return;
        }

        int id = renderer.GetInstanceID();
        if (!baselines.ContainsKey(id))
        {
            baselines[id] = new RendererBaseline
            {
                localScale = renderer.transform.localScale,
                spriteSize = GetSpriteSizeOrFallback(renderer.sprite)
            };
        }

        RendererBaseline baseline = baselines[id];

        renderer.drawMode = SpriteDrawMode.Simple;
        renderer.enabled = true;
        renderer.sprite = sprite;

        Vector3 targetScale = baseline.localScale;

        if (profile != null && profile.PreserveOriginalWorldSize)
        {
            Vector2 newSize = GetSpriteSizeOrFallback(sprite);
            if (newSize.x > 0.0001f && newSize.y > 0.0001f)
            {
                targetScale.x = baseline.localScale.x * (baseline.spriteSize.x / newSize.x);
                targetScale.y = baseline.localScale.y * (baseline.spriteSize.y / newSize.y);
            }
        }

        float safeMultiplier = Mathf.Clamp(multiplier, 0.05f, 10f);
        targetScale.x *= safeMultiplier;
        targetScale.y *= safeMultiplier;

        renderer.transform.localScale = targetScale;

        ProjectileVisual projectileVisual = renderer.GetComponent<ProjectileVisual>();
        if (projectileVisual != null)
        {
            projectileVisual.SetBaseScale(targetScale);
        }
    }

    private void ApplyMissingRendererRule(SpriteRenderer renderer)
    {
        if (renderer == null || profile == null)
        {
            return;
        }

        if (profile.HidePrimitivePlaceholderSprites && IsPrimitivePlaceholderSprite(renderer.sprite))
        {
            renderer.enabled = false;
            return;
        }

        if (profile.HideUnassignedGameplayPlaceholders)
        {
            renderer.enabled = false;
        }
    }

    private static bool IsPrimitivePlaceholderSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            return false;
        }

        string spriteName = sprite.name ?? string.Empty;
        return spriteName.Equals("Square", StringComparison.OrdinalIgnoreCase) ||
               spriteName.Equals("Circle", StringComparison.OrdinalIgnoreCase) ||
               spriteName.Equals("UI/Skin/Background", StringComparison.OrdinalIgnoreCase) ||
               spriteName.IndexOf("Knob", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private Sprite ResolveButtonSprite(string buttonName)
    {
        if (profile == null)
        {
            return null;
        }

        string normalized = buttonName ?? string.Empty;
        if (normalized.IndexOf("start", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return profile.startButtonSprite != null ? profile.startButtonSprite : profile.buttonSprite;
        }

        if (normalized.IndexOf("quit", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return profile.quitButtonSprite != null ? profile.quitButtonSprite : profile.buttonSprite;
        }

        if (normalized.IndexOf("restart", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return profile.restartButtonSprite != null ? profile.restartButtonSprite : profile.buttonSprite;
        }

        if (normalized.IndexOf("mainmenu", StringComparison.OrdinalIgnoreCase) >= 0 ||
            normalized.IndexOf("main menu", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return profile.mainMenuButtonSprite != null ? profile.mainMenuButtonSprite : profile.buttonSprite;
        }

        return profile.buttonSprite;
    }

    private float ResolveButtonScale(string buttonName)
    {
        if (profile == null)
        {
            return 1f;
        }

        string normalized = buttonName ?? string.Empty;
        float baseScale = profile.ButtonScaleMultiplier;
        float value = baseScale;

        if (normalized.IndexOf("start", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            value = baseScale * profile.StartButtonScaleMultiplier;
        }
        else if (normalized.IndexOf("quit", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            value = baseScale * profile.QuitButtonScaleMultiplier;
        }
        else if (normalized.IndexOf("restart", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            value = baseScale * profile.RestartButtonScaleMultiplier;
        }
        else if (normalized.IndexOf("mainmenu", StringComparison.OrdinalIgnoreCase) >= 0 ||
            normalized.IndexOf("main menu", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            value = baseScale * profile.MainMenuButtonScaleMultiplier;
        }

        if (profile.UseDefaultStyledButtons)
        {
            return Mathf.Clamp(value, 0.75f, 1.25f);
        }

        return value;
    }

    private void ApplyButtonScale(RectTransform rectTransform, float scaleMultiplier)
    {
        if (rectTransform == null)
        {
            return;
        }

        int id = rectTransform.GetInstanceID();
        if (!uiBaselines.ContainsKey(id))
        {
            uiBaselines[id] = new UiBaseline
            {
                localScale = rectTransform.localScale
            };
        }

        UiBaseline baseline = uiBaselines[id];
        float safe = Mathf.Clamp(scaleMultiplier, 0.1f, 5f);
        rectTransform.localScale = baseline.localScale * safe;
    }

    private static void SetButtonLabelEnabled(Button button, bool enabled)
    {
        if (button == null)
        {
            return;
        }

        TextMeshProUGUI[] tmpLabels = button.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < tmpLabels.Length; i++)
        {
            if (tmpLabels[i] != null)
            {
                tmpLabels[i].enabled = enabled;
                if (enabled)
                {
                    tmpLabels[i].color = Color.white;
                }
            }
        }

        Text[] legacyLabels = button.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < legacyLabels.Length; i++)
        {
            if (legacyLabels[i] != null)
            {
                legacyLabels[i].enabled = enabled;
                if (enabled)
                {
                    legacyLabels[i].color = Color.white;
                }
            }
        }
    }

    private void ApplyMinimumHorizontalGap(List<RectTransform> rects)
    {
        if (profile == null || rects == null || rects.Count < 2)
        {
            return;
        }

        float minGap = profile.MinimumButtonGap;
        if (minGap <= 0f)
        {
            return;
        }

        rects.Sort((a, b) =>
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;
            return a.anchoredPosition.x.CompareTo(b.anchoredPosition.x);
        });

        for (int i = 1; i < rects.Count; i++)
        {
            RectTransform left = rects[i - 1];
            RectTransform right = rects[i];
            if (left == null || right == null)
            {
                continue;
            }

            if (left.parent != right.parent)
            {
                continue;
            }

            float rowDelta = Mathf.Abs(left.anchoredPosition.y - right.anchoredPosition.y);
            if (rowDelta > 30f)
            {
                continue;
            }

            float leftHalf = Mathf.Abs(left.rect.width * left.localScale.x) * 0.5f;
            float rightHalf = Mathf.Abs(right.rect.width * right.localScale.x) * 0.5f;
            float centerDistance = right.anchoredPosition.x - left.anchoredPosition.x;
            float edgeGap = centerDistance - (leftHalf + rightHalf);

            if (edgeGap >= minGap)
            {
                continue;
            }

            float requiredShift = minGap - edgeGap;
            Vector2 leftPos = left.anchoredPosition;
            Vector2 rightPos = right.anchoredPosition;
            leftPos.x -= requiredShift * 0.5f;
            rightPos.x += requiredShift * 0.5f;
            left.anchoredPosition = leftPos;
            right.anchoredPosition = rightPos;
        }
    }

    private static Vector2 GetSpriteSizeOrFallback(Sprite sprite)
    {
        if (sprite == null)
        {
            return Vector2.one;
        }

        Vector2 size = sprite.bounds.size;
        if (size.x <= 0.0001f || size.y <= 0.0001f)
        {
            return Vector2.one;
        }

        return size;
    }

    private Image FindBackgroundImage(Scene scene)
    {
        List<Image> images = CollectSceneComponents<Image>(scene);
        Image named = null;

        for (int i = 0; i < images.Count; i++)
        {
            Image image = images[i];
            if (image == null || image.GetComponent<Button>() != null)
            {
                continue;
            }

            string objectName = image.gameObject.name;
            if (objectName.Equals("BG_main", StringComparison.OrdinalIgnoreCase) ||
                objectName.IndexOf("background", StringComparison.OrdinalIgnoreCase) >= 0 ||
                objectName.IndexOf("bg", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return image;
            }

            if (named == null)
            {
                named = image;
            }
        }

        return named;
    }

    private Image CreateBackgroundImage(Scene scene)
    {
        List<Canvas> canvases = CollectSceneComponents<Canvas>(scene);
        if (canvases.Count == 0)
        {
            return null;
        }

        Canvas targetCanvas = canvases[0];
        if (targetCanvas == null)
        {
            return null;
        }

        GameObject bgObject = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bgObject.transform.SetParent(targetCanvas.transform, false);
        bgObject.transform.SetAsFirstSibling();

        RectTransform rect = bgObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return bgObject.GetComponent<Image>();
    }

    private Sprite ResolveBackgroundForScene(string sceneName)
    {
        if (profile == null)
        {
            return null;
        }

        if (sceneName.Equals("MainMenu", StringComparison.OrdinalIgnoreCase))
        {
            return profile.mainMenuBackground;
        }

        if (sceneName.Equals("Game", StringComparison.OrdinalIgnoreCase))
        {
            return profile.gameBackground;
        }

        if (sceneName.Equals("Results", StringComparison.OrdinalIgnoreCase))
        {
            return profile.resultsBackground;
        }

        return null;
    }

    private static List<T> CollectSceneComponents<T>(Scene scene) where T : Component
    {
        List<T> result = new List<T>();
        if (!scene.IsValid())
        {
            return result;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            GameObject root = roots[i];
            if (root == null)
            {
                continue;
            }

            T[] components = root.GetComponentsInChildren<T>(true);
            for (int j = 0; j < components.Length; j++)
            {
                result.Add(components[j]);
            }
        }

        return result;
    }

    private float GetRefreshInterval()
    {
        if (profile == null)
        {
            return 0.5f;
        }

        return profile.DynamicRefreshInterval;
    }
}
