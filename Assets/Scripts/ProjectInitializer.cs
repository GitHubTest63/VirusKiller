using UnityEngine;
using System.Collections;

public class ProjectInitializer : DontDestroyOnLoad
{
    // Use this for initialization
    void Start()
    {
        GameManager.Instance.goToMainScene();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
