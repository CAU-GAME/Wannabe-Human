using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
  
    Rigidbody rigid;
    BoxCollider boxCollider;
    private bool check = false;

 
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
       
    }
    void Update()
    {

    }
    void IsPress()
    {
        if (player_move.drop == true) check = true;
        if (player_move.drop == false) check = false;
    }
    
    void OnTriggerStay(Collider other)
    {
        IsPress();
        if(other.tag=="Feet"&&check==true)
        {
            Destroy(gameObject);
            for(int i=0;i<4;i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                transform.GetChild(i).gameObject.GetComponent<ItemInfo>().enabled = true;
                transform.GetChild(i).transform.parent = null;
            }
        }
    }
}
