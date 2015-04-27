using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public float smoothing = 5.0f;
    private Vector3 offset;

    void Start()
    {
        this.offset = transform.position - target.transform.position;
    }

    void FixedUpdate()
    {
        if (this.target != null)
        {
            Vector3 targetCampPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCampPos, smoothing * Time.fixedDeltaTime);
        }
    }
}
