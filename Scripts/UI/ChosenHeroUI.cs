using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChosenHeroUI : NetworkBehaviour
{
    [SerializeField] Image _heroPortrait;
    [SerializeField] TextMeshProUGUI _classText;
    [SerializeField] Button _removeHeroButton;
    [SerializeField] TextMeshProUGUI _playerIdText;
    public ulong PlayerId { get; private set; } = 999;
    public int HeroID { get; private set; } = -1;
    public int ClassID { get; private set; } = -1;
    public bool IsOcupied { get; private set; } = false;

    private BaseHeroSO baseHeroSO;

    public override void OnNetworkSpawn()
    {
        Hide();
        _removeHeroButton.onClick.AddListener(() =>
        {
            if (IsServer)
            {
                RemoveChosenHeroRpc();
            }
        });
    }

    public void SetHero(int heroID)
    {
        HeroID = heroID;
        IsOcupied = true;
        baseHeroSO = SelectCharacterScreenUI.Instance.BaseHeroList.list[heroID];
        _heroPortrait.sprite = baseHeroSO.HeroPrefab.GetComponent<FieldHero>().HeroSprite;
    }

    public void SetPlayerId(ulong playerId)
    {
        PlayerId = playerId;
        _playerIdText.text = "PlyerID " + playerId;
    }

    public HeroData FormHeroData(int heroNumber)
    {
        Color heroColor = (PlayerId == 0) ? Color.red : Color.blue;

        if (IsOcupied) //&& ClassID != -1)
        {
            HeroData heroData = new HeroData(heroNumber, PlayerId, ClassID, baseHeroSO, heroColor);
            return heroData;
        }

        return null;
    }

    [Rpc(SendTo.Everyone)]
    private void RemoveChosenHeroRpc()
    {
        HeroID = -1;
        ClassID = -1;
        PlayerId = 999;
        _playerIdText.text = "";
        IsOcupied = false;
        Hide();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
