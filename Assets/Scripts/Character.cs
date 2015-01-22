using UnityEngine;
using System.Collections;

public abstract class Character : MonoBehaviour
{

    public enum DamageType
    {
        MAGIC,
        PHYSIC
    }

    public float health;
    public float maxHealth = 100;
    public float range = 10;
    public float speed = 3;

    public Weapon weapon;
    protected Animator anim;

    // Use this for initialization
    void Start()
    {
        if (this.health == 0)
            this.health = this.maxHealth;
        this.anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void moveForward(float tpf)
    {
        this.transform.Translate(this.transform.forward * this.speed * tpf, Space.World);
    }

    public void takeDamage(DamageType type, float amount)
    {

    }

    public float getDamages()
    {
        return this.weapon.damageValue;
    }

    public bool isAlive()
    {
        return this.health > 0;
    }
}
