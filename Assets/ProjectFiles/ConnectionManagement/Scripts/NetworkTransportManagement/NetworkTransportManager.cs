using UnityEngine;

public class NetworkTransportManager : MonoBehaviour
{
    [HideInInspector]
    public ConnectionManager currentTransportConnectionManager;

    [SerializeField]
    private NetworkTransport currentTransport;

    [Header("Connection Management")]
    [SerializeField]
    private GameObject facepunchConnectionManager;

    [SerializeField]
    private GameObject unityConnectionManager;

    [Header("Transport Menus")]
    [SerializeField]
    private GameObject facepunchTransportMenu;

    [SerializeField]
    private GameObject unityTransportMenu;

    [Header("Transport Logos")]
    [SerializeField]
    private GameObject facepunchLogo;

    [SerializeField]
    private GameObject unityLogo;

    private void Start()
    {
        EnableTransportConnectionManager();
    }

    public NetworkTransport GetCurrentNetworkTransport()
    {
        return currentTransport;
    }

    private void EnableTransportConnectionManager()
    {
        switch (currentTransport)
        {
            case NetworkTransport.Facepunch:
                Destroy(unityConnectionManager);
                Destroy(unityTransportMenu);
                Destroy(unityLogo);

                facepunchConnectionManager.SetActive(true);
                facepunchTransportMenu.SetActive(true);
                facepunchLogo.SetActive(true);

                currentTransportConnectionManager =
                    facepunchConnectionManager.GetComponent<FacepunchConnectionManager>();

                break;

            case NetworkTransport.Unity:
                Destroy(facepunchConnectionManager);
                Destroy(facepunchTransportMenu);
                Destroy(facepunchLogo);

                unityConnectionManager.SetActive(true);
                unityTransportMenu.SetActive(true);
                unityLogo.SetActive(true);

                currentTransportConnectionManager =
                    unityConnectionManager.GetComponent<UnityConnectionManager>();

                break;
        }
    }
}
