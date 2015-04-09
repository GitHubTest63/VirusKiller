using UnityEngine;
using System.Collections;

public class TimedSpawner : Spawner
{
    public float interval = 1.0f;
    private float timer;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.timer += Time.deltaTime;
        if (this.timer >= this.interval)
        {
            this.timer = 0.0f;
            this.spawn();
        }
    }
}
