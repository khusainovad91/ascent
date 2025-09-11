using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UiReactOnMoveCard : UiPassiveCard
{            //todo delete
    //protected StepAction _stepAction;

    private void Awake()
    {
        IsReactCard = true;
    }
    //todo delete
    //public void SetUpReactCard(StepAction stepAction)
    //{
    //    this._stepAction = stepAction;
    //}

    //public void ClearStepAction()
    //{
    //    this._stepAction = null;
    //}

}
