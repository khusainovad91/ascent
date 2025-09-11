using System.Collections;
using UnityEngine;

public class EnemyMpBar : SegmentedBar
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

    public void SetUpEnemyMpBar(int mp)
    {
        UpdateSegmentsNoAnimation(mp, vlg);
        this.gameObject.SetActive(false);
    }

    public IEnumerator HandleMpChange(int mp, float completeTime = 0.5f)
    {
        return UpdateSegments(mp, vlg, completeTime);
    }
}
