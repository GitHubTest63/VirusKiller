using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;
    public int scoreInc = 10;
    private Animator animator;
    //MyEnemyAttack attack;
    //ParticleSystem emitter;

    void Start()
    {
        if (this.currentHealth == null)
            this.currentHealth = this.maxHealth;

        this.animator = GetComponent<Animator>();

        //this.attack = GetComponent<MyEnemyAttack>();
    }

    public void takeDamage(int amount)
    {
        this.currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        if (currentHealth <= 0)
        {
            animator.SetBool("isDead", true);
            //emitter.Play();
            //attack.enabled = false;
            //GetComponent<Rigidbody>().AddExplosionForce(10.0f, transform.position, 10f, 3.0f);
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<CapsuleCollider>());
            //ScoreManager.score += scoreInc;
        }
    }

    public bool isAlive()
    {
        return currentHealth > 0;
    }
}
