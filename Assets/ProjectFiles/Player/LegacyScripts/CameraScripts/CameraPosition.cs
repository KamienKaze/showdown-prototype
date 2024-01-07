using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    [SerializeField]
    private Transform PlayerCameraPosition;

    void Update()
    {
        transform.position = PlayerCameraPosition.position;
    }
}
