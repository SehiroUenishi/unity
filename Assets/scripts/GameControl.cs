using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class GameControl : MonoBehaviour
{
    public GameObject player;
    
    public float moveWaitTime;
    public float moveTime;

    private bool isPlayerMove = false;

    private float inverseMoveTime;
    private WaitForSeconds waitForSeconds;

    [SerializeField] Tilemap[] tilemap;
    [SerializeField] TileScriptableObject tileScriptableObject;

    const string SAVE_FILE = "saveData.json";
    const string DATA_DIR = "Assets/save/";
    static string saveDataPath = Path.Combine(DATA_DIR + SAVE_FILE);


    // Start is called before the first frame update
    void Start()
    {
        inverseMoveTime = 1f / moveTime;
        waitForSeconds = new WaitForSeconds(moveWaitTime);

        Load();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Save();
        }
    }

    // プレイヤー移動関連 ---------------------------------------------------------------

    public void PlayerMove(int v, int h)
    {
        if (!isPlayerMove)
        {
            isPlayerMove = true;
            Vector3Int posDiff = new Vector3Int(h, v, 0);
            Vector3 targetPos = player.transform.position + posDiff;
            StartCoroutine(PlayerMoving(targetPos));
        }
    }

    IEnumerator PlayerMoving(Vector3 targetPosition)
    {
        yield return waitForSeconds;

        while (player.transform.position != targetPosition)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, inverseMoveTime * Time.deltaTime);
            yield return null;
        }

        isPlayerMove = false;
    }

    // データロード関連 ---------------------------------------------------------------
    void Load()
    {
        // FileStreamからファイル読み込み
        FileStream stream = File.Open(saveDataPath, FileMode.Open);
        StreamReader streamReader = new StreamReader(stream);
        string jsonStr = streamReader.ReadToEnd();
        streamReader.Close();
        stream.Close();

        // jsonStrからデータに変換
        SaveData saveData = JsonUtility.FromJson<SaveData>(jsonStr);
        PlayerLoad(saveData);
        MapLoad(saveData);
    }

    void PlayerLoad(in SaveData sav)
    {
        player.transform.position = new Vector3(sav.playerGrid.x, sav.playerGrid.y, 0);
    }

    void MapLoad(in SaveData sav)
    {
        List<List<string>> tempMapData = new List<List<string>>();
        List<string> tempList = new List<string>();

        for (int i = 0; i < sav.mapData.Count; i++)
        {
            if (sav.mapData[i].Contains(";"))
            {
                tempMapData.Add(new List<string>(tempList));
                tempList.Clear();
                continue;
            }
            tempList.Add(sav.mapData[i]);
        }

        for (int i = 0; i < tempMapData.Count; i++)
        {
            tilemap[i].ClearAllTiles();

            // 一列ごとに保存されているので列の数確認
            for (int y = 0; y < tempMapData[i].Count; y++)
            {
                // 列を,で文字ごとに分けてstringに保存
                string[] str = tempMapData[i][y].Split(",");

                for (int x = 0; x < str.Length; x++)
                {
                    if (str[x] == " ") continue;
                    tilemap[i].SetTile(new Vector3Int(x + sav.mapMinGrid[i].x, y + sav.mapMinGrid[i].y, 0), tileScriptableObject.tileDataList.Single(tile => tile.head == str[x]).tile);
                }
            }
        }
    }

    // データセーブ関連 ---------------------------------------------------------------

    void Save()
    {
        SaveData saveData = new SaveData();
        PlayerSave(saveData);
        MapSave(saveData);

        if (!Directory.Exists(DATA_DIR))
        {
            Directory.CreateDirectory(DATA_DIR);
        }

        var jsonStr = JsonUtility.ToJson(saveData, true);

        StreamWriter streamWriter = new StreamWriter(saveDataPath, false);
        streamWriter.WriteLine(jsonStr);
        streamWriter.Flush();
        streamWriter.Close();
    }

    void PlayerSave(in SaveData sav)
    {
        sav.playerGrid = new Vector2(player.transform.position.x, player.transform.position.y);
    }

    void MapSave(in SaveData sav)
    {
        string str = "";

        for (int i = 0; i < tilemap.Length; i++)
        {
            tilemap[i].CompressBounds();

            BoundsInt mapBounds = tilemap[i].cellBounds;
            sav.mapMinGrid.Add(new Vector2Int(mapBounds.min.x, mapBounds.min.y));

            for (int y = mapBounds.min.y; y < mapBounds.max.y; y++)
            {
                for (int x = mapBounds.min.x; x < mapBounds.max.x; x++)
                {
                    if (tilemap[i].HasTile(new Vector3Int(x, y, 0)))
                    {
                        str += tileScriptableObject.tileDataList.Single(tileDL => tileDL.tile == tilemap[i].GetTile(new Vector3Int(x, y, 0))).head + ",";
                    }
                    else
                    {
                        str += " ,";
                    }
                }

                str = str.TrimEnd(',');
                sav.mapData.Add(str);
                str = "";
            }
            sav.mapData.Add(";");
        }
    }


    [Serializable]
    class SaveData
    {
        public List<string> mapData = new List<string>();
        public List<Vector2Int> mapMinGrid = new List<Vector2Int>();

        public Vector2 playerGrid = new Vector2();
    }
}


