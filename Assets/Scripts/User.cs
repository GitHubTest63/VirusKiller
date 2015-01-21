using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class User : Character
{
    public int resources = 50;
    public int score = 0;
    public List<Weapon> availableWeapons;


    public void respawn()
    {

    }

    public void reload()
    {

    }

    public void changeWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public void spendResources(int amount)
    {
        this.resources -= amount;
        if (this.resources < 0)
            this.resources = 0;
    }


}
