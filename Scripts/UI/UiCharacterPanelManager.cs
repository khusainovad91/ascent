using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiCharacterPanelManager : PersistentSingleton<UiCharacterPanelManager>
{
    //hero image;
    [SerializeField]
    private Image heroImage;

    //bottom
    [SerializeField]
    private TMP_Text uiStrength;
    [SerializeField]
    private TMP_Text uiIntelect;
    [SerializeField]
    private TMP_Text uiAgility;
    [SerializeField]
    private TMP_Text uiWisdom;

    //upper stats
    [SerializeField]
    private TMP_Text uiMaxHealth;
    [SerializeField]
    private TMP_Text uiSpeed;
    [SerializeField]
    private TMP_Text uiMaxFatigue;

    private HeroStats currentHeroStats;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<HeroData>("OnHeroStatsChange", UpdateCurrentHeroStats);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<HeroData>("OnHeroStatsChange", UpdateCurrentHeroStats);
        }
    }

    public void UpdateCurrentHeroStats(HeroData heroData)
    {
        this.gameObject.SetActive(true);
        heroImage.sprite = heroData.HeroImage;
        uiStrength.text = heroData.Stats.strength.ToString();
        uiIntelect.text = heroData.Stats.intelect.ToString();
        uiAgility.text = heroData.Stats.agility.ToString();
        uiWisdom.text = heroData.Stats.wisdom.ToString();

        uiMaxHealth.text = heroData.Stats.MaxHP.ToString();
        uiSpeed.text = heroData.Stats.Speed.ToString();
        uiMaxFatigue.text = heroData.Stats.maxFatigue.ToString();
    }
}
