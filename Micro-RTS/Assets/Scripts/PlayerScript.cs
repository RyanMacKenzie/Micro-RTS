using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

//Will be moving player controls here due to how unity networking works
//Spoilers: I really still can't wrap my head around networking... It's voodoo magic...
public class PlayerScript : NetworkBehaviour
{


    [SerializeField] int resources;


    private RaycastHit downHitInfo;
    private bool downHit;

    //handle UI
    //List of all nodes (ping GM)

    // Use this for initialization
    void Start()
    {
        //meObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().players.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        //If left mouse button is clicked, raycast to see if it selects a node. If it doesn't hit anything, deselect current node.
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked");
            downHitInfo = hitInfo;
            downHit = hit;
            if (hit)
            {
                if (hitInfo.transform.tag == "Node")
                {
                    Debug.Log("Node Clicked");
                }
            }
        }
        //When the mouse button is released, check where it was pressed down and where it was released.
        //If both locations are nodes, and the player controls the first one, half of that node's units are move to the second.
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Un-Clicked");
            if (hit && downHit)
            {
                GameObject node1 = downHitInfo.transform.gameObject;
                GameObject node2 = hitInfo.transform.gameObject;
            }
        }

        /*
         foreach(Gameobject node in allnodes){
         if(node.controller == gameObject){
         //dostuff
         resources
         }

        check if player controlled ndoe for all functions
        increaseproductionfunction{
        if(Selectednode.controller == gameobject){
        //dostuff
        ]
        }
         
         
         
         
         
         
         */
    }
}
