using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameRoot gameRoot;

    private Vector2 inputDirection;
    private bool canMove = true;
    private Vector2 lastPhysicsPosition;
    private float currentSpeed;

    public Vector2 LastMoveDirection { get; private set; } = Vector2.right;
    public float CurrentSpeed => currentSpeed;
    public Vector2 SafeLastMoveDirection
    {
        get
        {
            if (LastMoveDirection.sqrMagnitude > 0.0001f)
            {
                return LastMoveDirection.normalized;
            }

            return Vector2.right;
        }
    }

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (rb == null)
        {
            Debug.LogError("PlayerController: Rigidbody2D is missing. Add Rigidbody2D to the Player object.", this);
            canMove = false;
            currentSpeed = 0f;
            return;
        }
        lastPhysicsPosition = rb.position;

        if (gameRoot == null)
        {
            gameRoot = FindObjectOfType<GameRoot>();
        }
    }

    private void Update()
    {
        if (!canMove)
        {
            return;
        }

        if (!IsControlAllowed())
        {
            inputDirection = Vector2.zero;
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 rawInput = new Vector2(horizontal, vertical);
        inputDirection = rawInput.normalized;

        if (inputDirection.sqrMagnitude > 0.0001f)
        {
            LastMoveDirection = inputDirection;
        }
    }

    private void FixedUpdate()
    {
        currentSpeed = CalculateActualSpeed();

        if (!canMove)
        {
            return;
        }

        if (!IsControlAllowed())
        {
            return;
        }

        Vector2 nextPosition = rb.position + (inputDirection * moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(nextPosition);
    }

    private float CalculateActualSpeed()
    {
        if (rb == null)
        {
            return 0f;
        }

        Vector2 currentPosition = rb.position;
        float speed = (currentPosition - lastPhysicsPosition).magnitude / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
        lastPhysicsPosition = currentPosition;
        return speed;
    }

    private bool IsControlAllowed()
    {
        if (gameRoot == null)
        {
            return true;
        }

        return gameRoot.IsPlaying;
    }
}
