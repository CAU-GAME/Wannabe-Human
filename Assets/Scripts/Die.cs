/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    Rigidbody rigid;
    CapsuleCollider capsuleCollider;
    private Animator animator;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand" && punch.Is_punch == true)
        {
            Is_die = true;
            if (Is_die == true)
            {
                animator.SetBool("Die", true);
                Is_die == false;
            }
            else
            {
                animator.SetBool("Die", false);
            }
        }

    }
    void Update()
    {
     
    }
}*/
