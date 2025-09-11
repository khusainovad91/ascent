using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(EnemyController))]
[RequireComponent(typeof(AttackingDicePool))]
[RequireComponent(typeof(DeffendingDicePool))]
public class EnemyObject : FieldObject
{
    [SerializeField]
    private GameObject _sunSkullTextSegmentPrefab;
    [field:SerializeField]
    public VerticalLayoutGroup hlzSunAndSkullTexts { get; private set; }
    //public DeffendingDicePool DeffendingDicePool {  get; private set; }
    //public AttackDicePool _attackingDicePool { get; private set; }


    [field:SerializeField]
    public List<SkullSunAbility> skullSunAbilities { get; private set; }
    //public Dictionary<SunAbilities, int> sunAbilities { get; private set; }
    public EnemyStats Stats { get; private set;}
    public GameObject RangeFromHero { get; private set; }

    public EnemyState currentState { get; private set; }
    public EnemyController EnemyController { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        defaultColor = Color.yellow;
        Stats = this.gameObject.GetComponent<EnemyStats>();
        currentState = EnemyState.Idle;
        EnemyController = GetComponent<EnemyController>();
        RangeFromHero = transform.Find("Canvas").transform.Find("Range").gameObject;
        RangeFromHero.SetActive(false);

        StartCoroutine(SpawnDicePoolsDelayed());
    }

    private IEnumerator SpawnDicePoolsDelayed()
    {
        yield return new WaitForSeconds(0.2f); // Можно 0.2f для надёжности

        GetComponent<AttackingDicePool>().EnemyAddAttackDices();
        GetComponent<DeffendingDicePool>().AddBaseDices();
    }

    public virtual List<IAction> GetSpecialActions()
    {
        return new List<IAction>();
    }

    private void Start()
    {
        //DeffendingDicePool = new DeffendingDicePool(this, _defendDicesPrefabs);
        //_attackingDicePool = new AttackDicePool(this);
        skullSunAbilities.Sort((a, b) => b.Weight.CompareTo(a.Weight));
        hlzSunAndSkullTexts = transform.Find("Canvas").transform.Find("Ability Text Group").GetComponent<VerticalLayoutGroup>();
        //Debug.Log("LOL " + hlzSunAndSkullTexts.name);

        foreach (var ability in skullSunAbilities)
        {
            GameObject sunSkullText = GameObject.Instantiate(_sunSkullTextSegmentPrefab, hlzSunAndSkullTexts.transform);
            sunSkullText.GetComponent<TextMeshProUGUI>().SetText(ability.Text);
            sunSkullText.SetActive(false);
        }
    }

    //------------------------------------------------------
    // CODE FOR HERO
    //------------------------------------------------------

    public void Targeted()
    {
        SetIsTargetedRpc(true);
        UtilClass.LeanPopUp(Stats.HealthBar.gameObject, LeanTweenType.easeOutBounce);
    }

    public void NotTargeted()
    {
        if (Stats.HealthBar.animationTime > 0)
        {
            StartCoroutine(Wait4HealthChange(Stats.HealthBar.animationTime + 0.5f));
        } else
        {
            SetIsTargetedRpc(false);
            UtilClass.LeanPopDown(Stats.HealthBar.gameObject);
        }
    }

    private IEnumerator Wait4HealthChange(float time)
    {
        yield return new WaitForSeconds(time);
        SetIsTargetedRpc(false);
        if (Stats?.HealthBar?.gameObject != null)
        {
            UtilClass.LeanPopDown(Stats.HealthBar.gameObject);
        }
    }

    public new void OnMouseEnter()
    {
        base.OnMouseEnter();
        if (!isTargeted.Value && GameManager.Instance.StateOfGame.Value == GameState.HeroTurn)
        {
            UtilClass.LeanPopUp(Stats.HealthBar.gameObject, LeanTweenType.easeOutBounce);
            UtilClass.LeanPopUp(Stats.EnemyConditionBar.gameObject, LeanTweenType.easeOutBounce);
        }

        if(!isTargeted.Value && SelectControllerManager.Instance.currentMode == SelectionMode.Enemy)
        {
            UtilClass.LeanPopUp(RangeFromHero, LeanTweenType.easeOutBounce);
        }

    }

    // Этот метод вызывается, когда курсор выходит за пределы объекта
    public new void OnMouseExit()
    {
        base.OnMouseExit();
        if (!isTargeted.Value && GameManager.Instance.StateOfGame.Value == GameState.HeroTurn)
        {
            UtilClass.LeanPopDown(Stats.HealthBar.gameObject);
            UtilClass.LeanPopDown(Stats.EnemyConditionBar.gameObject);
            UtilClass.LeanPopDown(RangeFromHero);
        }

        //enemyStats.HealthBar.gameObject.SetActive(false);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        EnemiesManager.Instance.RemoveEnemy(this);
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeStateRpc(EnemyState state)
    {
        currentState = state;
        //switch (currentState) {
        //    case EnemyState.EndedTurn:

        //}
    }
}

public enum EnemyState
{
    Start,
    Thinking,
    TryAttack,
    TryMove,
    Reevaluate,
    TrySpecial,
    TryEscape,
    RunToNearestHero,

    Idle,
    Moving,
    Attacking,
    SpecialCommand,
    EndedTurn
}