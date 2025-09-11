using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


//TODO DELETE DICEPOOL
[RequireComponent(typeof(FieldObject))]
public abstract class DicePool: NetworkBehaviour
{
    //protected List<DiceData> _dices = new List<DiceData>();
    //protected FieldObject _fieldObject;

    //[Rpc(SendTo.Server)]
    //public void ResetDicesPositionRpc(float distanceAboveCharacter)
    //{
    //    if (_dices != null)
    //    {
    //        //case 1
    //        if (_dices.Count == 1)
    //        {
    //            _dices[0].gameObject.transform.localPosition = new Vector3(0, distanceAboveCharacter, -0.5f);
    //        }
    //        if (_dices.Count == 2)
    //        {
    //            _dices[0].gameObject.transform.localPosition = new Vector3(-0.5f, distanceAboveCharacter, -0.5f);
    //            _dices[1].gameObject.transform.localPosition = new Vector3(0.5f, distanceAboveCharacter, -0.5f);
    //        }
    //        //todo
    //        //case 2
    //        //case 3

    //        //case 4
    //        //case 5
    //    }
    //}

    //[Rpc(SendTo.Server)]
    //public void RollAllDicesRpc()
    //{
    //    this.ShowDices();
    //    List<DiceSide> result = new List<DiceSide>();
    //    foreach (var dice in _dices)
    //    {
    //        dice.RollDice();
    //    }
    //}

    ////public List<DiceSide> GetRollResults()
    ////{
    ////    List<DiceSide> result = new List<DiceSide>();
    ////    foreach (var dice in _dices)
    ////    {
    ////        result.Add(dice.GetRollResult());
    ////    }
    ////    return result;
    ////}

    //protected void InstantiateAndAddDiceToDicePool(int diceNumber, bool attDice)
    //{ 
    //    if (!IsServer)
    //    {
    //        return;
    //    }
    //    GameObject dicePrefab = null;

    //    if (attDice)
    //    {
    //        //dicePrefab = GameObject.Instantiate(NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists[1].PrefabList[diceNumber].Prefab);
    //        dicePrefab = GameObject.Instantiate(DiceManager.Instance.AttackingDicePrefabs.PrefabList[diceNumber].Prefab);
    //    } else
    //    {
    //        //dicePrefab = GameObject.Instantiate(NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists[2].PrefabList[diceNumber].Prefab);
    //        dicePrefab = GameObject.Instantiate(DiceManager.Instance.DeffendingDicePrefabs.PrefabList[diceNumber].Prefab);
    //    }

    //    var diceNO = dicePrefab.GetComponent<NetworkObject>();
    //    diceNO.Spawn();
    //    diceNO.TrySetParent(this.GetComponent<FieldObject>().Model.transform, true);

    //    NetworkObjectReference diceNOR = new NetworkObjectReference(diceNO);

    //    dicePrefab.transform.LeanScale(Constants.DEF_SMALL_CUBE_SIZE, 0);

    //    _dices.Add(diceNO.GetComponent<DiceData>());
    //    //InstantiateAndAddDiceToDicePoolClientRpc(diceNOR);
    //}

    //[Rpc(SendTo.Everyone)]
    //private void InstantiateAndAddDiceToDicePoolClientRpc(NetworkObjectReference diceNOR)
    //{
    //    diceNOR.TryGet(out NetworkObject diceNO);
    //    Debug.Log("Клиент получил кубы" + diceNO.GetComponent<DiceData>().name);
    //    _dices.Add(diceNO.GetComponent<DiceData>());
    //}

    ////[Rpc(SendTo.Everyone)]
    ////private void InstantiateAndAddDiceToDicePoolClientRpc(NetworkObjectReference diceNOR)
    ////{
    ////    StartCoroutine(WaitAndAddDice(diceNOR));
    ////}

    ////private IEnumerator WaitAndAddDice(NetworkObjectReference diceNOR)
    ////{
    ////    NetworkObject diceNO = null;
    ////    //float timeout = 2f;
    ////    //float timer = 0f;

    ////    while (!diceNOR.TryGet(out diceNO))
    ////    {
    ////        Debug.Log("Waiting for dices");
    ////        yield return null;
    ////    }

    ////    if (diceNO != null)
    ////    {
    ////        _dices.Add(diceNO.GetComponent<DiceData>());
    ////    }
    ////    else
    ////    {
    ////        Debug.LogWarning("❌ Не удалось получить NetworkObject из NetworkObjectReference на клиенте.");
    ////    }
    ////}

    //public void RemoveAllDices()
    //{
    //    foreach (var dice in _dices)
    //    {
    //        LeanTween.scale(dice.gameObject, Vector3.zero, 1f) // Увеличиваем до 0% за 1 секунду
    //            .setEase(LeanTweenType.easeOutBack);
    //        LeanTween.delayedCall(1f, () => GameObject.Destroy(dice.gameObject));
    //    }
    //}

    //public List<DiceData> GetDices() {
    //    return _dices;
    //}

    //// time - время исчезновения
    //[Rpc(SendTo.Server)]
    //public void HideDicesRpc(float time)
    //{
    //    if (_dices != null && _dices.Count > 0)
    //    { 
    //        foreach (var dice in _dices)
    //        {
    //            LeanTween.scale(dice.gameObject, Vector3.zero, time) // Уменьшаем до 0% за time секунд
    //                .setEase(LeanTweenType.easeOutBack);
    //            LeanTween.delayedCall(1f, () => dice?.gameObject?.SetActive(false));
    //        }
    //    }
    //}

    //public void ShowDices()
    //{
    //    if (_dices != null && _dices.Count > 0) 
    //    {
    //        foreach (var dice in _dices)
    //        {
    //            dice.transform.localScale = Vector3.zero; // Исходный размер 0
    //            LeanTween.scale(dice.gameObject, Constants.DEF_SMALL_CUBE_SIZE, 1f) // Увеличиваем до 100% за 1 секунду
    //                .setEase(LeanTweenType.easeOutBack); // Красивый "вылетающий" эффект

    //            dice.gameObject.SetActive(true);
    //        }
    //    }
    //}
}

