using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(CardFlip))]
public abstract class UiCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    protected int _cardCost;
    [SerializeField]
    private bool isFlippable;
    [SerializeField] private List<SunAndSkull> sunAndSkullTexts;
    protected HeroData _heroData;
    public bool AnimateInHeroEnd = false;
    public bool IsClickable = true;
    [SerializeField] protected bool IsReactCard;
    [SerializeField] protected GameObject ChildCardForAnimation; //Вставить сюда дочернюю карту, надо чтобы не было конфликтов анимации у LeanTween
    protected UiSelectHandler uiSelectHandler;

    protected virtual void OnEnable()
    {
        EventManager.Instance.Subscribe<HeroData>("OnHeroTurn", EnableTurnDisableReact);
        EventManager.Instance.Subscribe("OnEnemyTurn", EnableReactDisableTurn);
    }

    protected virtual void OnDisable()
    {
        EventManager.Instance.Unsubscribe<HeroData>("OnHeroTurn", EnableTurnDisableReact);
        EventManager.Instance.Unsubscribe("OnEnemyTurn", EnableReactDisableTurn);
    }

    public void setUpHeroData(HeroData heroData)
    {
        this._heroData = heroData;
        uiSelectHandler = UiSelectHandler.Instance;
        Debug.Log("Test cardName: " + this.name + "/" +  heroData.FieldHero.name);
        //uiSelectHandler.Hero = heroData;

        foreach (var sunSkull in sunAndSkullTexts)
        {
            sunSkull.SetUpHeroData(heroData);
        }
        //todo remake
        //foreach (var item in sunAndSkullTexts)
        //{
        //    item.SetUpHeroData(_heroData);
        //}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UtilClass.PlayClickAnimation(this.gameObject);
        if (!this._heroData.FieldHero.IsOwner) return;
        //todo refactor uiSelectHandler.Hero
        uiSelectHandler.Hero = this._heroData;

        bool isCardExecuted = HandlePointerClick(eventData);
        
        if (isCardExecuted)
        {
            EventManager.Instance.TriggerEvent("HeroReacted");
        }

        if (isFlippable && IsClickable && isCardExecuted)
        {
            this.gameObject.GetComponent<CardFlip>().FlipCard();
            IsClickable = false;
        }

    }

    protected abstract bool HandlePointerClick(PointerEventData eventData);

    // Метод для увелечения карты
   

    protected void EnableTurnDisableReact(HeroData heroData)
    {
        if (_heroData != null && _heroData == heroData)
        {
            IsClickable = !IsReactCard;
            if (!gameObject.GetComponent<CardFlip>().IsFaceUp)
            {
                Debug.Log("Попытка перевернуть карту");
                gameObject.GetComponent<CardFlip>().FlipCard();
            }
        }
    }
    protected void EnableReactDisableTurn()
    {
        if (_heroData != null)
        {
           IsClickable = IsReactCard;
        }
    }

    //todo интерфейс?
    public IEnumerator AnimateInEnd()
    {
        if (!AnimateInHeroEnd) yield break;
        //todo нормальная анимация
        yield return UtilClass.PlayClickAnimation(this.gameObject, 1.2f, 2f);
    }
    protected bool ConditionsCheck(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return false;
        }
        
        if (SelectControllerManager.Instance.currentMode != SelectionMode.Free)
        {
            return false;
        }

        if (_heroData.CurrentState != HeroState.Idle)
        {
            return false;
        }

        if (!IsClickable || _heroData.IsMakingChoice)
        {
            Debug.Log(_heroData.IsMakingChoice);
            return false;
        }

        return true;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<CardFlip>().IsFaceUp)
        {
            transform.SetAsLastSibling();
            ChildCardForAnimation.GetComponent<RectTransform>().LeanScale(new Vector3(2, 2, 2), 0.2f).setEase(LeanTweenType.easeOutBack);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponent<CardFlip>().IsFaceUp)
        {
            ChildCardForAnimation.GetComponent<RectTransform>().LeanScale(new Vector3(1, 1, 1), 0.2f).setEase(LeanTweenType.easeOutBack);
        }
    }

}
