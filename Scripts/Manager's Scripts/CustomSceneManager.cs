using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Unity.Netcode;
using System;

public class CustomSceneManager : PersistentSingleton<CustomSceneManager>
{
    public void LoadScene(CustomScene newScene)
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

    public string GetSceneName(CustomScene scene)
    {
        switch (scene)
        {
            case CustomScene.CharacterSelectScene:
                return "CharacterSelectScene"; // Точное имя как в Build Settings
            case CustomScene.TestScene:
                return "TestScene";
            case CustomScene.MultiplayerMenu:
                return "MultiplayerMenu";
            case CustomScene.BarScene1:
                return "BarScene1";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum CustomScene
{
    CharacterSelectScene,
    TestScene,
    MultiplayerMenu,
    BarScene1
}