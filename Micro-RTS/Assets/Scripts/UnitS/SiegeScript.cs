﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SiegeScript : NetworkBehaviour
{
    
    SiegeUnit thisUnit;
    [SerializeField] GameObject controller;
    [SerializeField] Sprite fullHP;
    [SerializeField] Sprite damaged1;
    [SerializeField] Sprite damaged2;
    [SerializeField] Sprite damaged3;
    [SerializeField] Sprite damaged4;
    [SerializeField] Vector3 destination = Vector3.zero;
    [SerializeField] public string id;

    // Use this for initialization
    void Start()
    {
        thisUnit = new SiegeUnit();
        destination = Vector3.zero;
        GameObject.FindGameObjectWithTag("GameController");
        foreach (GameObject node in GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerNetworking>().AllNodes)
        {
            if ((this.gameObject.transform.position - node.transform.position).magnitude < 1)
            {
                controller = node.GetComponent<NodeScript>().Controller;

                if (node.GetComponent<SpriteRenderer>().color == Color.blue || node.GetComponent<SpriteRenderer>().color == Color.yellow)
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                }
                else
                {
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                }
            }
        }
        this.GetComponent<Rigidbody>().maxDepenetrationVelocity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (thisUnit.IsDead)
        {
            NetworkManager.Destroy(this.gameObject);
        }
        else
        {
            if (destination != Vector3.zero)
            {
                if ((destination - this.transform.position).magnitude <= 1)
                {
                    //this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    destination = Vector3.zero;
                }
                else
                {
                    this.transform.position += new Vector3((destination - this.transform.position).x, (destination - this.transform.position).y).normalized * Time.deltaTime;
                }
            }
            else
            {
                
            }

            if (thisUnit.Health == 5)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = fullHP;
            }
            else if (thisUnit.Health == 4)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = damaged1;
            }
            else if (thisUnit.Health == 3)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = damaged2;
            }
            else if (thisUnit.Health == 2)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = damaged3;
            }
            else if (thisUnit.Health == 1)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = damaged4;
            }
        }
    }

    public void MoveTo(Vector3 vector)
    {
        if (destination == Vector3.zero)
        {
            if ((vector - this.transform.position).magnitude > 14)
            {
                return;
            }
            destination = vector;
        }
    }

    public void setid(string newId)
    {
        id = newId;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (destination == Vector3.zero)
        {
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (this.transform.position == collision.transform.position)
        {
            this.transform.position += (new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f))).normalized * Time.deltaTime;
        }
        else
        {
            if (collision.gameObject.tag == "Swarm" && collision.gameObject.GetComponent<SwarmScript>().Destination == destination)
            {
                this.transform.position += (this.transform.position - collision.transform.position).normalized * Time.deltaTime;
            }
            else if (collision.gameObject.tag == "Siege" && collision.gameObject.GetComponent<SiegeScript>().Destination == destination)
            {
                this.transform.position += (this.transform.position - collision.transform.position).normalized * Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Swarm" && collision.gameObject.GetComponent<SwarmScript>().Controller != controller)
        {
            thisUnit.takeDamage(collision.gameObject.GetComponent<SwarmScript>().Unit.Damage);
        }
        else if (collision.gameObject.tag == "Siege" && collision.gameObject.GetComponent<SiegeScript>().Controller != controller)
        {
            thisUnit.takeDamage(collision.gameObject.GetComponent<SiegeScript>().Unit.Damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Node")
        {
            if (other.gameObject.GetComponent<NodeScript>().CurrentHP <= 0)
            {
                Debug.Log("Node Entered");

                //Make enemy units fight
                /*foreach (GameObject unit in other.gameObject.GetComponent<NodeScript>().UnitsInNode)
                {
                    if (unit.tag == "Swarm" && unit.GetComponent<SwarmScript>().Controller != controller)
                    {
                        if (!thisUnit.IsDead && !unit.GetComponent<SwarmScript>().Unit.IsDead)
                        {
                            thisUnit.takeDamage(unit.GetComponent<SwarmScript>().Unit.Damage);
                            unit.GetComponent<SwarmScript>().Unit.takeDamage(thisUnit.Damage);
                        }
                    }
                    if (unit.tag == "Siege" && unit.GetComponent<SiegeScript>().Controller != controller)
                    {
                        if (!thisUnit.IsDead && !unit.GetComponent<SiegeScript>().Unit.IsDead)
                        {
                            thisUnit.takeDamage(unit.GetComponent<SiegeScript>().Unit.Damage);
                            unit.GetComponent<SiegeScript>().Unit.takeDamage(thisUnit.Damage);
                        }
                    }
                }*/
            }
            else if (other.gameObject.GetComponent<NodeScript>().Controller != Controller)
            {
                thisUnit.takeDamage(5);
                other.gameObject.GetComponent<NodeScript>().CurrentHP -= thisUnit.Damage;
                other.gameObject.GetComponent<NodeScript>().CmdUpdateHP(other.gameObject.GetComponent<NodeScript>().CurrentHP);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!thisUnit.IsDead && other.gameObject.tag == "Node" && other.gameObject.GetComponent<NodeScript>().Controller != Controller)
        {
            //Make enemy units fight
            foreach (GameObject unit in other.gameObject.GetComponent<NodeScript>().UnitsInNode)
            {
                Debug.Log("Yeet");
                if (unit.tag == "Swarm" && unit.GetComponent<SwarmScript>().Controller != controller)
                {
                    if (!thisUnit.IsDead && !unit.GetComponent<SwarmScript>().Unit.IsDead)
                    {
                        thisUnit.takeDamage(unit.GetComponent<SwarmScript>().Unit.Damage);
                        unit.GetComponent<SwarmScript>().Unit.takeDamage(thisUnit.Damage);
                    }
                }
                if (unit.tag == "Siege" && unit.GetComponent<SiegeScript>().Controller != controller)
                {
                    if (!thisUnit.IsDead && !unit.GetComponent<SiegeScript>().Unit.IsDead)
                    {
                        thisUnit.takeDamage(unit.GetComponent<SiegeScript>().Unit.Damage);
                        unit.GetComponent<SiegeScript>().Unit.takeDamage(thisUnit.Damage);
                    }
                }
            }
            if (!thisUnit.IsDead)
            {
                other.gameObject.GetComponent<NodeScript>().Controller = controller;
            }

            //other.gameObject.GetComponent<NodeScript>().UnitsInNode.Add(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (destination == Vector3.zero)
        {
            destination = other.transform.position;
        }
    }

    public SiegeUnit Unit
    {
        get
        {
            return thisUnit;
        }
    }

    public GameObject Controller
    {
        get
        {
            return controller;
        }
    }

    public string Id
    {
        get { return id; }
    }

    public Vector3 Destination
    {
        get
        {
            return destination;
        }
    }
}
