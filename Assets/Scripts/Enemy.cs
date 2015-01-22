using UnityEngine;
using System.Collections;

public class Enemy : Character
{

    public enum Type
    {
        DISTANCE,
        CAC
    };

    public Type enemyType = Type.CAC;
    public Transform target;
    private bool isWalking = false;


    // Update is called once per frame
    void Update()
    {
        if (hasTarget())
        {
            //facing target
            Vector3 targetPostition = new Vector3(target.position.x, this.transform.position.y, target.position.z);
            this.transform.LookAt(targetPostition);

            //check range and move it target is too far
            if (Vector3.Distance(transform.position, this.target.transform.position) > this.range)
            {
                follow();
            }
            else
            {
                if (this.isWalking)
                {
                    isWalking = false;
                    this.anim.SetBool("isWalking", false);
                }
                attack();
                this.anim.SetBool("isAttacking", true);
            }
        }
    }

    private bool hasTarget()
    {
        return this.target != null;
    }

    private void follow()
    {
        if (hasTarget())
        {
            this.moveForward(Time.deltaTime);
            if (!isWalking)
            {
                this.isWalking = true;
                this.anim.SetBool("isWalking", true);
            }
        }
    }

    private void attack()
    {
        if (hasTarget())
        {
            Debug.Log("Attack");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            this.target = other.gameObject.transform;
        }
    }
}
