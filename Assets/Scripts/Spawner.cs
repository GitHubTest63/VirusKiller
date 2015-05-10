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

    // Update is called once per frame
    void Update()
    {
    }

    protected virtual void spawn()
    {
        GameObject spawned = PhotonNetwork.Instantiate("Prefabs/" + this.toSpawn.name, Vector3.up * 5, Quaternion.identity, 0);
        spawned.transform.parent = this.transform;
        spawned.transform.position = this.transform.position;
        spawned.transform.rotation = this.transform.rotation;
    }
}
