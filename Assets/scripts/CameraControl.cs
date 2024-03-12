using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraControl : MonoBehaviour
{
    public Vector2 widthToBounds;

    [SerializeField] GameObject player;
    [SerializeField] Tilemap groundTilemap;
    BoundsInt groundBounds;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Initialize());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 cameraPos = gameObject.transform.position;

        if (groundBounds.min.x + widthToBounds.x + 2 <= player.transform.position.x && player.transform.position.x <= groundBounds.max.x - widthToBounds.x - 1)
        {
            cameraPos.x = player.transform.position.x;
        }
        if (groundBounds.min.y + widthToBounds.y + 2 <= player.transform.position.y && player.transform.position.y <= groundBounds.max.y - widthToBounds.y - 1)
        {
            cameraPos.y = player.transform.position.y;
        }

        gameObject.transform.position = cameraPos;
    }

    void GroundBoundsInit()
    {
        groundBounds = groundTilemap.cellBounds;
    }

    void CameraPosInit()
    {
        Vector3 cameraPos = player.transform.position;
        cameraPos.z = -10;

        if (groundBounds.min.x + widthToBounds.x + 2 >= player.transform.position.x)
        {
            cameraPos.x = groundBounds.min.x + widthToBounds.x + 2;
        }
        else if(player.transform.position.x >= groundBounds.max.x - widthToBounds.x - 1)
        {
            cameraPos.x = groundBounds.max.x - widthToBounds.x - 1;
        }
        if (groundBounds.min.y + widthToBounds.y + 2 >= player.transform.position.y)
        {
            cameraPos.y = groundBounds.min.y + widthToBounds.y + 2;
        }
        else if(player.transform.position.y >= groundBounds.max.y - widthToBounds.y - 1)
        {
            cameraPos.y = groundBounds.max.y - widthToBounds.y - 1;
        }

        gameObject.transform.position = cameraPos;
    }

    IEnumerator Initialize()
    {
        yield return null;

        GroundBoundsInit();
        CameraPosInit();
    }
}
