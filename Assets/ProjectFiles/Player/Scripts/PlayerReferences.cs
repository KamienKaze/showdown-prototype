using Unity.Netcode;
using UnityEngine;

public class PlayerReferences : NetworkBehaviour
{
    [HideInInspector]
    public float maxMovementSpeed;

    [Header("Player Game Objects")]
    public GameObject playerObject;
    public GameObject playerModel;
    public GameObject orientation;
    public GameObject cameraPosition;

    [Header("Running Values")]
    public float runSpeed;
    public float groundDrag;

    [Header("Jumping Values")]
    public float jumpForce;
    public float jumpCooldown;
    public float airSpeedMultiplier;

    [Header("Wall Running Values")]
    public float wallRunSpeed;
    public float wallRunForce;
    public float wallClimbSpeed;
    public float wallRunTime;
    public float minimalJumpHeight;

    [Header("Wall Jumping Values")]
    public float wallJumpUpForce;
    public float wallJumpSideForce;

    [Header("Wall Exiting Values")]
    public float exitWallTime;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float playerHeight;

    [Header("WallDetection")]
    public LayerMask wallLayer;
    public float sideWallCheckDistance;

    private void Start()
    {
        if (!IsOwner)
        {
            return;
        }
    }
}
