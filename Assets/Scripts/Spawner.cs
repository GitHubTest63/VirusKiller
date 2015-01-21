using UnityEngine;
using System.Collections;

public abstract class Spawner : MonoBehaviour
{

    public GameObject toSpawn;
    // Use this for initialization
    void Start()
    {
        if (this.toSpawn == null)
            this.enabled = false;
        Debug.Log("Nothing to spawn !!!");
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected virtual void spawn()
    {
        GameObject spawned = Instantiate(this.toSpawn, this.transform.position, this.transform.rotation) as GameObject;
        spawned.transform.parent = this.transform;
    }
}
