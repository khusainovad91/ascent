using UnityEngine;

public abstract class SkullSunAbility : ScriptableObject
{
    public abstract float AnimationTime { get; set; }
    public abstract int Weight { get; set; }
    public abstract string Text { get; set; }
    public abstract bool Use(AttackAction aa);
}
