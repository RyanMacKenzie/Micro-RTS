using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SwarmScript : NetworkBehaviour
{
    SwarmUnit thisUnit;
    [SerializeField] GameObject controller;
    [SerializeField] BoxCollider collider;

	// Use this for initialization
	void Start ()
    {
        thisUnit = new SwarmUnit();
        GameObject.FindGameObjectWithTag("GameController");
        foreach (GameObject node in GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes)
        {
            if((this.gameObject.transform.position - node.transform.position).magnitude < 1)
            {
                controller = node.GetComponent<NodeScript>().Controller;
                
                if(node.GetComponent<SpriteRenderer>().color == Color.blue)
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                }
                else
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                }
            }
        }
        this.GetComponent<Rigidbody>().maxDepenetrationVelocity = 0.25f;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(thisUnit.IsDead)
        {
            NetworkManager.Destroy(this.gameObject);
        }
	}

    private void OnCollisionExit(Collision collision)
    {
        this.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Swarm" && collision.gameObject.GetComponent<SwarmScript>().controller != controller)
        {
            thisUnit.takeDamage(collision.gameObject.GetComponent<SwarmScript>().Unit.Damage);
            Debug.Log(thisUnit.Health);
        }
    }

    public SwarmUnit Unit
    {
        get
        {
            return thisUnit;
        }
    }
}
