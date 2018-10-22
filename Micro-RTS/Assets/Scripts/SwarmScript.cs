using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SwarmScript : NetworkBehaviour
{
    SwarmUnit thisUnit;
    [SerializeField] float startX;
    [SerializeField] float startY;

	// Use this for initialization
	void Start ()
    {
        thisUnit = new SwarmUnit();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
