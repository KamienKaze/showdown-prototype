using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementBase))]
public class WallRunning : MonoBehaviour
{
    [Header("Wall Running")]
    public float wallRunSpeed;
    public float wallClimbSpeed;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallRunTime;
    private float wallRunTimer;

    public LayerMask wallLayer;
    public LayerMask groundLayer;

    [Header("Wall Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("References")]
    [SerializeField]
    private PlayerCamera playerCamera;
    private PlayerMovementBase playerMovementBase;
    private Transform orientation;

    private bool upwardsRunning;
    private bool downwardsRunning;

    private void Start()
    {
        playerMovementBase = GetComponent<PlayerMovementBase>();
        orientation = playerMovementBase.orientation.transform;
    }

    private void Update()
    {
        //upwardsRunning = playerMovementBase.inputManager.leftShift;
        //downwardsRunning = playerMovementBase.inputManager.leftControl;

        CheckForWall();
        WallRunningSpeedControl();
    }

    private void FixedUpdate()
    {
        CheckIfWallRunShouldStart();

        if (playerMovementBase.isWallRunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(
            transform.position,
            orientation.right,
            out rightWallHit,
            wallCheckDistance,
            wallLayer
        );
        wallLeft = Physics.Raycast(
            transform.position,
            -orientation.right,
            out leftWallHit,
            wallCheckDistance,
            wallLayer
        );
    }

    private bool IsPlayerAboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, groundLayer);
    }

    private void CheckIfWallRunShouldStart()
    {
        if (
            (wallLeft || wallRight)
            && playerMovementBase.inputManager.verticalInput > 0
            && IsPlayerAboveGround()
            && !exitingWall
        )
        {
            if (!playerMovementBase.isWallRunning)
            {
                StartWallRun();
            }

            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if (wallRunTimer <= 0 && playerMovementBase.isWallRunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if (playerMovementBase.inputManager.jumpInput)
            {
                WallJump();
            }
        }
        else if (exitingWall)
        {
            if (playerMovementBase.isWallRunning)
            {
                StopWallRun();
            }

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        else
        {
            if (playerMovementBase.isWallRunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        playerMovementBase.isWallRunning = true;
        playerMovementBase.playerRigidbody.useGravity = false;

        wallRunTimer = wallRunTime;

        playerCamera.DoFov(90f);
        if (wallLeft)
            playerCamera.DoTilt(-5f);
        if (wallRight)
            playerCamera.DoTilt(5f);
    }

    private void StopWallRun()
    {
        playerMovementBase.isWallRunning = false;
        playerMovementBase.playerRigidbody.useGravity = true;

        playerCamera.DoFov(80f);
        playerCamera.DoTilt(0f);
    }

    private void WallRunningMovement()
    {
        playerMovementBase.playerRigidbody.velocity = new Vector3(
            playerMovementBase.playerRigidbody.velocity.x,
            0f,
            playerMovementBase.playerRigidbody.velocity.z
        );

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if (
            (orientation.forward - wallForward).magnitude
            > (orientation.forward - -wallForward).magnitude
        )
            wallForward = -wallForward;

        // forward force
        playerMovementBase.playerRigidbody.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // upwards/downwards force
        if (upwardsRunning)
            playerMovementBase.playerRigidbody.velocity = new Vector3(
                playerMovementBase.playerRigidbody.velocity.x,
                wallClimbSpeed,
                playerMovementBase.playerRigidbody.velocity.z
            );
        if (downwardsRunning)
            playerMovementBase.playerRigidbody.velocity = new Vector3(
                playerMovementBase.playerRigidbody.velocity.x,
                -wallClimbSpeed,
                playerMovementBase.playerRigidbody.velocity.z
            );

        // push to wall force
        if (
            !(wallLeft && playerMovementBase.inputManager.horizontalInput > 0)
            && !(wallRight && playerMovementBase.inputManager.horizontalInput < 0)
        )
            playerMovementBase.playerRigidbody.AddForce(-wallNormal * 100, ForceMode.Force);
    }

    private void WallRunningSpeedControl()
    {
        if (playerMovementBase.currentPlayerState != PlayerState.WallRunning)
        {
            return;
        }

        Vector3 flatVelocity = new Vector3(
            playerMovementBase.playerRigidbody.velocity.x,
            0f,
            playerMovementBase.playerRigidbody.velocity.z
        );

        if (flatVelocity.magnitude > wallRunSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * wallRunSpeed;
            playerMovementBase.playerRigidbody.velocity = new Vector3(
                limitedVelocity.x,
                playerMovementBase.playerRigidbody.velocity.y,
                limitedVelocity.z
            );
        }
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        playerMovementBase.playerRigidbody.velocity = new Vector3(
            playerMovementBase.playerRigidbody.velocity.x,
            0f,
            playerMovementBase.playerRigidbody.velocity.z
        );
        playerMovementBase.playerRigidbody.AddForce(forceToApply, ForceMode.Impulse);
    }
}
