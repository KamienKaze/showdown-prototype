using UnityEngine;

[RequireComponent(typeof(PlayerReferences))]
public class PlayerRun : MonoBehaviour
{
    // References
    private PlayerReferences playerReferences;
    private InputManager inputManager;

    private Rigidbody playerRigidbody;
    private Transform orientation;

    // Variables
    private Vector3 moveDirection;

    private void Start() { }
}
