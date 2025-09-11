
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsBar : SegmentedBar
{
    private HeroData heroData;
    [SerializeField] bool vlg = false;
    private void OnEnable()
    {
        EventManager.Instance.Subscribe<HeroData>("OnActionsAmountChange", UpdateActionsUI);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<HeroData>("OnActionsAmountChange", UpdateActionsUI);
        }
    }

    private void Awake()
    {
        Inititalize();
    }

    protected void Inititalize()
    {
        base.Inititalize(vlg);
        maxAmountOfSegments = 4;
    }

    //Функция определяет был выбран ли тот-же герой
    private void UpdateActionsUI(HeroData heroData)
    {
        if (this.heroData == heroData)
        {   
            StartCoroutine(UpdateSegments(heroData.Stats.ActionsAmount, vlg));
        }
    }

    //todo в SegmentedBar
    private void UpdateActionsUINoAnimation(HeroData heroData)
    {
        UpdateSegmentsNoAnimation(heroData.Stats.ActionsAmount, vlg);
    }

    public void SetUpActionsUi(HeroData heroData)
    {
        this.heroData = heroData;
        UpdateActionsUINoAnimation(heroData);
    }

}
