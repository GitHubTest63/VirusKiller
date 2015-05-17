using UnityEngine;
using System.Collections;

public abstract class Spawner : MonoBehaviour
{

    public GameObject toSpawn;
    // Use this for initialization
    void Start()
    {
        if (this.toSpawn == null)
        {
            this.enabled = false;
            Debug.Log("Nothing to spawn !!!");
        }
    }

    void Update()
    {
    }

    protected virtual void spawn()
    {
        GameObject spawned = Instantiate(this.toSpawn, Vector3.up * 5, Quaternion.identity) as GameObject;
        spawned.transform.parent = this.transform;
        spawned.transform.position = this.transform.position;
        spawned.transform.rotation = this.transform.rotation;
    }
}
