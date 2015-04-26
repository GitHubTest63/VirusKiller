using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class User : Character
{
    public int resources = 50;
    public int score = 0;
    public List<Weapon> availableWeapons;

    protected override void Start()
    {
        base.Start();
        this.updateHealth();
    }

    public void respawn()
    {

    }

    public void reload()
    {

    }

    protected override void modifyHealth(int amount)
    {
        base.modifyHealth(amount);
        this.updateHealth();
    }

    private void updateHealth()
    {
        GUIManager_Game.Instance.setHealth(this.health, this.maxHealth);
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
