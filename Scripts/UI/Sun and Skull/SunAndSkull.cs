using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public abstract class SunAndSkull : MonoBehaviour, IPointerClickHandler
{
    protected HeroData _heroData;
    protected HeroAttackCommand _hac;

    protected virtual void OnEnable()
    {
        EventManager.Instance.Subscribe<HeroAttackCommand>("HacExecuted", RemoveHac);
    }

    protected virtual void OnDisable()
    {
        EventManager.Instance.Subscribe<HeroAttackCommand>("HacExecuted", RemoveHac);
    }


    private void RemoveHac(HeroAttackCommand hac)
    {
        ReturnToNormal();
        _hac = null;
    }

    public void SetUpHeroData(HeroData heroData)
    {
        _heroData = heroData;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        ReturnToNormal();
    }

    public void ReturnToNormal()
    {
        this.transform.LeanScale(Constants.VECTOR_1, 1).setOnComplete(() => LeanTween.cancel(this.gameObject));
    }

    protected void HighlightThis()
    {
        Debug.Log("Подсветка: " + this.name);
        LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 1)
            .setLoopPingPong()
            .setEase(LeanTweenType.easeOutBack);
    }
}
