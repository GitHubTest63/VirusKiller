using UnityEngine;
using System.Collections;

public class Weapon
{

    public enum DamageType
    {
        MAGIC,
        PHYSIC
    }

    public int damageValue = 1;
    public Weapon.DamageType damageType = DamageType.PHYSIC;

    public virtual void applyDamages()
    {

    }
}
