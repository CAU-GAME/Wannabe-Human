using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;

    public float dist = 7f;
    public float height = 5f;
    // Start is called before the first frame update
    void Start()
    {
        //target = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position - (1 * Vector3.forward * dist) + (Vector3.up * height);
        transform.LookAt(target);
        Debug.Log(transform.position);
    }
}
