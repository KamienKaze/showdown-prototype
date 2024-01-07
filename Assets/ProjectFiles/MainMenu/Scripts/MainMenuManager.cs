using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private NetworkTransportManager networkTransportManager;

    [Header("Buttons")]
    [SerializeField]
    private Button hostButton;

    [SerializeField]
    private Button clientButton;

    [SerializeField]
    private Button exitButton;

    private NetworkTransport currentNetworkTransport;

    private void Start()
    {
        currentNetworkTransport = networkTransportManager.GetCurrentNetworkTransport();

        EnableNetworkTransportObjects();
    }

    private void EnableNetworkTransportObjects()
    {
        switch (currentNetworkTransport)
        {
            case NetworkTransport.Facepunch:
                EnableFacepunchObjects();
                clientButton.gameObject.SetActive(false);
                break;

            case NetworkTransport.Unity:
                EnableUnityObjects();
                break;
        }
    }

    private void EnableFacepunchObjects()
    {
        hostButton
            .onClick
            .AddListener(
                delegate()
                {
                    FacepunchConnectionManager.Singleton.StartHost();
                }
            );
    }

    private void EnableUnityObjects()
    {
        hostButton
            .onClick
            .AddListener(
                delegate()
                {
                    NetworkManager.Singleton.StartHost();
                }
            );

        clientButton
            .onClick
            .AddListener(
                delegate()
                {
                    NetworkManager.Singleton.StartClient();
                }
            );
    }
}
