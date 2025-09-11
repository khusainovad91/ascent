using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Unity.Netcode;
using System;

public class CustomSceneManager : PersistentSingleton<CustomSceneManager>
{
    public void LoadScene(Scene newScene)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        
        bool allClientsLoaded = false;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += (loadedSceneName, mode, clientsCompiled, clientsTimedOut) =>
        {
            if (loadedSceneName == GetSceneName(newScene))
            {
                allClientsLoaded = true;
            }
        };

        NetworkManager.Singleton.SceneManager.LoadScene(newScene.ToString(), LoadSceneMode.Single);

        StartCoroutine(WaitForSceneLoad(allClientsLoaded));
    }

    private IEnumerator WaitForSceneLoad(bool allClientsLoaded)
    {
        while (!allClientsLoaded)
        {
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Все клиенты загрузили сцену");
    }

    public string GetSceneName(Scene scene)
    {
        switch (scene)
        {
            case Scene.CharacterSelectScene:
                return "CharacterSelect"; // Точное имя как в Build Settings
            case Scene.TestScene:
                return "TestScene";
            case Scene.MultiplayerMenu:
                return "MultiplayerMenu";
            case Scene.BarScene1:
                return "BarScene1";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum Scene
{
    CharacterSelectScene,
    TestScene,
    MultiplayerMenu,
    BarScene1
}