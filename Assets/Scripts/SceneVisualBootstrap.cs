using UnityEngine;

public class SceneVisualBootstrap : MonoBehaviour
{
    [Header("Camera Baseline")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool enforceOrthographic = true;
    [SerializeField] private Color backgroundColor = new Color(0.09f, 0.11f, 0.14f, 1f);
    [SerializeField] private float orthographicSize = 6f;

    [Header("Warnings")]
    [SerializeField] private bool warnIfNoMainCamera = true;
    [SerializeField] private bool warnIfNoPlayerSprite = true;
    [SerializeField] private bool warnIfCameraFollowMissingTarget = true;

    private void Awake()
    {
        EnsureCameraBaseline();
        ValidateCriticalReferences();
    }

    private void EnsureCameraBaseline()
    {
        Camera cam = ResolveCamera();
        if (cam == null)
        {
            if (warnIfNoMainCamera)
            {
                Debug.LogError("SceneVisualBootstrap: Camera is missing. Add a Camera to the scene and tag it MainCamera.", this);
            }

            return;
        }

        if (enforceOrthographic)
        {
            cam.orthographic = true;
            cam.orthographicSize = Mathf.Clamp(orthographicSize, 5f, 7f);
        }

        cam.backgroundColor = backgroundColor;

        if (warnIfCameraFollowMissingTarget)
        {
            CameraFollow2D follow = cam.GetComponent<CameraFollow2D>();
            if (follow == null)
            {
                Debug.LogWarning("SceneVisualBootstrap: CameraFollow2D is not on the camera. Add it for smooth follow.", cam);
            }
        }
    }

    private void ValidateCriticalReferences()
    {
        if (!warnIfNoPlayerSprite)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogWarning("SceneVisualBootstrap: Player with tag 'Player' not found. Player visual checks were skipped.", this);
            return;
        }

        SpriteRenderer playerRenderer = playerObject.GetComponentInChildren<SpriteRenderer>();
        if (playerRenderer == null)
        {
            Debug.LogWarning("SceneVisualBootstrap: Player has no SpriteRenderer in children.", playerObject);
            return;
        }

        if (playerRenderer.sharedMaterial == null)
        {
            Debug.LogWarning("SceneVisualBootstrap: Player SpriteRenderer has no material assigned. Set Sprites-Default.", playerRenderer);
        }
    }

    private Camera ResolveCamera()
    {
        if (targetCamera != null)
        {
            return targetCamera;
        }

        if (Camera.main != null)
        {
            return Camera.main;
        }

        return FindObjectOfType<Camera>();
    }

    private void OnValidate()
    {
        orthographicSize = Mathf.Clamp(orthographicSize, 5f, 7f);
    }
}
