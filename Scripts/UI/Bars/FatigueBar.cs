using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FatigueBar : SegmentedBar
{
    private HeroData heroData;
    [SerializeField] bool vlg;
    //todo сделать base класс SegmentedBar, вынести туда RemoveSegment, AddSegment, SegmentsExpandeController()
    //добавить тригер OnFatigueChanged 
    private void OnEnable()
    {
        EventManager.Instance.Subscribe<HeroData>("OnFatigueChanged", UpdateFatigueUi);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<HeroData>("OnFatigueChanged", UpdateFatigueUi);
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

    private void UpdateFatigueUi(HeroData heroData)
    {
        if (this.heroData == heroData)
        {
            StartCoroutine(UpdateSegments(heroData.Stats.fatigue, vlg));
        }
    }

    private void UpdateFatigueUINoAnimation(HeroData heroData)
    {
        UpdateSegmentsNoAnimation(heroData.Stats.fatigue, vlg);
    }

    public void SetUpHeroFatigue(HeroData heroData)
    {
        this.heroData = heroData;
        UpdateFatigueUINoAnimation(heroData);
    }

}
