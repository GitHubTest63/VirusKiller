using UnityEngine;
using System.Collections;

public class GUIManager_Lobby : MonoBehaviour
{
    private static GUIManager_Lobby instance;
    public static GUIManager_Lobby Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
