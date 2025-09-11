using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButtonUI : NetworkBehaviour
{
    private void OnEnable()
    {
        this.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!IsServer)
            {
                return;
            }

            SetHeroDataRpc();

            if (GameManager.Instance.Heroes.Count > 0)
            {
                CustomSceneManager.Instance.LoadScene(Scene.BarScene1);
                StartGameRpc();
            }
            else
            {
                Debug.Log("Не хватает героев");
            }

        });
    }

    [Rpc(SendTo.Everyone)]
    private void StartGameRpc()
    {
        GameManager.Instance.StartFirstLevel();
    }

    [Rpc(SendTo.Everyone)]
    private void SetHeroDataRpc()
    {
        int heroNumber = 0;
        foreach (var chosenHero in SelectCharacterScreenUI.Instance.ChosenHeroes)
        {
            if (chosenHero.IsOcupied)
            {
                var heroData = chosenHero.FormHeroData(heroNumber);
                if (heroData != null)
                {
                    GameManager.Instance.AddHero(heroData);
                    heroNumber++;
                }
            }
        }
    }
}
