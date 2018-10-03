using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//listofallnodes


public class GameManagerNetworking : NetworkBehaviour
{
    //Nodes
    [SerializeField] public List<GameObject> AllNodes;
    public List<GameObject> Players;
    public GameObject localPlayer;

    // Use this for initialization
    void Start ()
    {
        InvokeRepeating("UpdateGameInfo", 0.0f, 1.0f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    //Request each player gain resources and produce units in all controlled nodes each second
    public void UpdateGameInfo()
    {
        foreach(GameObject player in Players)
        {
            player.GetComponent<PlayerScript>().TickNodes();
        }
    }

    public void IncreaseCurrentUnitsPerSecond()
    {
        localPlayer.GetComponent<PlayerScript>().IncreaseCurrentUnitsBeingBuilt();
    }

    public void DecreaseCurrentUnitsPerSecond()
    {
        localPlayer.GetComponent<PlayerScript>().DecreaseCurrentUnitsBeingBuilt();
    }

    public void IncreaseResourceProduction()
    {
         localPlayer.GetComponent<PlayerScript>().IncreaseResourceProduction();
    }

    public void IncreaseMaxUnitProduction()
    {
        localPlayer.GetComponent<PlayerScript>().IncreaseMaxUnitProdution();
    }


}
