using UnityEngine;
using System.Collections;

public class MoveForward : MonoBehaviour
{
    public float speed = 3.0f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(this.transform.forward * this.speed * Time.deltaTime);
    }
}
