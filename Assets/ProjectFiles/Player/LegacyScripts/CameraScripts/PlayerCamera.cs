using DG.Tweening;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private InputManager InputManager;

    [SerializeField]
    private Transform orientation;

    [SerializeField]
    private Transform cameraHolder;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * InputManager.sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * InputManager.sensitivityY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
