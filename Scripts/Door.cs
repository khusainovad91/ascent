using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(RightClickHandler))]
[RequireComponent(typeof(FieldStaticObject))]
public class Door : Wall
{
    public NetworkVariable<bool> isClosed = new NetworkVariable<bool>(true);
    [SerializeField]
    bool isClosedOnSpawn = true;
    public bool NeedKey = false;
    public KeyType KeyToOpen;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isClosed.OnValueChanged += (oldVal, newVal) => onWallValueChanged();
        isClosed.Value = isClosedOnSpawn;
        //todo delete
        //this.GetComponent<RightClickHandler>().AddNewCommandRpc(CommandType.OpenDoor);
        ChangeDoorStatus();
    }

    [Rpc(SendTo.Server)]
    public void ChangeWallStateRpc(bool isClosed)
    {
        this.isClosed.Value = isClosed;
        ChangeDoorStatus();
    }

    private void ChangeDoorStatus()
    {
        if (this.isClosed.Value)
        {
            this.GetComponent<RightClickHandler>().AddNewCommandRpc(CommandType.OpenDoor);
            this.GetComponent<RightClickHandler>().RemoveCommandRpc(CommandType.CloseDoor);
        }
        else
        {
            this.GetComponent<RightClickHandler>().AddNewCommandRpc(CommandType.CloseDoor);
            this.GetComponent<RightClickHandler>().RemoveCommandRpc(CommandType.OpenDoor);
        }
        RotateWallByZ(this.isClosed.Value);
    }

    private void RotateWallByZ(bool isClosed)
    {
        if (isClosed)
        {
            if (!XWall)
            {
                //this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                //LeanTween.rotate(this.gameObject, new Vector3(0, 0, 90), 0.5f);
                //this.transform.LeanRotateZ(90, 0.5f);

                Quaternion targetRotation = Quaternion.Euler(0, 0, 90);
                LeanTween.rotateLocal(gameObject, targetRotation.eulerAngles, 0.5f);
            }
            else
            {
                //this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                //LeanTween.rotate(this.gameObject, new Vector3(0, 0, 180), 0.5f);
                //this.transform.LeanRotateZ(180, 0.5f);

                Quaternion targetRotation = Quaternion.Euler(0, 0, 180);
                LeanTween.rotateLocal(gameObject, targetRotation.eulerAngles, 0.5f);
            }
        }
        else
        {
            if (!XWall)
            {
                //this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -15));
                //LeanTween.rotate(this.gameObject, new Vector3(0, 0, -15), 0.5f);
                //this.transform.LeanRotateZ(-15, 0.5f);

                Quaternion targetRotation = Quaternion.Euler(0, 0, -15);
                LeanTween.rotateLocal(gameObject, targetRotation.eulerAngles, 0.5f);
            }
            else
            {
                //this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 75));
                //LeanTween.rotate(this.gameObject, new Vector3(0, 0, 75), 0.5f);
                //this.transform.LeanRotateZ(75, 0.5f);

                Quaternion targetRotation = Quaternion.Euler(0, 0, 75);
                LeanTween.rotateLocal(gameObject, targetRotation.eulerAngles, 0.5f);
            }
        }
    }

    internal void onWallValueChanged()
    {
        ActivateWall(isClosed.Value);
        EventManager.Instance.TriggerEvent("RecalculateMovement");
    }
}


public enum KeyType
{
    YellowKey,
    RedKey,
    BlueKey
}