using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(EnemyObject))]
public class EnemyStats : Stats
{
    //todo переделать на List<Cube>
    //todo 
    //intelect lvl
    [field: SerializeField]
    public EnemyHealthBar HealthBar { get; private set; }
    [field: SerializeField]
    public EnemyMpBar MpBar { get; private set; }
    [field: SerializeField]
    public EnemyActionsBar ActionsBar { get; private set; }
    [field: SerializeField]
    public EnemyConditionBar EnemyConditionBar { get; private set; }

    [field: SerializeField]
    public bool IsMelee { get; private set; } = true;
    [field: SerializeField]
    public bool Coward;
    [field:SerializeField]
    public int AttackRange { get; private set; } = 1;

    public override void OnNetworkSpawn()
    {
        HealthBar.SetUpEnemyHealthBar(MaxHP);
        MpBar.SetUpEnemyMpBar(MovementPoints);
        ActionsBar.SetUpEnemyActionsBar(ActionsAmount);
    }


    //todo delete
    [Rpc(SendTo.Server)]
    public void ChangeActionsAmountRpc(int actionsAmount)
    {
        ChangeActionsAmountClientRpc(actionsAmount);
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeActionsAmountClientRpc(int actionsAmount)
    {
        if (actionsAmount + ActionsAmount < 0)
        {
            throw new System.Exception("actionAmount < 0");
        }
        ActionsAmount += actionsAmount;
        ActionsBar.HandleActionsChange(ActionsAmount);
    }

    [Rpc(SendTo.Server)]
    public void ChangeHealthRpc(int hp)
    {
        ChangeHealthClientRpc(hp);
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeHealthClientRpc(int hp)
    {
        this.Hp -= hp;
        Debug.Log(this.gameObject.name + " получил урона: " + hp);
        StartCoroutine(ChangeHealth(hp));
    }

    private IEnumerator ChangeHealth(int hp)
    {
        yield return HealthBar.HandleHpChange(this.Hp);

        if (this.Hp <= 0 && IsServer)
        {
            var thisEnemy = this.GetComponent<EnemyObject>();
            EventManager.Instance.TriggerEvent<EnemyObject>("EnemyDied", thisEnemy);
            thisEnemy.GetNetworkObject().Despawn();
        } else {
            this.GetComponent<EnemyObject>().OnMouseExit();
        }


    }

    //waiting
    public IEnumerator EnemyChangeMovementPoints(int mp)
    {
        yield return MpBar.HandleMpChange(this.MovementPoints + mp, 0f);
        this.MovementPoints += mp;
    }

    //todo delete
    //[Rpc(SendTo.Server)]
    //public void ChangeMovementPointsRpc(int mp)
    //{
    //    ChangeMovementPointsClientRpc(mp);
    //}

    //No waiting
    [Rpc(SendTo.Everyone)]
    public void ChangeMovementPointsClientRpc(int mp)
    {
        this.MovementPoints += mp;
        StartCoroutine(MpBar.HandleMpChange(this.MovementPoints));
    }

}

