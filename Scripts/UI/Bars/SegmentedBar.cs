using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.HableCurve;

//todo сейчас сделанно по усти только дл€ vlg, вызываетс€ VlgSegmentsExpandeController
public abstract class SegmentedBar : MonoBehaviour
{
    [SerializeField] protected GameObject segment;
    protected int maxAmountOfSegments;
    protected HorizontalOrVerticalLayoutGroup lg { get; set; }
    protected List<GameObject> segments { get; set; }
    public float animationTime { get; private set; }

    //”казать lg и segments дл€ работы этого класса
    protected void Inititalize(bool vlg)
    {
        segments = new List<GameObject>();
        if (vlg)
        {
            lg = this.gameObject.GetComponent<VerticalLayoutGroup>();
        } else
        {
            lg = this.gameObject.GetComponent<HorizontalLayoutGroup>();
        }
    }

    protected void AddSegment()
    {
        var currentSegment = Instantiate(segment, this.gameObject.transform);
        segments.Add(currentSegment);
    }

    protected void LgSegmentsExpandeController(int maxAmountOfSegments, bool vlg)
    {
        if (vlg)
        {
            if (segments.Count() > maxAmountOfSegments)
            {
                lg.childControlHeight = true;
            }
            else
            {
                lg.childControlHeight = false;
            }
        } else
        {
            if (segments.Count() > maxAmountOfSegments)
            {
                lg.childControlWidth = true;
            }
            else
            {
                lg.childControlWidth = false;
            }
        }
    }

    //обновл€ет количество сегментов в заивисмости от потерь/добавлений
    protected IEnumerator UpdateSegments(int characterStat, bool vlg, float completeTime = 0.5f)
    {
        if (segments.Count > characterStat)
        {
            yield return RemoveSegmentsOneByOne(characterStat, 1f);
        }
        else if (segments.Count() < characterStat)
        {
            while (segments.Count != characterStat)
            {
                AddSegment();
            }
        }

        LgSegmentsExpandeController(maxAmountOfSegments, vlg);
    }

    protected void UpdateSegmentsNoAnimation(int characterStat, bool vlg)
    {
        if (segments.Count > characterStat)
        {
            while (segments.Count != characterStat)
            {
                RemoveSegmentNoAnimation();
            }
        }
        else if (segments.Count() < characterStat)
        {
            while (segments.Count != characterStat)
            {
                AddSegment();
            }
        }

        LgSegmentsExpandeController(maxAmountOfSegments, vlg);
    }

    private IEnumerator RemoveSegmentsOneByOne(int characterStat, float time)
    {
        if (segments.Count == 0) yield break;

        animationTime = (segments.Count - characterStat) * time;

        while (segments.Count > characterStat && segments.Count > 0)
        {
            yield return RemoveSegment(characterStat, time);
        }

        animationTime = 0;
    }
    protected void RemoveSegmentNoAnimation()
    {
        var lastSegment = segments.Last();
        segments.Remove(lastSegment);
        Destroy(lastSegment.gameObject);
    }

    protected IEnumerator RemoveSegment(int characterStat, float time)
    {
        var lastSegment = segments.Last();
        segments.Remove(lastSegment);

        LeanTween.scale(lastSegment.gameObject, Vector3.zero, time).
            setEase(LeanTweenType.easeInBack).setOnComplete(() =>
            {
                Destroy(lastSegment.gameObject); // ”ничтожаем объект после анимации
            });
        yield return new WaitForSeconds(time);
    }

    protected void HlgSegmentsExpandeController()
    {
        throw new NotImplementedException();
    }

}
