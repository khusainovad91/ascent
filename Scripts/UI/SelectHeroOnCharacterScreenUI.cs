using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectHeroOnCharacterScreenUI : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] int heroId;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        UtilClass.PlayClickAnimation(this.gameObject);
        Debug.Log("ֱûכ גûבנאם דונמי: " + heroId);
        SelectHeroServerRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SelectHeroServerRpc(RpcParams rpcParams = default)
    {
        SelectCharacterScreenUI.Instance.SetNewHeroRpc(heroId, rpcParams.Receive.SenderClientId);
    }

    //todo delete
    //[Rpc(SendTo.Everyone)]
    //private void SelectHeroClientRpc()
    //{
    //    _selectCharacterScreenUI.SetNewHeroRpc(heroId);
    //}

}
