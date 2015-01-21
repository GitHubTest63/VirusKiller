using UnityEngine;
using System.Collections;

public class TriggeredSpawner : Spawner
{
    public bool spawnOnce;
    public int nbToSpawn = 3;
    private int nbSpawned;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void spawn()
    {
        base.spawn();
        this.nbSpawned++;
    }

    void OnTriggerEnter(Collider other)
    {
        if (this.spawnOnce)
        {
            if (this.nbSpawned == 0)
            {
                this.spawn();
            }
        }
        else
        {
            if (this.nbSpawned < this.nbToSpawn)
                this.spawn();
        }
    }
}
