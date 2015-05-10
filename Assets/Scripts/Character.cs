using UnityEngine;
using System.Collections;

public abstract class Character : Photon.MonoBehaviour
{

    public int health;
    public int maxHealth = 100;
    public float range = 10.0f;
    public float speed = 3.0f;
    public float rotationSpeed = 180.0f;

    public Weapon weapon;
    protected Animator anim;
    public GameObject damageTextPrefab;

    protected virtual void Start()
    {
        if (this.health == 0)
            this.health = this.maxHealth;
        this.anim = GetComponent<Animator>();

        //temp
        this.weapon = new Weapon();
    }

    void Update()
    {

    }

    protected void moveForward(float tpf)
    {
        this.transform.Translate(this.transform.forward * this.speed * tpf, Space.World);
    }

    public void takeDamage(Weapon.DamageType type, int amount)
    {
        this.modifyHealth(amount);
        if (!isAlive())
        {
            this.death();
        }
        else
        {
            if (this.damageTextPrefab != null)
            {
                Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
                pos.x = Mathf.Clamp(pos.x, 0.05f, 0.95f); // clamp position to screen to ensure
                pos.y = Mathf.Clamp(pos.y, 0.05f, 0.9f); // the string will be visible
                pos.z = 0.0f;
                GameObject damageText = Instantiate(this.damageTextPrefab, pos, Quaternion.identity) as GameObject;
                damageText.GetComponent<DamageText>().setValue(amount);
            }
        }
    }

    [RPC]
    protected virtual void modifyHealth(int amount)
    {
        this.health = Mathf.Clamp(this.health - amount, 0, this.maxHealth);

        if (this.photonView.isMine)
            this.photonView.RPC("modifyHealth", PhotonTargets.Others, amount);
    }

    public float getDamages()
    {
        return this.weapon.damageValue;
    }

    public bool isAlive()
    {
        return this.health > 0;
    }

    protected virtual void death()
    {
        Debug.Log("Death of " + this.tag);
    }
}
