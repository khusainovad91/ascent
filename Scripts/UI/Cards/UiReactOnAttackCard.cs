using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UiReactOnAttackCard : UiCard
{
    public AttackAction Aa { get; private set; }

    private void Awake()
    {
        IsReactCard = true;
    }

    public void SetUpReactCard(AttackAction aa)
    {
        Aa = aa;
    }

    public void ClearAa()
    {
        Aa = null;
    }
}
