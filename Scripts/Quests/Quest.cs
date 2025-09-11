using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Quest: NetworkBehaviour
{
    [SerializeField]
    private bool _initialIsMainQuest;
    [SerializeField]
    private string _initialTitle;
    [SerializeField]
    private string _initialDescription;
    [SerializeField]
    private bool _initialIsThisActive;

    [HideInInspector]
    public NetworkVariable<bool> IsThisActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [HideInInspector]
    public NetworkVariable<bool> IsMainQuest = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [HideInInspector]
    public NetworkVariable<FixedString64Bytes> Title = new NetworkVariable<FixedString64Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private TextMeshProUGUI _tmpLink;
        
    public NetworkVariable<FixedString64Bytes> Description = new NetworkVariable<FixedString64Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    protected NetworkVariable<bool> _isCompleted = 
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        base.OnNetworkSpawn();
        IsMainQuest.Value = _initialIsMainQuest;
        Title.Value = _initialTitle; 
        Description.Value = (FixedString64Bytes) _initialDescription;
        IsThisActive.Value = _initialIsThisActive;
        if (IsThisActive.Value) SendNewQuestToUIClientRpc();

        _isCompleted.OnValueChanged += OnQuestComplete;
        //EventManager.Instance.Subscribe<>("OnGameStarted", OnGameStarted);
    }

    //private void OnGameStarted(object obj)
    //{
    //    if (IsServer)
    //    {
    //        if (IsThisActive.Value) SendNewQuestToUIClientRpc();
    //    }
    //}

    private void OnQuestComplete(bool previousValue, bool newValue)
    {
        Debug.Log($"{this.Description} is completed");
        //LeanTween.scale(_tmpLink.gameObject, Vector3.one * 1.2f, 0.2f).setEase(LeanTweenType.easeInBounce)
        //    .setOnComplete(() =>
        //    {
        //        _tmpLink.fontStyle
        //    }
        //    );
        // _tmpLink.text.
    }

    [Rpc(SendTo.Everyone)]
    private void SendNewQuestToUIClientRpc()
    {
        _tmpLink = UiQuestsManager.Instance.AddNewQuest(this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        _isCompleted.OnValueChanged -= OnQuestComplete;
    }

    [Rpc(SendTo.Everyone)]
    protected void UpdateUiClientRpc(String text)
    {
        _tmpLink.text = text;
    }
}
