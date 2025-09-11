using UnityEngine;

public class CardFlip : MonoBehaviour
{
    [SerializeField]
    GameObject frontSide;
    [SerializeField]
    GameObject backSide;

    public bool IsFaceUp { get; private set; } = true;

    public void Awake()
    {
        backSide.LeanScaleX(0, 0f);
        IsFaceUp = true;
    }

    public void FlipCard() {
        if (IsFaceUp)
        {
            frontSide.LeanScaleX(0f, 0.2f).setOnComplete(() =>
            {
                IsFaceUp = !IsFaceUp;
                backSide.LeanScaleX(1f, 0.2f);
            });
        } else
        {
            backSide.LeanScaleX(0f, 0.2f).setOnComplete(() =>
            {
                IsFaceUp = !IsFaceUp;
                frontSide.LeanScaleX(1f, 0.2f);
            });
        }

    }
}
