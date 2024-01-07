using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputManager))]
public class PlayerMovementBase : MonoBehaviour
{
    public PlayerState currentPlayerState;

    [Header("Movement")]
    public float runSpeed;
    public float groundDrag;
    public float playerVelocity;

    private float maxMoveSpeed;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    private bool isReadyToJump = true;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("References")]
    public GameObject playerObject;
    public GameObject orientation;
    public GameObject cameraPosition;

    [HideInInspector]
    public Rigidbody playerRigidbody;

    [HideInInspector]
    public InputManager inputManager;

    private Vector3 moveDirection;

    #region

    [HideInInspector]
    public bool isWallRunning;

    #endregion

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = CheckIfPlayerIsGrounded();
        ApplyGroundDrag();
        GroundAndAirSpeedControl();
        HandleJumpRequest();
        StateHandler();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        moveDirection =
            orientation.transform.forward * inputManager.verticalInput
            + orientation.transform.right * inputManager.horizontalInput;

        if (isGrounded)
        {
            playerRigidbody.AddForce(moveDirection.normalized * runSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            playerRigidbody.AddForce(
                moveDirection.normalized * runSpeed * 10f * airMultiplier,
                ForceMode.Force
            );
        }
    }

    private void GroundAndAirSpeedControl()
    {
        Vector3 flatVelocity = new Vector3(
            playerRigidbody.velocity.x,
            0f,
            playerRigidbody.velocity.z
        );

        playerVelocity = flatVelocity.magnitude;

        if (currentPlayerState != PlayerState.Running && currentPlayerState != PlayerState.Air)
        {
            return;
        }

        if (flatVelocity.magnitude > runSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * runSpeed;
            playerRigidbody.velocity = new Vector3(
                limitedVelocity.x,
                playerRigidbody.velocity.y,
                limitedVelocity.z
            );
        }
    }

    private void HandleJumpRequest()
    {
        if (inputManager.jumpInput && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Jump()
    {
        playerRigidbody.velocity = new Vector3(
            playerRigidbody.velocity.x,
            0f,
            playerRigidbody.velocity.z
        );

        playerRigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        isReadyToJump = true;
    }

    private bool CheckIfPlayerIsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            playerHeight * 0.5f + 0.2f,
            groundLayer
        );
    }

    private void ApplyGroundDrag()
    {
        if (isGrounded)
        {
            playerRigidbody.drag = groundDrag;
        }
        else
        {
            playerRigidbody.drag = 0;
        }
    }

    private void StateHandler()
    {
        if (isGrounded && (inputManager.horizontalInput > 0 || inputManager.verticalInput > 0))
        {
            currentPlayerState = PlayerState.Running;
        }
        else if (isGrounded)
        {
            currentPlayerState = PlayerState.Standing;
        }
        else if (isWallRunning)
        {
            currentPlayerState = PlayerState.WallRunning;
        }
        else if (!isGrounded)
        {
            currentPlayerState = PlayerState.Air;
        }
    }
}
