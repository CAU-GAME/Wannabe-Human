using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 PlayerPickupItem 클래스
 - 플레이어가 아이템을 수집할 수 있도록 UI를 띄우고, 
    MainStageManager에게 아이템 추가 신호를 보낸다. 
 */
public class PlayerPickupItem : MonoBehaviour
{
    public MainStageManager mainStageManager;
    public GameObject pressKeyGuide;
    public bool useTrigger = false;//테스트할 때는 Trigger이벤트를 이용해 아이템을 수집하였다.

    private void Start()
    {
        mainStageManager = GameObject.Find("Main Stage Manager").GetComponent<MainStageManager>();
        pressKeyGuide = GameObject.Find("Notice").transform.GetChild(2).GetChild(4).gameObject;
    }

    //여기서는 임시로 충돌이벤트로 처리했다.
    //이벤트를 통해 ItemInfo를 받아와야 한다.
    private void OnTriggerEnter(Collider other)
    {
        if(pressKeyGuide != null && other.tag == "item")
        {
            pressKeyGuide.SetActive(true);
        }
        if (!useTrigger) return;
        //아이템 수집 상태일 때만 아이템을 획득할 수 있도록 제한
        if (other.tag == "item")
        {
            ItemInfo info = other.gameObject.GetComponent<ItemInfo>();
            //Debug.Log("item info : " + info.itemName);
            PickupItem(other.gameObject.GetComponent<ItemInfo>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (pressKeyGuide != null && other.tag == "item")
        {
            pressKeyGuide.SetActive(false);
        }
    }

    //이벤트 발생 시 실행시킬 함수
    public void PickupItem(ItemInfo info)//아이템 획득
    {
        //아이템 수집 상태가 아니면 아이템 획득을 하지 못하도록 제한
        if (mainStageManager.GetCurState() != MainStageState.Gathering) return;
        //mainStageManager에 아이템 수집 신호를 보낸다.
        if (mainStageManager.AddItem(info))
        {
            info.gameObject.SetActive(false);//획득한 아이템은 비활성화한다.
            pressKeyGuide.SetActive(false);
        }
    }
}
