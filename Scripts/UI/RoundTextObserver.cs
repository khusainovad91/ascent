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
        EventManager.Instance.Subscribe("OnHeroesWon", HandleWin);
        EventManager.Instance.Subscribe("OnEnemiesWon", HandleLoose);
        EventManager.Instance.Subscribe("OnEnemyTurn", HandleEnemyTurn);
        
        Debug.Log("Подписался");
    }

    private void Start()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0);
    }

    private void HandleWin()
    {
        TextPopUp("Heroes won", true, false);
    }

    private void HandleLoose()
    {
        TextPopUp("Enemies won", false, false);
    }

    private void HandleEnemyTurn()
    {
        TextPopUp("Enemies turn", false, true);
    }

    private void HandleStartRound()
    {
        TextPopUp("Heroes turn", true, true);
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        EventManager.Instance.Unsubscribe("OnRoundStart", HandleStartRound);
        EventManager.Instance.Unsubscribe("OnEnemyTurn", HandleEnemyTurn);
        EventManager.Instance.Unsubscribe("OnHeroesWon", HandleLoose);
        EventManager.Instance.Unsubscribe("OnEnemiesWon", HandleWin);
    }

    private void TextPopUp(String text, bool goodBackGround, bool popDown)
    {
        _text.text = text;
        if (goodBackGround) {
            _text.color = Constants.ColorMap[Colorr.SlightlyGreen];
            _badBackGround.gameObject.SetActive(false);
            _goodBackGround.gameObject.SetActive(true);
        } else
        {
            _text.color = Constants.ColorMap[Colorr.Yellow];
            _badBackGround.gameObject.SetActive(true);
            _goodBackGround.gameObject.SetActive(false);
        }

        UtilClass.LeanPopUp(this.gameObject, LeanTweenType.easeOutQuad);
        if (popDown)
        {
            LeanTween.delayedCall(1.5f, () => UtilClass.LeanPopDown(this.gameObject));
        }
    }

}
