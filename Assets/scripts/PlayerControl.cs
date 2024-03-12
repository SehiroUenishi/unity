using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public GameControl gameControl;

    private Vector3 origin;
    private Vector2Int direction;

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        origin = gameObject.transform.position;
        direction = new Vector2Int(0, 0);

        //WSADŒŸ’m
        if (Input.GetKeyDown(KeyCode.W))
        {
            origin.y += 0.5f;
            direction = new Vector2Int(0, 1);
            KeyActive();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            origin.y -= 0.5f;
            direction = new Vector2Int(0, -1);
            KeyActive();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            origin.x -= 0.5f;
            direction = new Vector2Int(-1, 0);
            KeyActive();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            origin.x += 0.5f;
            direction = new Vector2Int(1, 0);
            KeyActive();
        }
    }

    void KeyActive()
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 0.8f);
        KeyAction action = gameControl.PlayerMove;

        if (hit)
        {
            if(hit.transform.gameObject.CompareTag("wall"))
            {
                action = NoAction;
            }
        }

        action(direction.y, direction.x);
    }

    // delegate --------------------------------------------------
    delegate void KeyAction(int i1, int i2);

    void NoAction(int i1, int i2)
    {
        Debug.Log("wall is here");
    }
}
