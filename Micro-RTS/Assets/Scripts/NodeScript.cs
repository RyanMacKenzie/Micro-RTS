using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NodeScript : NetworkBehaviour
{

    //Properties
    [SerializeField] [SyncVar] protected float resourcesPerSecond;
    [SerializeField] [SyncVar] protected GameObject unitText;
    [SerializeField] protected float unitsInNode;
    [SerializeField] [SyncVar] protected GameObject controller;
    [SerializeField] [SyncVar] protected float maxUnitsPerSecond;
    [SerializeField] [SyncVar] protected float netResourcesPerSecond;
    [SerializeField] protected float currentUnitsPerSecond;
    [SerializeField] protected List<GameObject> neighbors;
    [SerializeField] [SyncVar] protected int unitsBeingBuilt;
    [SerializeField] [SyncVar] protected int resourcesBeingBuilt;
    [SerializeField] protected List<int> resourcesBeingBuiltTimeLeft;
    [SerializeField] [SyncVar] protected int unitProductionBeingBuilt;
    [SerializeField] protected List<int> unitProductionBeingBuiltTimeLeft;
    [SerializeField] [SyncVar] protected int maxUnitIncreaseCost;
    [SerializeField] [SyncVar] protected int resourceProductionIncreaseCost;

    //Use this for initialization

   void Start ()
    {
        resourcesPerSecond = 1;
        maxUnitsPerSecond = 5;
        currentUnitsPerSecond = 0;
        unitsBeingBuilt = 0;
        resourcesBeingBuilt = 0;
        resourcesBeingBuiltTimeLeft = new List<int>(0);
        unitProductionBeingBuilt = 0;
        unitProductionBeingBuiltTimeLeft = new List<int>(0);
        unitsInNode = 0;
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
        netResourcesPerSecond = resourcesPerSecond - currentUnitsPerSecond - resourcesBeingBuilt;
        //Debug.Log(netResourcesPerSecond);
        if (resourcesBeingBuiltTimeLeft.Count == 0)
        {
            return netResourcesPerSecond;
        }
        else
        {
            return 0;
        }
    }

    public void unitTick()
    {
        if(resourcesBeingBuiltTimeLeft.Count == 0)
        {
            unitsInNode += currentUnitsPerSecond;
        }
        unitText.GetComponent<TextMesh>().text = "" + unitsInNode;
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

