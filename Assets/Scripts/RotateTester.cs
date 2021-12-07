using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*회전 테스트용 스크립트*/

public class RotateTester : MonoBehaviour
{
    public Transform target;
    Vector3 targetDir;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("forward : " + transform.forward.x + " " + transform.forward.y + " " + transform.forward.z);
        //Debug.Log("up : " + transform.up.x + " " + transform.up.y + " " + transform.up.z);
    }

    // Update is called once per frame
    void Update()
    {
        targetDir = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(targetDir/*, Vector3.right*/);
    }
}
