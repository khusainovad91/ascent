using ExtensionMethods;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : PersistentSingleton<UiManager>
{
    HeroData _heroData;

    //top
    [SerializeField] GameObject _uiTopPanel;
    [SerializeField] GameObject _uiTopHeroUiPrefab;
    List<GameObject> _uiTopHeroList;

    //bottom
    [SerializeField] GameObject _uiBottomPanel;
    [SerializeField] UiCharacterPanelManager _uiBottomCharacterPanel;

    //bars
    [SerializeField] HealthBar _uiBottomHealthBar;
    [SerializeField] FatigueBar _uiBottomFatigueBar;
    [SerializeField] MpBar _uiBottomMovementPointsBar;

    //buttons
    [SerializeField] GameObject _bottomButtonsPanel;
    [SerializeField] PatchButton _uiPatchButton;
    [SerializeField] MoveButton _uiMoveButton;
    [SerializeField] RestButton _uiRestButton;
    [SerializeField] FatigueToMpButton _uiFatigueToMp;
    [SerializeField] EndOfTurnButton _uiEndTurn;
    [SerializeField] AttackButton _uiAttackButton;
    //todo delete
    //[SerializeField] ThrowDicesButton _uiThrowDicesButton;

    [SerializeField] ReactButton _uiReactButton;

    //cards
    [SerializeField] GameObject _uiCardsInventoryPanel;
    [SerializeField] GameObject _uiCardsPassivePanel;
    [SerializeField] GameObject _uICardsActivePanel;
    [SerializeField] GameObject cardPool;

    //quest

    private new void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<FieldHero>("event_SelectHero", SetUpThis);
        EventManager.Instance.Subscribe("event_UnselectHero", TurnOffBottomPanel);
        EventManager.Instance.Subscribe<HeroAttackCommand>("ChooseSunSkull", SetUpPerformAttackButton);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<FieldHero>("event_SelectHero", SetUpThis);
            EventManager.Instance.Unsubscribe("event_UnselectHero", TurnOffBottomPanel);
            EventManager.Instance.Unsubscribe<HeroAttackCommand>("ChooseSunSkull", SetUpPerformAttackButton);
        }
    }

    private void Start()
    {
        TurnOffBottomPanel();
    }

    public void TurnOnTopPanel() => _uiTopPanel.SetActive(true);
    public void TurnOffTopPanel() => _uiTopPanel.SetActive(false);
    public void TurnOnBottomPanel() => _uiBottomPanel.SetActive(true);
    public void TurnOffBottomPanel()
    {
        if (_heroData != null)
        {
            _heroData.FieldHero.GetComponent<Outline>().enabled = false;
        }
        UtilClass.SetNewParentForAllChilds(_uiCardsInventoryPanel.transform, cardPool.transform);
        UtilClass.SetNewParentForAllChilds(_uiCardsPassivePanel.transform, cardPool.transform);
        UtilClass.SetNewParentForAllChilds(_uICardsActivePanel.transform, cardPool.transform);
        _uiAttackButton.gameObject.SetActive(false);
        _uiBottomPanel.SetActive(false);
    }
    // Метод устанавливает настройки для панелей сверху снизу
    private void SetUpThis(FieldHero fieldHero)
    {
        this._heroData = fieldHero.HeroData;
        fieldHero.GetComponent<Outline>().enabled = true;
        fieldHero.GetComponent<Outline>().OutlineColor = fieldHero.HeroData.PlayerColor;

        TurnOnBottomPanel();
        if (_heroData.IsActive)
        {
            _bottomButtonsPanel.SetActive(true);
            _uiReactButton.gameObject.SetActive(false);
            SetUpBottomPanel(this._heroData);
        }
    }

    public void SetUpThisOnEnemyTurn(FieldHero fieldHero)
    {
        this._heroData = fieldHero.HeroData;
        TurnOnBottomPanel();
        _uiCardsInventoryPanel.gameObject.SetActive(true);
        _bottomButtonsPanel.SetActive(false);
        _uiReactButton.gameObject.SetActive(false);

        SetUpStatsAndBars(_heroData);
        SetUpCards(_heroData);
    }

    public void SetUpReactButton(HeroData heroData)
    {
        _uiReactButton.gameObject.SetActive(true);
        _uiReactButton.SetUp(heroData);
    }

    public void SetUpTopPanel(List<HeroData> heroes)
    {
        _uiTopHeroList = new List<GameObject>();
        for (int i = 0; i < heroes.Count; i++)
        {
            //setting up hero ui for every hero
            if (heroes[i] != null)
            {
                var uiCurentHero = Instantiate(_uiTopHeroUiPrefab, _uiTopPanel.transform);
                uiCurentHero.name = $"Hero {i}";
                uiCurentHero.transform.Find("Player Image").GetComponent<Image>().sprite = heroes[i].HeroImage; //setting up picture
                uiCurentHero.transform.Find("Player Color").GetComponent<Image>().color = heroes[i].PlayerColor;
                _uiTopHeroList.Add(uiCurentHero);

                uiCurentHero.GetComponent<UiHeroTop>().SetUpUiHeroTop(heroes[i]);
                //setting up top hero bars: hp / mp / fatigue
                var uiFatigueBar = uiCurentHero.GetComponentInChildren<FatigueBar>();
                uiFatigueBar.SetUpHeroFatigue(heroes[i]);
                var uiHealthBar = uiCurentHero.GetComponentInChildren<HealthBar>();
                uiHealthBar.SetUpHeroHealth(heroes[i]);
                var uiMpBar = uiCurentHero.GetComponentInChildren<MpBar>();
                uiMpBar.SetUpMpUi(heroes[i]);
                var actionsBar = uiCurentHero.GetComponentInChildren<ActionsBar>();
                actionsBar.SetUpActionsUi(heroes[i]);
                var conditionsBar = uiCurentHero.GetComponentInChildren<ConditionBar>();
                conditionsBar.SetUpConditionBar(heroes[i]);

                uiCurentHero.SetActive(true);
            }
        }
        TurnOnTopPanel();
    }

    // Метод устанавливает настройки для панели внизу
    private void SetUpBottomPanel(HeroData heroData)
    {
        SetUpStatsAndBars(heroData);
        SetUpCards(heroData);

        //buttons
        if (!heroData.IsActive || !heroData.FieldHero.IsOwner)
        {
            _bottomButtonsPanel.SetActive(false);
            return;
        }

        SetUpButtons(heroData);
    }

    private void SetUpCards(HeroData heroData)
    {
        _uiCardsInventoryPanel.GetComponent<LayoutGroup>().enabled = true; 
        _uiCardsPassivePanel.GetComponent<LayoutGroup>().enabled = true;
        _uICardsActivePanel.GetComponent<LayoutGroup>().enabled = true;
        UtilClass.SetNewParentForAllChilds(_uiCardsInventoryPanel.transform, cardPool.transform);
        UtilClass.SetNewParentForAllChilds(_uiCardsPassivePanel.transform, cardPool.transform);
        UtilClass.SetNewParentForAllChilds(_uICardsActivePanel.transform, cardPool.transform);
        //todo other panels nado почистить тоже
        foreach (var item in heroData.InventoryData.EquipedCards)
        {
            switch (item.Key)
            {
                case HandItem handItem:
                    item.Value.transform.SetParent(_uiCardsInventoryPanel.transform);
                    break;
                default:
                    Debug.Log("Lox"); //todo
                    break;
            }
        }

        foreach (var card in heroData.InventoryData.Abilities)
        {
            switch (card)
            {
                case UiPassiveCard uiPassiveCard:
                    uiPassiveCard.transform.SetParent(_uiCardsPassivePanel.transform);
                    break;
                case UiActiveCard uiActiveCard:
                    uiActiveCard.transform.SetParent(_uICardsActivePanel.transform);
                    break;
            }
        }

        StartCoroutine(_uiCardsInventoryPanel.GetComponent<LayoutGroup>().DisableLayoutAfterFrame());
        StartCoroutine(_uiCardsPassivePanel.GetComponent<LayoutGroup>().DisableLayoutAfterFrame());
        StartCoroutine(_uICardsActivePanel.GetComponent<LayoutGroup>().DisableLayoutAfterFrame());
    }

    //except reactButton
    private void SetUpButtons(HeroData heroData)
    {
        _uiAttackButton.gameObject.SetActive(false);

        if (heroData.FieldHero.GetComponent<ConditionHandler>().ContainsCondition<Tired>())
        {
            _uiMoveButton.gameObject.SetActive(false);
            _uiFatigueToMp.gameObject.SetActive(true);
            _uiFatigueToMp.SetUp(heroData);
        }
        else
        {
            _uiMoveButton.gameObject.SetActive(true);
            _uiFatigueToMp.gameObject.SetActive(false);
            _uiMoveButton.GetComponent<MoveButton>().SetUp(heroData);
        }

        if (heroData.FieldHero.GetComponent<ConditionHandler>().ContainsCondition<Rested>())
        {
            _uiRestButton.gameObject.SetActive(false);
        }
        else
        {
            _uiRestButton.gameObject.SetActive(true);
            _uiRestButton.GetComponent<RestButton>().SetUp(heroData);
        }

        _uiPatchButton.GetComponent<PatchButton>().SetUp(heroData);

        _uiEndTurn.GetComponent<EndOfTurnButton>().SetUp(heroData);
    }

    private void SetUpStatsAndBars(HeroData heroData)
    {
        _uiBottomCharacterPanel.UpdateCurrentHeroStats(heroData);
        _uiBottomHealthBar.SetUpHeroHealth(heroData);
        _uiBottomFatigueBar.SetUpHeroFatigue(heroData);
        _uiBottomMovementPointsBar.SetUpMpUi(heroData);
    }

    // Метод в конце хода убирает кнопки
    private void ToggleOffButtonsPanel(HeroData heroData)
    {
        if (this._heroData == heroData)
        {
            _bottomButtonsPanel.SetActive(false);
        }
    }

    private void SetUpPerformAttackButton(HeroAttackCommand hac)
    {
        LeanTween.delayedCall(Constants.DICE_ROLL_TIME, TurnOnAttackButton);

        void TurnOnAttackButton()
        {
            _uiAttackButton.gameObject.SetActive(true);
            _uiAttackButton.GetComponent<AttackButton>().SetUp(hac);
        }
    }

    //---------------------------------------------------------------------
    //REACTING
    //---------------------------------------------------------------------

    public void HandleReact()
    {
        _uiBottomCharacterPanel.gameObject.SetActive(false);
        _uiReactButton.gameObject.SetActive(false);
        TurnOffBottomPanel();
    }
}
