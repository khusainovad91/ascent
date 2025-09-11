using UnityEngine;

public abstract class SkullAbility : SkullSunAbility
{
    public abstract int SkullCost { get; set; }
    public override abstract string Text { get; set; }

    public override abstract bool Use(AttackAction aa);
}
