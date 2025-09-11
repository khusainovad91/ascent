using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FatigueToMpButton : UiButton
{

    private new void Awake()
    {
        base.Awake();
        this.GetComponent<Button>().onClick.AddListener(AddMovementPoint);
    }

    private void AddMovementPoint()
    {
        if (SelectControllerManager.Instance.currentMode != SelectionMode.Free) { return; }
        if (_heroData.CurrentState != HeroState.Idle) return;
        
        if (_heroData.Stats.fatigue < _heroData.Stats.maxFatigue)
        {
            UtilClass.PlayClickAnimation(this.gameObject);
            {
                _heroData.Stats.ChangeFatigueRpc(+1);
                _heroData.Stats.ChangeMovementPointsRpc(+1);
            }
        }
    }
}
