using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(FieldObject))]
public class DeffendingDicePool : NetworkBehaviour
{
    public List<DiceData> Dices { get; private set; } = new List<DiceData>();
    protected FieldObject _fieldObject;
    [SerializeField]
    private DeffendingDice[] baseDeffendingDices;

    public void AddBaseDices()
    {
        AddDefDices(baseDeffendingDices);
    }

    private void AddDefDices(DeffendingDice[] diceNumbers)
    {
        if (!IsServer)
            return;
        
        //base.OnNetworkSpawn();
        foreach (var baseDefDice in baseDeffendingDices)
        {
            InstantiateAndAddDiceToDicePool((int) baseDefDice, false);
        }

        this.HideDicesRpc(0);
        ResetDicesPosition(Constants.MID_ABOVE_CHARACTER);   
    }

    //public List<DiceSide> GetPoolRollResult()
    //{
    //    List<DiceSide> result = new List<DiceSide>();
    //    foreach (var dice in Dices)
    //    {
    //        result.Add(dice.GetRollResult());
    //        //todo delete
    //        Debug.Log("res " + dice.GetRollResult().shields);
    //    };
    //    return result;
    //}

    //[Rpc(SendTo.Server)]
    public void ResetDicesPosition(float distanceAboveCharacter)
    {
        float dist = 0.4f;
        if (Dices != null)
        {
            //case 1
            if (Dices.Count == 1)
            {
                Dices[0].gameObject.transform.localPosition = new Vector3(0, distanceAboveCharacter, 0f);
            }
            if (Dices.Count == 2)
            {
                Dices[0].gameObject.transform.localPosition = new Vector3(-dist, distanceAboveCharacter, 0);
                Dices[1].gameObject.transform.localPosition = new Vector3(dist, distanceAboveCharacter, 0);
            }
            if (Dices.Count == 3)
            {
                Dices[0].gameObject.transform.localPosition = new Vector3(dist, distanceAboveCharacter, dist);
                Dices[1].gameObject.transform.localPosition = new Vector3(-dist, distanceAboveCharacter, dist);
                Dices[2].gameObject.transform.localPosition = new Vector3(0, distanceAboveCharacter, -dist);
            }
            if (Dices.Count == 4)
            {
                Dices[0].gameObject.transform.localPosition = new Vector3(dist, distanceAboveCharacter, dist);
                Dices[1].gameObject.transform.localPosition = new Vector3(-dist, distanceAboveCharacter, dist);
                Dices[2].gameObject.transform.localPosition = new Vector3(-dist, distanceAboveCharacter, -dist);
                Dices[3].gameObject.transform.localPosition = new Vector3(dist, distanceAboveCharacter, -dist);
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void RollAllDicesRpc()
    {
        this.ShowDices();
        foreach (var dice in Dices)
        {
            dice.RollDiceRpc();
        }
    }

    //public List<DiceSide> GetRollResults()
    //{
    //    List<DiceSide> result = new List<DiceSide>();
    //    foreach (var dice in Dices)
    //    {
    //        result.Add(dice.GetRollResult());
    //    }
    //    return result;
    //}

    protected void InstantiateAndAddDiceToDicePool(int diceNumber, bool attDice)
    {
        if (!IsServer)
        {
            return;
        }
        GameObject dicePrefab = null;

        if (attDice)
        {
            //dicePrefab = GameObject.Instantiate(NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists[1].PrefabList[diceNumber].Prefab);
            dicePrefab = GameObject.Instantiate(DiceManager.Instance.AttackingDicePrefabs.PrefabList[diceNumber].Prefab);
        }
        else
        {
            //dicePrefab = GameObject.Instantiate(NetworkManager.Singleton.NetworkConfig.Prefabs.NetworkPrefabsLists[2].PrefabList[diceNumber].Prefab);
            dicePrefab = GameObject.Instantiate(DiceManager.Instance.DeffendingDicePrefabs.PrefabList[diceNumber].Prefab);
        }

        var diceNO = dicePrefab.GetComponent<NetworkObject>();
        diceNO.Spawn();
        diceNO.TrySetParent(this.transform, false);

        NetworkObjectReference diceNOR = new NetworkObjectReference(diceNO);

        dicePrefab.transform.LeanScale(Constants.DEF_SMALL_CUBE_SIZE, 0);

        InstantiateAndAddDiceToDicePoolClientRpc(diceNOR);
    }

    [Rpc(SendTo.Everyone)]
    private void InstantiateAndAddDiceToDicePoolClientRpc(NetworkObjectReference diceNOR)
    {
        StartCoroutine(WaitAndAddDice(diceNOR));
    }

    private IEnumerator WaitAndAddDice(NetworkObjectReference diceNOR)
    {
        NetworkObject diceNO = null;
        //float timeout = 2f;
        //float timer = 0f;

        while (!diceNOR.TryGet(out diceNO))
        {
            Debug.Log("Waiting for dices");
            yield return null;
        }

        if (diceNO != null)
        {
            Dices.Add(diceNO.GetComponent<DiceData>());
        }
        else
        {
            Debug.LogWarning("❌ Не удалось получить NetworkObject из NetworkObjectReference на клиенте.");
        }
    }



    public void RemoveAllDices()
    {
        foreach (var dice in Dices)
        {
            LeanTween.scale(dice.gameObject, Vector3.zero, 1f) // Увеличиваем до 0% за 1 секунду
                .setEase(LeanTweenType.easeOutBack);
            LeanTween.delayedCall(1f, () => GameObject.Destroy(dice.gameObject));
        }
    }

    public List<DiceData> GetDices()
    {
        return Dices;
    }

    // time - время исчезновения
    [Rpc(SendTo.Server)]
    public void HideDicesRpc(float time)
    {
        EnableNetworkTransformRpc();

        if (Dices != null && Dices.Count > 0)
        {
            foreach (var dice in Dices)
            {
                LeanTween.scale(dice.gameObject, Vector3.zero, time) // Уменьшаем до 0% за time секунд
                    .setEase(LeanTweenType.easeOutBack)
                    .setOnComplete(() => dice.transform.localScale = Vector3.zero);

                //LeanTween.delayedCall(1f, () => dice?.gameObject?.SetActive(false));
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void EnableNetworkTransformRpc()
    {
        if (Dices != null && Dices.Count > 0)
        {
            foreach (var dice in Dices)
            {
                dice.GetComponent<NetworkTransform>().enabled = true;
                //LeanTween.delayedCall(1f, () => dice?.gameObject?.SetActive(false));
            }
        }
    }

    public void ShowDices()
    {
        if (Dices != null && Dices.Count > 0)
        {
            foreach (var dice in Dices)
            {
                dice.transform.localScale = Vector3.zero; // Исходный размер 0
                LeanTween.scale(dice.gameObject, Constants.DEF_SMALL_CUBE_SIZE, 1f) // Увеличиваем до 100% за 1 секунду
                    .setEase(LeanTweenType.easeOutBack); // Красивый "вылетающий" эффект

                dice.gameObject.SetActive(true);
            }
        }
    }

}
