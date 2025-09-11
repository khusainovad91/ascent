////todo remake
//using NUnit.Framework;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;

//public class UiSuns : MonoBehaviour
//{
//    [SerializeField] List<GameObject> children;
//    private void OnEnable()
//    {
//        if (EventManager.Instance != null)
//        {
//            EventManager.Instance.Subscribe<HeroAttackCommand>("OnSuns", EnableSuns);
//        }
//    }

//    private void OnDisable()
//    {
//        if (EventManager.Instance != null)
//        {
//            EventManager.Instance.Unsubscribe<HeroAttackCommand>("OnSuns", EnableSuns);
//        }
//    }

//    private void EnableSuns(HeroAttackCommand hac)
//    {
//        if (this.GetComponentInParent<FieldHero>() != hac.FieldHero)
//        {
//            return;
//        }
        
//        for (int i = 0; i < hac._suns; i++) {
//            if (i > children.Count - 1) {
//                throw new System.Exception("Jss Christ, Adel, you forgot to add suns");
//            }
//            children[i].SetActive(true);
//        }

//        hac.UiSuns = this;
//    }

//    public void TuggleOffSuns(int suns)
//    {
//        for (int i = suns; i > 0; i--)
//        {
//            children.Last(go => go.activeInHierarchy == true).SetActive(false);
//        }
//    }


//    public void TuggleOffAllSuns()
//    {
//        foreach (var item in children)
//        {
//            item.SetActive(false);
//        }
//    }
//}
