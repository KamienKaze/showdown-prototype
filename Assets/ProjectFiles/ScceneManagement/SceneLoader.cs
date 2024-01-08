using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameManager))]
public class SceneLoader : MonoBehaviour
{
    private Dictionary<MenuScene, string> menuScenes = new Dictionary<MenuScene, string>();
    private Dictionary<Map, string> maps = new Dictionary<Map, string>();

    private void Start()
    {
        InitializeMenuScenes();
        InitializeMaps();
    }

    private void InitializeMenuScenes()
    {
        menuScenes.Add(MenuScene.MainMenu, "MainMenu");
        menuScenes.Add(MenuScene.GameLobby, "GameLobby");
    }

    private void InitializeMaps()
    {
        maps.Add(Map.Playground, "Playground");
    }

    public void LoadMenuScene(MenuScene menuScene)
    {
        foreach (KeyValuePair<MenuScene, string> scene in menuScenes)
        {
            if (scene.Key == menuScene)
            {
                SceneManager.LoadScene(scene.Value, LoadSceneMode.Single);
            }
        }
    }

    public void LoadMap(Map map)
    {
        foreach (KeyValuePair<Map, string> scene in maps)
        {
            if (scene.Key == map)
            {
                SceneManager.LoadScene(scene.Value, LoadSceneMode.Single);
            }
        }
    }
}
