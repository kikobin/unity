using UnityEngine;

public class PlayerSpriteFacing : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private bool faceRightByDefault = true;
    [SerializeField] private float minDirectionThreshold = 0.01f;

    private Vector2 lastFacingDirection = Vector2.right;
    private bool warnedMissingRenderer;

    private void Awake()
    {
        CacheReferences();
        if (!faceRightByDefault)
        {
            lastFacingDirection = Vector2.left;
        }
    }

    private void OnEnable()
    {
        CacheReferences();
        if (playerAttack != null)
        {
            playerAttack.FiredWithDirection += OnFired;
        }
    }

    private void OnDisable()
    {
        if (playerAttack != null)
        {
            playerAttack.FiredWithDirection -= OnFired;
        }
    }

    private void Update()
    {
        if (targetRenderer == null)
        {
            CacheReferences();
            if (targetRenderer == null)
            {
                if (!warnedMissingRenderer)
                {
                    warnedMissingRenderer = true;
                    Debug.LogWarning("PlayerSpriteFacing: SpriteRenderer is missing. Assign Target Renderer.", this);
                }

                return;
            }
        }

        Vector2 candidateDirection = ResolveDirection();
        if (candidateDirection.sqrMagnitude >= minDirectionThreshold * minDirectionThreshold)
        {
            lastFacingDirection = candidateDirection.normalized;
        }

        if (Mathf.Abs(lastFacingDirection.x) >= minDirectionThreshold)
        {
            targetRenderer.flipX = lastFacingDirection.x < 0f;
        }
    }

    private Vector2 ResolveDirection()
    {
        if (playerRb != null && playerRb.linearVelocity.sqrMagnitude >= 0.0001f)
        {
            return playerRb.linearVelocity;
        }

        if (playerController != null)
        {
            return playerController.SafeLastMoveDirection;
        }

        if (playerAttack != null)
        {
            return playerAttack.LastShootDirection;
        }

        return lastFacingDirection;
    }

    private void OnFired(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            lastFacingDirection = direction.normalized;
        }
    }

    private void CacheReferences()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
            if (playerController == null)
            {
                playerController = GetComponentInParent<PlayerController>();
            }
        }

        if (playerAttack == null)
        {
            playerAttack = GetComponent<PlayerAttack>();
            if (playerAttack == null)
            {
                playerAttack = GetComponentInParent<PlayerAttack>();
            }
        }

        if (playerRb == null)
        {
            playerRb = GetComponent<Rigidbody2D>();
            if (playerRb == null)
            {
                playerRb = GetComponentInParent<Rigidbody2D>();
            }
        }
    }
}
