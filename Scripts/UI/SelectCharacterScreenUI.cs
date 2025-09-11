using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SelectCharacterScreenUI : NetworkBehaviour
{
    [field: SerializeField] public HeroesListSO BaseHeroList { get; private set; }
    public static SelectCharacterScreenUI Instance;
    [field:SerializeField] public GameObject SelectedCharactersLayoutGroup { get; private set; }
    public bool IsChoosingClass = false;
    [field:SerializeField] public List<ChosenHeroUI> ChosenHeroes { get; private set; }

    public override void OnNetworkSpawn()
    {
        Instance = this;
    }

    [Rpc(SendTo.Everyone)]
    public void SetNewHeroRpc(int heroId, ulong clientId)
    {
        ChosenHeroUI chosenHeroUi = null;
        for (int i = 0; i < ChosenHeroes.Count; i++) { 
            if (!ChosenHeroes[i].IsOcupied)
            {
                chosenHeroUi = ChosenHeroes[i];
                break;
            }
        }
        if (chosenHeroUi == null)
        {
            return;
        }

        chosenHeroUi.Show();
        chosenHeroUi.SetHero(heroId);
        chosenHeroUi.SetPlayerId(clientId);
        IsChoosingClass = true;
    }
}
