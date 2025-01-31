﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour
{
    public float speed = 4.0f;
    public Sprite idleSprite;
    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection;
    private Node currentNode, previousNode, targetNode;
    private int pelletsConsumed = 0;
    // Start is called before the first frame update
    void Start()
    {
        Node node = GetNodeAtPosition (transform.localPosition);
        if (node != null)
        {
            currentNode = node;
            Debug.Log (currentNode);
        }
        direction = Vector2.left;
        ChangePosition(direction);
        // Debug.Log("X:" + transform.position.x + "Y:" + transform.position.y);
    }
    // Update is called once per frame
    void Update()
    {
        // Debug.Log("SCORE: " + GameObject.Find("Game").GetComponent<Gameboard>().score);
        CheckInput();
        Move();
        UpdateOrientation();
        UpdateAnimationState();
        ConsumePellet();
    }
    void CheckInput()
    {
        if (Input.GetKeyDown (KeyCode.LeftArrow))
        {
           ChangePosition(Vector2.left);
        }
        else if (Input.GetKeyDown (KeyCode.RightArrow))
        {
            ChangePosition(Vector2.right);
        }
        else if (Input.GetKeyDown (KeyCode.UpArrow))
        {
            ChangePosition(Vector2.up);
        }
        else if (Input.GetKeyDown (KeyCode.DownArrow))
        {
            ChangePosition(Vector2.down);
        }
    }
    void ChangePosition(Vector2 d)
    {
        if(d != direction)
        nextDirection = d;

        if(currentNode != null)
        {
            Node moveToNode = CanMove(d);

            if (moveToNode != null)
            {
                direction = d;
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }
    void Move ()
    {
        if(targetNode != currentNode && targetNode != null)
        {
            if(nextDirection == direction * -1)
            {
                direction *= -1;
                Node tempNode = targetNode;
                targetNode = previousNode;
                previousNode = tempNode;
            }
            if(OverShotTarget())
            {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;
                GameObject otherPortal = GetPortal(currentNode.transform.position);
                if(otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                    currentNode = otherPortal.GetComponent<Node>();
                }

                Node moveToNode = CanMove(nextDirection);

                if(moveToNode != null)
                direction = nextDirection;

                if(moveToNode == null)
                moveToNode = CanMove(direction);

                if(moveToNode != null)
                {
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
                }
                else
                {
                    direction = Vector2.zero;
                }
                
            }
            else
            {            
                transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
            }
        }    
    }
    void MoveToNode (Vector2 d)
    {
        Node moveToNode = CanMove (d);
        if (moveToNode != null)
        {
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }
    void UpdateOrientation()
    {
        if(direction == Vector2.left)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if(direction == Vector2.right)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);

        }
        else if(direction == Vector2.up)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if(direction == Vector2.down)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }

    }

    void UpdateAnimationState()
    {
        if(direction == Vector2.zero)
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = idleSprite; 
        }
        else
        {
            GetComponent<Animator>().enabled = true;
        }
    }

    void ConsumePellet()
    {
        GameObject o = GetTileAtPosition(transform.position);
        if(o != null)
        {
            Tile tile = o.GetComponent<Tile>();
            if(tile != null)
            {
                if(!tile.didConsume && (tile.isPellet || tile.isPowerPellet))
                {
                    o.GetComponent<SpriteRenderer>().enabled = false;
                    tile.didConsume = true;
                    GameObject.Find("Game").GetComponent<Gameboard>().score += 1;
                    pelletsConsumed++;
                }
            }
        }
    }

    Node CanMove(Vector2 d)
    {
        Node moveToNode = null;
        for (int i = 0; i < currentNode.neighbors.Length; i++)
        {
            if(currentNode.validDirections [i] == d)
            {
                moveToNode = currentNode.neighbors [i];
                break;
            }
        }
        return moveToNode;
    }
    GameObject GetTileAtPosition(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);
        GameObject tile = GameObject.Find("Game").GetComponent<Gameboard>().board [(int)tileX, (int)tileY];
        if(tile != null)
            return tile;
        return null;
    }
    Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find ("Game").GetComponent<Gameboard> ().board [(int)pos.x, (int)pos.y];

        if (tile != null)
        {
            return tile.GetComponent<Node>();
        }
        return null;
    }
    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }
    float LengthFromNode(Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }
    GameObject GetPortal(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<Gameboard>().board[(int)pos.x, (int)pos.y];

        if(tile != null)
        {
            if(tile.GetComponent<Tile>() != null)
            {
                if(tile.GetComponent<Tile>().isPortal)
                {
                    GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver;
                    return  otherPortal;
                }
            }
        }
        return null;
    }
}
