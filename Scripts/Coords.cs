using System;
using Unity.Netcode;
using UnityEngine;

public struct Coords : INetworkSerializable
{
    public int x;
    public int y;
    public int z;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref x);
        serializer.SerializeValue(ref y);
        serializer.SerializeValue(ref z);
    }

    public Coords(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(x, y, z);
    }

    public static Coords Vector3IntToCoords(Vector3Int vector3Int) 
    {
        return new Coords(vector3Int.x, vector3Int.y, vector3Int.z);
    }
}
