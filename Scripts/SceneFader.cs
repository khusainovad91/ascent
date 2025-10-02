using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Unity.Netcode;

public class SceneFader : NetworkBehaviour//<SceneFader>
{
    public static SceneFader Instance;
    public Image fadeImage;          // чёрный фон
    public float fadeDuration = 1f;  // скорость затухания
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Rpc(SendTo.Everyone)]
    public void FadeOutClientRpc()
    {
        if (fadeImage != null)
            StartCoroutine(FadeOut());
    }

    [Rpc(SendTo.Everyone)]
    public void FadeInClientRpc()
    {
        if (fadeImage != null)
            StartCoroutine(FadeIn());
    }

    // ===================== API =====================

    // Вызываешь это на сервере перед загрузкой сцены
    public void RunFadeOut()
    {
        FadeOutClientRpc();
    }

    // Вызываешь это на сервере после загрузки
    public void RunFadeIn()
    {
        FadeInClientRpc();
    }


    //public IEnumerator LoadSceneWithFader(Scene name)
    //{
    //    yield return FadeOut();
    //    CustomSceneManager.Instance.LoadScene(name);
    //    yield return new WaitForSeconds(2f);
    //    yield return FadeIn();
    //}

    public IEnumerator LoadSceneWithFader(CustomScene name)
    {
        // сервер говорит всем клиентам запустить FadeOut
        SceneFader.Instance.RunFadeOut();
        yield return new WaitForSeconds(SceneFader.Instance.fadeDuration);

        bool loaded = false;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += (sceneName, mode, completed, timedOut) =>
        {
            if (sceneName == CustomSceneManager.Instance.GetSceneName(name))
            {
                SceneFader.Instance.RunFadeIn();
                loaded = true;
            }
        };

        CustomSceneManager.Instance.LoadScene(name);

        while (!loaded)
            yield return null;
    }

    public IEnumerator FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = t / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = Color.black;
    }

    public IEnumerator FadeIn()
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = 1 - (t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
        fadeImage.color = Color.clear;
    }
}
