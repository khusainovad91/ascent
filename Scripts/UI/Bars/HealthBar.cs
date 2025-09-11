
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : SegmentedBar
{
    private HeroData heroData;
    [SerializeField] bool vlg = false;
    private void OnEnable()
    {
        EventManager.Instance.Subscribe<HeroData>("OnHpChanged", UpdateHealthUI);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<HeroData>("OnHpChanged", UpdateHealthUI);
        }
    }

    private void Awake()
    {
        Inititalize();
    }

    protected void Inititalize()
    {
        base.Inititalize(vlg);
        maxAmountOfSegments = 10;
    }

    //Функция определяет был выбран ли тот-же герой
    private void UpdateHealthUI(HeroData heroData)
    {
        if (this.heroData == heroData)
        {
            StartCoroutine(UpdateSegments(heroData.Stats.Hp, vlg));
        }
    }

    //todo в SegmentedBar
    private void UpdateHealthUINoAnimation(HeroData heroData)
    {
        UpdateSegmentsNoAnimation(heroData.Stats.Hp, vlg);
    }


    public void SetUpHeroHealth(HeroData heroData)
    {
        this.heroData = heroData;
        UpdateHealthUINoAnimation(heroData);
    }

}
