using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyConditionBar : MonoBehaviour
{
    private EnemyObject _enemyObject;
    private Dictionary<ConditionType, Image> condImage;

    private void Awake()
    {
        condImage = new Dictionary<ConditionType, Image>();
        _enemyObject = GetComponentInParent<EnemyObject>();
    }


    private void OnEnable()
    {
        EventManager.Instance.Subscribe<Condition>("OnConditionAdd", OnConditionAdd);
        EventManager.Instance.Subscribe<Condition>("OnConditionRemove", OnConditionRemove);
    }

    private void OnConditionRemove(Condition condition)
    {
        if (condition.FieldObject is EnemyObject enemyObject && enemyObject == _enemyObject)
        {
            if (!condImage.ContainsKey(condition.Type)) return;

            Destroy(condImage[condition.Type].gameObject);
            condImage.Remove(condition.Type);
        }
    }

    private void OnConditionAdd(Condition condition)
    {
        Debug.Log("Попытка добавить кондишн");
        if (condition.FieldObject is EnemyObject enemyObject)
        {
            if (enemyObject == _enemyObject)
            {
                Debug.Log("Попытка успешна");
                if (condImage.ContainsKey(condition.Type)) return;

                var newImage = Instantiate(condition.Image, transform);
                newImage.GetComponent<RectTransform>().sizeDelta = new Vector2(0.5f, 0.5f);
                newImage.transform.LeanScale(new Vector2(0.0f, 0.0f), 0f);
                newImage.transform.LeanScale(new Vector2(1f, 1f), 0.5f).setOnComplete(() => LeanTween.delayedCall(1f, () => UtilClass.LeanPopDown(this.gameObject)));

                condImage.Add(condition.Type, newImage);
            } else
            {
                Debug.Log("enemyObject is " + enemyObject.name);
            }

        }
    }
}
