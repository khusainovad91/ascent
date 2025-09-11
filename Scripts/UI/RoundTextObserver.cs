using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundTextObserver : Singleton<RoundTextObserver>
{
    [SerializeField] Image _goodBackGround;
    [SerializeField] Image _badBackGround;
    [SerializeField] TextMeshProUGUI _text;

    protected override void Awake()
    {
        base.Awake();
        EventManager.Instance.Subscribe("OnRoundStart", HandleStartRound);
        EventManager.Instance.Subscribe("OnEnemyTurn", HandleEnemyTurn);
        
        Debug.Log("Пописался");
    }

    private void Start()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0);
    }
    private void HandleEnemyTurn()
    {
        _text.text = "Ход Мердоков";
        _text.color = Constants.ColorMap[Colorr.Yellow];
        _badBackGround.gameObject.SetActive(true);
        _goodBackGround.gameObject.SetActive(false);
        UtilClass.LeanPopUp(this.gameObject, LeanTweenType.easeOutQuad);
        LeanTween.delayedCall(1.5f, () => UtilClass.LeanPopDown(this.gameObject));
    }

    private void HandleStartRound()
    {
        _text.text = "Ход Героев";
        _text.color = Constants.ColorMap[Colorr.SlightlyGreen];
        _badBackGround.gameObject.SetActive(false);
        _goodBackGround.gameObject.SetActive(true);
        UtilClass.LeanPopUp(this.gameObject, LeanTweenType.easeOutQuad);
        LeanTween.delayedCall(1.5f, () => UtilClass.LeanPopDown(this.gameObject));
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe("OnRoundStart", HandleStartRound);
        EventManager.Instance.Unsubscribe("OnEnemyTurn", HandleEnemyTurn);
    }



}
