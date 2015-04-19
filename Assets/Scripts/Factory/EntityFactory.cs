using UnityEngine;
using System.Collections;

public class EntityFactory : MonoBehaviour
{
    private static EntityFactory instance;
    public static EntityFactory Instance
    {
        get
        {
            return instance;
        }
    }

    public GameObject playerModel;
    public GameObject enemyModel;

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

    public GameObject createPlayer(string name)
    {
        GameObject player = Instantiate(this.playerModel) as GameObject;
        player.name = name;
        player.transform.parent = this.transform;
        return player;
    }
}
