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
    [SerializeField] protected List<int> unitsBeingBuiltTimeLeft;
    [SerializeField] protected int resourcesBeingBuilt;
    [SerializeField] protected List<int> resourcesBeingBuiltTimeLeft;
    // Use this for initialization
    void Start ()
    {
        resourcesPerSecond = 1;
        maxUnitsPerSecond = 1;
        currentUnitsPerSecond = 0;
        unitsBeingBuilt = 0;
        unitsBeingBuiltTimeLeft = new List<int>();
        unitsInNode = 0;
        controller = "";
        unitText.GetComponent<TextMesh>().text = unitsInNode.ToString();
	}

    public void calculateNetResources()
    {
        netResourcesPerSecond = resourcesPerSecond - unitsBeingBuilt - resourcesBeingBuilt;
    }

    public void UnitTick()
    {
        for(int i = 0; i < unitsBeingBuiltTimeLeft.Count; i++)
        {
            if (i == maxUnitsPerSecond)
                return;

            unitsBeingBuiltTimeLeft[i]--;
            if(unitsBeingBuiltTimeLeft[i] == 0)
            {
                unitsBeingBuiltTimeLeft.Remove(i);
                i--;
                unitsBeingBuilt = unitsBeingBuiltTimeLeft.Count;
                unitsInNode++;
            }
        }
    }

    public void addUnitToQueue()
    {
            unitsBeingBuiltTimeLeft.Add(5);
            unitsBeingBuilt = unitsBeingBuiltTimeLeft.Count;
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
            }
        }
    }

    public void addResourceToQueue()
    {
            resourcesBeingBuiltTimeLeft.Add(5 + ((int)resourcesPerSecond * 5));
            resourcesBeingBuilt = resourcesBeingBuiltTimeLeft.Count;
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
}

