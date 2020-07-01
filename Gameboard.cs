using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour
{
    private static int boardWidth = 28;
    private static int boardHeight = 36;

    public int totalPellets = 0;
    public int score = 0;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];
    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject o in objects)
        {
            Vector2 pos = o.transform.position;
            if (o.tag == "Node" || o.tag == "Pellet")
            {
                if(o.GetComponent<Tile>() != null)
                {
                    if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isPowerPellet)
                    {
                        totalPellets++;
                    }
                }
                board [(int)pos.x, (int)pos.y] = o;
            } 
            // else
            // {
            //     Debug.Log ("Found PacMan at" + pos);
            // }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
