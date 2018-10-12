﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NodeScript : NetworkBehaviour
{

    //Properties
    [SerializeField] [SyncVar] protected float resourcesPerSecond;
    [SerializeField] [SyncVar] protected GameObject unitText;
    [SerializeField] [SyncVar] protected float unitsInNode;
    [SerializeField] [SyncVar] protected GameObject controller;
    [SerializeField] [SyncVar] protected float maxUnitsPerSecond;
    [SerializeField] [SyncVar] protected float netResourcesPerSecond;
    [SerializeField] [SyncVar] protected float currentUnitsPerSecond;
    [SerializeField] protected List<GameObject> neighbors;
    [SerializeField] [SyncVar] protected int unitsBeingBuilt;
    [SerializeField] [SyncVar] protected int resourcesBeingBuilt;
    [SerializeField] protected List<int> resourcesBeingBuiltTimeLeft;
    [SerializeField] [SyncVar] protected int unitProductionBeingBuilt;
    [SerializeField] protected List<int> unitProductionBeingBuiltTimeLeft;
    [SerializeField] protected List<int> unitsBeingBuiltTimeLeft;
    [SerializeField] protected List<string> unitQueue;
    [SerializeField] [SyncVar] protected int maxUnitIncreaseCost;
    [SerializeField] [SyncVar] protected int resourceProductionIncreaseCost;
    [SerializeField] protected int swarmCount;
    [SerializeField] protected int siegeCount;
    [SerializeField] protected int defenseCount;

    //Use this for initialization

    void Start ()
    {
        resourcesPerSecond = 1;
        maxUnitsPerSecond = 2;
        resourcesBeingBuiltTimeLeft = new List<int>(0);
        unitProductionBeingBuiltTimeLeft = new List<int>(0);
        maxUnitIncreaseCost = 10;
        resourceProductionIncreaseCost = 10;
        unitText.GetComponent<TextMesh>().text = unitsInNode.ToString();
        if (!isServer)
        {
            return;
        }
    }

    void Update()
    {
        unitText.GetComponent<TextMesh>().text = unitsInNode.ToString();
    }

    public float calculateNetResources()
    {
        netResourcesPerSecond = resourcesPerSecond;
        if (unitQueue[0] == "swarm")
        {
            netResourcesPerSecond -= 2;
        }
        if (unitQueue[0] == "siege")
        {
            netResourcesPerSecond -= 3;
        }
        if (unitQueue[0] == "swarm")
        {
            netResourcesPerSecond -= 4;
        }
        return 0;
    }
    [Command]
    void CmdUpdateUnits()
    {
        if (unitsBeingBuiltTimeLeft[0] == 0)
        {
            if(unitQueue[0] == "swarm")
            {
                swarmCount++;
            }
            if (unitQueue[0] == "siege")
            {
                siegeCount++;
            }
            if (unitQueue[0] == "defense")
            {
                defenseCount++;
            }
            unitQueue.RemoveAt(0);
        }
        else if(unitsBeingBuiltTimeLeft[0] > 0)
        {
            unitsBeingBuiltTimeLeft[0]--;
        }
        Debug.Log("test");
    }

    [Command]
    public void CmdAddUnitToQueue(string unitType)
    {
        if (unitQueue.Count == 5)
            return;
        unitQueue.Add(unitType);
        if (unitType == "swarm")
            unitsBeingBuiltTimeLeft.Add(5);
        else if (unitType == "siege")
            unitsBeingBuiltTimeLeft.Add(5);
        else if (unitType == "defense")
            unitsBeingBuiltTimeLeft.Add(5);
        RpcAddUnitToQueue(unitType);
    }

    [ClientRpc]
    void RpcAddUnitToQueue(string unitType)
    {
        if (isServer)
            return;
        if (unitQueue.Count == 5)
            return;
        unitQueue.Add(unitType);
        if (unitType == "swarm")
            unitsBeingBuiltTimeLeft.Add(5);
        else if (unitType == "siege")
            unitsBeingBuiltTimeLeft.Add(5);
        else if (unitType == "defense")
            unitsBeingBuiltTimeLeft.Add(5);
    }
    public void unitTick()
    {
        CmdUpdateUnits();
    }

    public void ResourceBuildTick()
    {
        for (int i = 0; i < resourcesBeingBuiltTimeLeft.Count; i++)
        {
            resourcesBeingBuiltTimeLeft[i]--;
            if (resourcesBeingBuiltTimeLeft[i] == 0)
            {
                resourcesBeingBuiltTimeLeft.Remove(i);
                i--;
                resourcesBeingBuilt = resourcesBeingBuiltTimeLeft.Count;
                ResourcesPerSecond++;
                if (resourcesBeingBuilt == 0)
                    resourcesBeingBuiltTimeLeft.Clear();
            }
        }
    }

    public void addResourceToQueue()
    {
        resourcesBeingBuiltTimeLeft.Add(5 + ((int)resourcesPerSecond * 5));
        resourcesBeingBuilt = resourcesBeingBuiltTimeLeft.Count;
    }

    public void unitProductionBuildTick()
    {
        for (int i = 0; i < unitProductionBeingBuiltTimeLeft.Count; i++)
        {
            unitProductionBeingBuiltTimeLeft[i]--;
            if (unitProductionBeingBuiltTimeLeft[i] == 0)
            {
                unitProductionBeingBuiltTimeLeft.Remove(i);
                i--;
                unitProductionBeingBuilt = unitProductionBeingBuiltTimeLeft.Count;
                maxUnitsPerSecond++;
            }
        }
    }

    public void addunitProductionToQueue()
    {
        unitProductionBeingBuiltTimeLeft.Add(5 + ((int)resourcesPerSecond * 5));
        unitProductionBeingBuilt = unitProductionBeingBuiltTimeLeft.Count;
    }

    public void IncreaseUnitProductionTest()
    {

    }


    //get functions

    public float ResourcesPerSecond
    {
        get
        {
            return resourcesPerSecond;
        }
        set
        {
            resourcesPerSecond = value;
        }
    }

    public float NetResourcesPerSecond
    {
        get
        {
            return netResourcesPerSecond;
        }
        set
        {
            netResourcesPerSecond = value;
        }
    }

    public float MaxUnitsPerSecond
    {
        get
        {
            return maxUnitsPerSecond;
        }
        set
        {
            maxUnitsPerSecond = value;
        }
    }

    public float CurrentUnitsPerSecond
    {
        get
        {
            return currentUnitsPerSecond;
        }
        set
        {
            if (value == -1)
                return;

            else if (value > maxUnitsPerSecond)
                return;

            else
                currentUnitsPerSecond = value;

        }
    }

    public float UnitsInNode
    {
        get
        {
            return unitsInNode;
        }
        set
        {
            unitsInNode = value;
        }
    }

    public GameObject Controller
    {
        get
        {
            if (controller == null)
                return null;
            else
                return controller;
        }
        set
        {
            controller = value;
        }
    }

    public List<GameObject> Neighbors
    {
        get
        {
            return neighbors;
        }
        set
        {
            neighbors = value;
        }
    }

    public GameObject UnitText
    {
        get
        {
            return unitText;
        }
        set
        {
            unitText = value;
        }
    }

    public int MaxUnitIncreaseCost
    {
        get
        {
            return maxUnitIncreaseCost;
        }
        set
        {
            maxUnitIncreaseCost = value;
        }
    }
    public int ResourceProductionIncreaseCost
    {
        get
        {
            return resourceProductionIncreaseCost;
        }
        set
        {
            resourceProductionIncreaseCost = value;
        }
    }
}

