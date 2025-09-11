using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(LookAtLocalCamera))]
public abstract class DiceData : NetworkBehaviour
{
    protected List<DiceSide> sides;
    public Dictionary<DiceSide, Vector3> SideToRotate;

    [SerializeField]
    protected float animationDuration = Constants.DICE_ROLL_TIME;
    [SerializeField]
    protected float animationEnds = 1f;
    [SerializeField]
    protected float rotationSpeed = 20f;
    [SerializeField]
    private ParticleSystem rollParticles;
    public Outline Outline;
    private Color _defaultColor = Color.white;

    public NetworkVariable<DiceSide> LastRollResult = new NetworkVariable<DiceSide>(
        new DiceSide(0, 0, 0, 0, 0, 0, false),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        LastRollResult.OnValueChanged += (oldVal, newVal) =>
        {
            Debug.Log($"[CLIENT {OwnerClientId}] [DICENAME {this.name}] 🎲 Кубик обновился: {newVal}");
        };
        Outline = GetComponent<Outline>();
    }

    //public DiceSide GetRollResult()
    //{
    //    return LastRollResult;
    //}

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RollDiceRpc()
    {
        SetRandomSideAsLastRollResult();
        //this.GetComponent<DiceFaceCamera>().enabled = false;
        ShowResultToLocalCameraRpc();
        //Устанавливаем следующее значение
    }

    [Rpc(SendTo.Everyone)]
    private void ShowResultToLocalCameraRpc()
    {
        FaceLocalCamera(false);
        StartCoroutine(ShowResult());
    }

    private void FaceLocalCamera(bool face)
    {
        this.GetComponent<LookAtLocalCamera>().FaceCamera = face;
    }

    protected void SetRandomSideAsLastRollResult()
    {
        int rand = Random.Range(0, sides.Count);
        LastRollResult.Value = sides[rand];
    }

    protected IEnumerator ShowResult()
    {

        while (LeanTween.isTweening(this.gameObject))
        {
            yield return null;
        }

        transform.LeanScale(Constants.NORMAL_CUBE_SIZE, 0.2f).setEase(LeanTweenType.easeOutBack);
        Vector3 randomRotation = new Vector3(
            Random.Range(720, 1440) * (Random.value > 0.5f ? 1 : -1), // X: вперед или назад
            Random.Range(720, 1440) * (Random.value > 0.5f ? 1 : -1), // Y: по или против часовой
            Random.Range(720, 1440) * (Random.value > 0.5f ? 1 : -1)  // Z: по или против часовой
        );
        transform.LeanRotate(randomRotation, 1.5f).setEase(LeanTweenType.linear);
        yield return new WaitForSeconds(1.5f);

        transform.gameObject.transform.LeanRotate(SideToRotate[LastRollResult.Value], 0.5f).setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => {
                this.GetComponent<NetworkTransform>().enabled = false;
                FaceLocalCamera(true); 
            });
        
        //this.GetComponent<DiceFaceCamera>().enabled = true;
    }

    public void SetDefaultColor()
    {
        Outline.OutlineColor = _defaultColor;
    }
}

