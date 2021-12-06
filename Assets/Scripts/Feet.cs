using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour
{
    
    Rigidbody rigid;
    BoxCollider boxCollider;
    
    void Start()
    {
         rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    
    void Update()
    {
        
        
    }
   
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "CanCrush")
        {
            rigid.isKinematic = true;
            
        }
    }
}
