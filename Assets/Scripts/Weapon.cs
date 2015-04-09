using UnityEngine;
using System.Collections;

public abstract class Weapon
{
    public float damageValue = 1;
    public DamageType damageType = DamageType.PHYSIC;

    public virtual void applyDamages()
    {

    }
}
