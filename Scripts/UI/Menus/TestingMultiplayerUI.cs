using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingMultiplayerUI : MonoBehaviour
{
    [SerializeField] private Button StartHost;
    [SerializeField] private Button StartClient;

    private void Start()
    {
        StartHost.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            //SceneFader.Instance.StartSceneWithFaderServerRpc(CustomScene.CharacterSelectScene);
            StartCoroutine(SceneFader.Instance.LoadSceneWithFader(CustomScene.CharacterSelectScene));
            //CustomSceneManager.Instance.LoadScene(Scene.CharacterSelectScene);

        });
        StartClient.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
