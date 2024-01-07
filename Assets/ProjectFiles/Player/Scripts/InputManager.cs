using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Input Manager Config")]
    [SerializeField]
    private bool isPlayer = true;

    [Header("Mouse Settings")]
    public float sensitivityX;
    public float sensitivityY;

    [Header("Keybinds")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode wallClimbUpKey = KeyCode.LeftShift;
    public KeyCode wallClimbDownKey = KeyCode.LeftControl;

    [Header("Inputs")]
    public float horizontalInput;
    public float verticalInput;
    public bool jumpInput;
    public bool wallClimbUpInput;
    public bool wallClimbDownInput;

    private void Awake()
    {
        if (isPlayer)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (isPlayer)
        {
            GetPlayerInput();
        }
    }

    private void GetPlayerInput()
    {
        horizontalInput = GetHorizontalInput();
        verticalInput = GetVerticalInput();

        jumpInput = GetKeyInput(jumpKey);
        wallClimbUpInput = GetKeyInput(wallClimbUpKey);
        wallClimbDownInput = GetKeyInput(wallClimbDownKey);
    }

    private float GetHorizontalInput()
    {
        if (!Input.GetKey(leftKey) && Input.GetKey(rightKey))
        {
            return 1;
        }

        if (Input.GetKey(leftKey) && !Input.GetKey(rightKey))
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    private float GetVerticalInput()
    {
        if (Input.GetKey(forwardKey) && !Input.GetKey(backwardKey))
        {
            return 1;
        }

        if (!Input.GetKey(forwardKey) && Input.GetKey(backwardKey))
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    private bool GetKeyInput(KeyCode key)
    {
        return Input.GetKey(key);
    }
}
