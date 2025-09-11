using System;
using Unity.Netcode;
using UnityEngine;

public struct DiceSide : INetworkSerializable
{
    public int diceSideNumber;
    public int range;
    public int hits;            
    public int suns;
    public int skulls;
    public int shields;
    public bool miss;

    public DiceSide(int diceSideNumber, int range, int hits, int suns, int skulls, int shields, bool miss)
    {
        this.diceSideNumber = diceSideNumber;
        this.range = range;
        this.hits = hits;
        this.suns = suns;
        this.skulls = skulls;
        this.shields = shields;
        this.miss = miss;
    }

    // Промах
    public DiceSide(int diceSideNumber, bool miss)
    {
        this.diceSideNumber = diceSideNumber;

        this.range = 0;
        this.hits = 0;
        this.suns = 0;
        this.skulls = 0;
        this.shields = 0;
        this.miss = true;
    }

    // Атакующий кубик
    public DiceSide(int diceSideNumber, int hearts, int range, int suns, int skulls)
    {
        this.diceSideNumber = diceSideNumber;

        this.range = range;
        this.hits = hearts;
        this.suns = suns;   
        this.skulls = skulls;
        this.shields = 0;
        this.miss = false;
    }

    // Обораняющийся
    public DiceSide(int diceSideNumber, int shields)
    {
        this.diceSideNumber = diceSideNumber;

        this.range = 0;
        this.hits = 0;
        this.suns = 0;
        this.skulls = 0;
        this.shields = shields;
        this.miss = true;
    }

    public override string ToString()
    {
        return base.ToString() +
            "[" + "range: " + range + "]" + "\n" +
            "[" + "hearts: " + hits + "]" + "\n" +
            "[" + "suns: " + suns + "]" + "\n" +
            "[" + "skulls: " + skulls + "]" + "\n" +
            "[" + "shields: " + shields + "]" + "\n";
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref diceSideNumber);
        serializer.SerializeValue(ref range);
        serializer.SerializeValue(ref hits);
        serializer.SerializeValue(ref suns);
        serializer.SerializeValue(ref skulls);
        serializer.SerializeValue(ref shields);
        serializer.SerializeValue(ref miss);
    }

    // Переопределение Equals
    public override bool Equals(object obj)
    {
        if (obj is DiceSide other)
        {
            return diceSideNumber == other.diceSideNumber &&
                   range == other.range &&
                   hits == other.hits &&
                   suns == other.suns &&
                   skulls == other.skulls &&
                   shields == other.shields &&
                   miss == other.miss;
        }
        return false;
    }

    // Переопределение GetHashCode
    public override int GetHashCode()
    {
        return HashCode.Combine(diceSideNumber, range, hits, suns, skulls, shields, miss);
    }
}
