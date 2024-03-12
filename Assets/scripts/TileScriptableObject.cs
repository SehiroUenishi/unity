using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileScriptableObject : ScriptableObject
{
    public List<TileMaster> tileDataList = new List<TileMaster>();
}
[Serializable]
public class TileMaster
{
    public string head;
    public Tile tile;
}
