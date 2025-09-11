using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConditionBar : MonoBehaviour
{
    private HeroData heroData;
    private Dictionary<ConditionType, Image> condImage;

    private void Awake()
    {
        condImage = new Dictionary<ConditionType, Image>();
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe<Condition>("OnConditionAdd", OnConditionAdd);
        EventManager.Instance.Subscribe<Condition>("OnConditionRemove", OnConditionRemove);
    }

    private void OnConditionAdd(Condition condition)
    {
        if (condition.FieldObject is FieldHero fieldHero && fieldHero.HeroData == heroData)
        {
            if (condImage.ContainsKey(condition.Type)) return;

            var newImage = Instantiate(condition.Image);
            newImage.transform.SetParent(transform);

            condImage.Add(condition.Type, newImage);
        } 
    }

    private void OnConditionRemove(Condition condition)
    {
        if (condition.FieldObject is FieldHero fieldHero && fieldHero.HeroData == heroData)
        {
            if (!condImage.ContainsKey(condition.Type)) return;

            Destroy(condImage[condition.Type].gameObject);
            condImage.Remove(condition.Type);
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<Condition>("OnConditionAdd", OnConditionAdd);
            EventManager.Instance.Unsubscribe<Condition>("OnConditionRemove", OnConditionRemove);
        }
    }

    public void SetUpConditionBar(HeroData heroData)
    {
        this.heroData = heroData;
    }
}
