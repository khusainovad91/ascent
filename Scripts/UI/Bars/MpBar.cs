using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MpBar : SegmentedBar
{
    private HeroData heroData;
    [SerializeField] bool vlg;
    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Subscribe<HeroData>("OnMpChanged", UpdateMpUi);
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<HeroData>("OnMpChanged", UpdateMpUi);
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
    private void UpdateMpUi(HeroData heroData)
    {
        if (this.heroData == heroData)
        {
            StartCoroutine(UpdateSegments(heroData.Stats.MovementPoints, vlg));
        }
    }

    //todo в SegmentedBar
    private void UpdateMpUiNoAnimation(HeroData heroData)
    {
        UpdateSegmentsNoAnimation(heroData.Stats.MovementPoints, vlg);
    }

    public void SetUpMpUi(HeroData heroData)
    {
        this.heroData = heroData;
        UpdateMpUiNoAnimation(heroData);
    }

}
