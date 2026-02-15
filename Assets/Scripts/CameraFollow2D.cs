using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followLerpSpeed = 8f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Arena Clamp (Optional)")]
    [SerializeField] private bool useClampBounds = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-12f, -7f);
    [SerializeField] private Vector2 maxBounds = new Vector2(12f, 7f);

    private Camera cam;
    private bool warnedNoTarget;
    private bool warnedNoCamera;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            if (!warnedNoTarget)
            {
                warnedNoTarget = true;
                Debug.LogWarning("CameraFollow2D: Target is missing. Assign Player transform to Target.", this);
            }

            return;
        }

        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
            {
                if (!warnedNoCamera)
                {
                    warnedNoCamera = true;
                    Debug.LogError("CameraFollow2D: Camera not found. Attach script to Main Camera or assign MainCamera tag.");
                }

                return;
            }
        }

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = transform.position.z;

        if (useClampBounds)
        {
            desiredPosition = ClampToArena(desiredPosition);
        }

        float t = Mathf.Clamp01(followLerpSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
    }

    private Vector3 ClampToArena(Vector3 desiredPosition)
    {
        float xMin = Mathf.Min(minBounds.x, maxBounds.x);
        float xMax = Mathf.Max(minBounds.x, maxBounds.x);
        float yMin = Mathf.Min(minBounds.y, maxBounds.y);
        float yMax = Mathf.Max(minBounds.y, maxBounds.y);

        if (cam != null && cam.orthographic)
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            xMin += halfWidth;
            xMax -= halfWidth;
            yMin += halfHeight;
            yMax -= halfHeight;
        }

        if (xMin > xMax)
        {
            float middleX = (minBounds.x + maxBounds.x) * 0.5f;
            xMin = middleX;
            xMax = middleX;
        }

        if (yMin > yMax)
        {
            float middleY = (minBounds.y + maxBounds.y) * 0.5f;
            yMin = middleY;
            yMax = middleY;
        }

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, xMin, xMax);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, yMin, yMax);
        return desiredPosition;
    }

    private void OnValidate()
    {
        followLerpSpeed = Mathf.Clamp(followLerpSpeed, 1f, 20f);
    }
}
