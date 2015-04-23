using UnityEngine;
using System.Collections;

public class MiniMapFollow : MonoBehaviour
{

    public GameObject target;
    public float offset = 25.0f;
    private Vector3 newPos = new Vector3();

    void Start()
    {

    }

    void Update()
    {
        if (this.target != null)
        {
            this.newPos.Set(this.target.transform.position.x, this.offset, this.target.transform.position.z);
            this.transform.position = this.newPos;
        }
    }
}
