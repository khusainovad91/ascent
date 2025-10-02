using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//todo реализовать дочерние классы атак, защиты и тп с рызным OnPointerClick
public class UiCardAttack : UiCard
{
    //ссылка на героя
    public WeaponItem WeaponItem; //ссылка на ScriptableObject, чтобы достать кубики

    protected override bool HandlePointerClick(PointerEventData eventData)
    {
        Debug.Log("SelectControllerManager currentMode: " + SelectControllerManager.Instance.currentMode);
        switch (SelectControllerManager.Instance.currentMode)
        {
            case SelectionMode.Free:
                {
                    if (!ConditionsCheck(eventData)) { return false; }

                    if (this._heroData.Stats.ActionsAmount <= 0)
                    {
                        return false;
                    }

                    //todo delete
                    //base.OnPointerClick(eventData);
                    StartCoroutine(HandleAttack());
                    return false;
                }
            case SelectionMode.AttackCard:
                EventManager.Instance.TriggerEvent<MonoBehaviour>("OnSelect", this);
                return true;
        }
        return false;
    }

    public IEnumerator HandleAttack()
    {
        Debug.Log("usedWeaponRange: " + WeaponItem.MaxWeaponRange);

        var enemiesInRange = UtilClass.EnemiesInRange(_heroData.FieldHero, WeaponItem.MaxWeaponRange)
            .Where(e => !e.Key.isTargeted.Value && LineOfSight.CheckLos(_heroData.FieldHero.CurrentCell, e.Key))
            .ToDictionary(e => e.Key, e => e.Value);

        if (enemiesInRange.Count <= 0)
        {
            yield break;
        }

        _heroData.ChangeState(HeroState.Attacking);
        SelectControllerManager.Instance.ChangeMode(SelectionMode.Enemy);

        UtilClass.ShowEnemiesInRange(enemiesInRange);

        yield return uiSelectHandler.SelectEnemy(enemiesInRange.Keys.ToList());

        if (uiSelectHandler.Denied)
        {
            uiSelectHandler.RestoreStates();
            Debug.Log("ОТМЕНА");
            yield break;
        }

        AudioAndVisuals();
        var attackCommand = new HeroAttackCommand(_heroData.FieldHero, WeaponItem, uiSelectHandler.SelectedEnemy, enemiesInRange[uiSelectHandler.SelectedEnemy]);
        yield return attackCommand.SetUp();
        uiSelectHandler.EndOfMakingChoice<EnemyObject>(enemiesInRange.Keys.ToList());
        uiSelectHandler.RestoreStates();
    }

    private void AudioAndVisuals()
    {
        LeanTween.delayedCall(0.2f, () => _heroData.FieldHero.GetComponent<PersonSoundHandler>().PlaySound(PersonSound.Attack));
        if (WeaponItem.MaxWeaponRange > 1)
        {
            LeanTween.delayedCall(0.8f, () =>
            {
                uiSelectHandler.SelectedEnemy.Animator.SetTrigger("Impact");
                uiSelectHandler.SelectedEnemy.GetComponent<PersonSoundHandler>().PlaySound(PersonSound.Hitted);
            });
        }
        else
        {
            LeanTween.delayedCall(0.2f, () =>
            {
                uiSelectHandler.SelectedEnemy.Animator.SetTrigger("Impact");
                uiSelectHandler.SelectedEnemy.GetComponent<PersonSoundHandler>().PlaySound(PersonSound.Hitted);
            });
        }
        _heroData.FieldHero.Animator.SetTrigger("Attack");
    }



    //private Dictionary<EnemyObject, int> EnemiesInRange(int range)
    //{
    //    var objectFinder = new ObjectFinder(_heroData.FieldHero.CurrentCell, range);
    //    return objectFinder.SearchObjectsInRage<EnemyObject>();
    //}


}
