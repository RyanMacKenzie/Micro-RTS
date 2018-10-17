using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;


public class Unit : NetworkBehaviour
{
    float health;
    float damage;
    float costperTick;
    float timeToBuild;
    Boolean isDead;
    string type;
    
	public Unit()
	{
        health = 1;
        damage = 1;
        costperTick = 1;
        timeToBuild = 1;
        isDead = false;
        type = "unit";
	}

    void Update()
    {
        if (isDead)
            die();
    }

    public void takeDamage(float damageTaken)
    {
        health -= damageTaken;
        if (health <= 0)
            isDead = true;
    }

    void die()
    {
        ////
    }
    
    //get-set
    float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }

    float Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }

    float CostPerTick
    {
        get
        {
            return costperTick;
        }
        set
        {
            costperTick = value;
        }
    }

    float TimeToBuild
    {
        get
        {
            return TimeToBuild;
        }
        set
        {
            TimeToBuild = value;
        }
    }

    Boolean IsDead
    {
        get
        {
            return isDead;
        }
        set
        {
            isDead = value;
        }
    }

    string UnitType
    {
        get
        {
            return UnitType;
        }
        set
        {
            UnitType = value;
        }
    }
}

