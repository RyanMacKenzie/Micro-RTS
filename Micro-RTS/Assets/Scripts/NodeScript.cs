﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour {

    //Properties
    [SerializeField] protected float resourcesPerSecond;
    [SerializeField] protected float maxUnitsPerSecond;
    [SerializeField] protected float currentUnitsPerSecond;
    [SerializeField] protected float unitsInNode;
    [SerializeField] protected string controller;
    [SerializeField] protected List<GameObject> neighbors;
    // Use this for initialization
    void Start () {
        resourcesPerSecond = 1;
        maxUnitsPerSecond = 1;
        currentUnitsPerSecond = 0;
        unitsInNode = 0;
        controller = "";
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
}