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
    
	public Unit(float health, float damage, float costperTick, float timeToBuild, string type)
	{
        this.health = health;
        this.damage = damage;
        this.costperTick = costperTick;
        this.timeToBuild = timeToBuild;
        isDead = false;
        this.type = type;
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

