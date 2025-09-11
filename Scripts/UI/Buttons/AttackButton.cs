using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AttackButton : MonoBehaviour
{
    HeroAttackCommand hac;
    public void SetUp(HeroAttackCommand hac)
    {
        this.hac = hac;
    }

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(ExecuteAttack);
    }

    private void ExecuteAttack()
    {
        if (hac.FieldHero.HeroData.IsMakingChoice)
        {
            return;
        }
        CommandManager.Instance.AddCommand(hac);
        this.gameObject.SetActive(false);
    }

    private void ToggleOffThisButton(HeroAttackCommand hac)
    {
        if (this.hac == hac)
        {
            this.gameObject.SetActive(false);
        }
    }
}
