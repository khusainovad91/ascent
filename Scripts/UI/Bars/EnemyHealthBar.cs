using System.Collections;
using UnityEngine;

public class EnemyHealthBar : SegmentedBar
{
    [SerializeField] bool vlg = false;

    private void Awake()
    {
        Inititalize();
    }

    protected void Inititalize()
    {
        base.Inititalize(vlg);
        maxAmountOfSegments = 10;
    }

    public void SetUpEnemyHealthBar(int maxHp)
    {
        UpdateSegmentsNoAnimation(maxHp, vlg);
        //this.gameObject.SetActive(false);
        UtilClass.LeanPopDown(this.gameObject);
    }

    public IEnumerator HandleHpChange(int hp)
    {
        yield return UpdateSegments(hp, vlg);
    }

}
