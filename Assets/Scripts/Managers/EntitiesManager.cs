using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntitiesManager : MonoBehaviour
{
    private static EntitiesManager instance;
    public static EntitiesManager Instance
    {
        get
        {
            return instance;
        }
    }

    private Dictionary<string, GameObject> entities = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void Start()
    {

    }

    public void addEntity(string id, GameObject entity)
    {
        this.entities.Add(id, entity);
    }

    public GameObject getEntity(string id)
    {
        GameObject entity;
        if (this.entities.TryGetValue(id, out entity))
        {
            return entity;
        }
        return null;
    }



    // Update is called once per frame
    void Update()
    {

    }
}
