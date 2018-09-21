using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeScript : MonoBehaviour
{

    //Properties
    [SerializeField] protected float resourcesPerSecond;
    [SerializeField] protected float netResourcesPerSecond;
    [SerializeField] protected float maxUnitsPerSecond;
    [SerializeField] protected float currentUnitsPerSecond;
    [SerializeField] protected float unitsInNode;
    [SerializeField] protected string controller;
    [SerializeField] protected List<GameObject> neighbors;
    [SerializeField] protected GameObject unitText;
    [SerializeField] protected int unitsBeingBuilt;
    [SerializeField] protected int resourcesBeingBuilt;
    [SerializeField] protected List<int> resourcesBeingBuiltTimeLeft;
    [SerializeField] protected int unitProductionBeingBuilt;
    [SerializeField] protected List<int> unitProductionBeingBuiltTimeLeft;
    [SerializeField] protected int maxUnitIncreaseCost;
    [SerializeField] protected int resourceProductionIncreaseCost;
    // Use this for initialization
    void Start ()
    {
        resourcesPerSecond = 1;
        maxUnitsPerSecond = 1;
        currentUnitsPerSecond = 0;
        unitsBeingBuilt = 0;
        resourcesBeingBuilt = 0;
        resourcesBeingBuiltTimeLeft = new List<int>(0);
        unitProductionBeingBuilt = 0;
        unitProductionBeingBuiltTimeLeft = new List<int>(0);
        unitsInNode = 0;
        maxUnitIncreaseCost = 10;
        resourceProductionIncreaseCost = 10;
        controller = "";
        unitText.GetComponent<TextMesh>().text = unitsInNode.ToString();
	}

    public void calculateNetResources()
    {
        netResourcesPerSecond = resourcesPerSecond - currentUnitsPerSecond - resourcesBeingBuilt;
    }

    public void UnitTick()
    {
        unitsInNode += currentUnitsPerSecond;
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

    public string Controller
    {
        get
        {
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

