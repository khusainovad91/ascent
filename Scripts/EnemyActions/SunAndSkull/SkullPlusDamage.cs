using UnityEngine;

[CreateAssetMenu(menuName = "Skulls/PlusDmg")]
public class SkullPlusDamage : SkullAbility
{
    [field:SerializeField]
    public override int SkullCost { get; set; }
    [field:SerializeField]
    public override string Text { get; set; }
    [field:SerializeField]
    public override int Weight { get; set; }
    [field: SerializeField]
    public override float AnimationTime { get; set; }

    [SerializeField]
    private int damage;

    public override bool Use(AttackAction aa)
    {
        if (aa.Skulls - SkullCost < 0) return false;
        aa.Skulls -= SkullCost;
        aa.TotalDamage += damage;
        return true;
    }
}
