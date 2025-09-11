using UnityEngine;

public class EnemyActionsBar : SegmentedBar
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

    public void SetUpEnemyActionsBar(int actions)
    {
        UpdateSegmentsNoAnimation(actions, vlg);
        this.gameObject.SetActive(false);
    }

    public void HandleActionsChange(int actions)
    {
        StartCoroutine(UpdateSegments(actions, vlg));
    }
}
