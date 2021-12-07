using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 PlayerPickupItem Ŭ����
 - �÷��̾ �������� ������ �� �ֵ��� UI�� ����, 
    MainStageManager���� ������ �߰� ��ȣ�� ������. 
 */
public class PlayerPickupItem : MonoBehaviour
{
    public MainStageManager mainStageManager;
    public GameObject pressKeyGuide;
    public bool useTrigger = false;//�׽�Ʈ�� ���� Trigger�̺�Ʈ�� �̿��� �������� �����Ͽ���.

    private void Start()
    {
        mainStageManager = GameObject.Find("Main Stage Manager").GetComponent<MainStageManager>();
        pressKeyGuide = GameObject.Find("Notice").transform.GetChild(2).GetChild(4).gameObject;
    }

    //���⼭�� �ӽ÷� �浹�̺�Ʈ�� ó���ߴ�.
    //�̺�Ʈ�� ���� ItemInfo�� �޾ƿ;� �Ѵ�.
    private void OnTriggerEnter(Collider other)
    {
        if(pressKeyGuide != null && other.tag == "item")
        {
            pressKeyGuide.SetActive(true);
        }
        if (!useTrigger) return;
        //������ ���� ������ ���� �������� ȹ���� �� �ֵ��� ����
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

    //�̺�Ʈ �߻� �� �����ų �Լ�
    public void PickupItem(ItemInfo info)//������ ȹ��
    {
        //������ ���� ���°� �ƴϸ� ������ ȹ���� ���� ���ϵ��� ����
        if (mainStageManager.GetCurState() != MainStageState.Gathering) return;
        //mainStageManager�� ������ ���� ��ȣ�� ������.
        if (mainStageManager.AddItem(info))
        {
            info.gameObject.SetActive(false);//ȹ���� �������� ��Ȱ��ȭ�Ѵ�.
            pressKeyGuide.SetActive(false);
        }
    }
}
