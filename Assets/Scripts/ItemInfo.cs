using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 ItemInfo Ŭ����
- �������� ������ �����ϱ� ���� Ŭ����
- �������� �ڵ�/�̸�/���̵� �����Ѵ�.

- �� �����ۿ� ����

������� ����
- ������ �ڵ�     : �������� ������ �����Ѵ�.
- ������ �̸�     : �������� �̹����� �ҷ����� ���� �ʿ��ϴ�.
- ������ ���̵�   : ������ �������� �����ϱ� ���� �ʿ��ϴ�. -> ���߿� Palette�� �������� �׸��� �����ȴ�.
 */

public class ItemInfo : MonoBehaviour
{
    public int code;//������ ������ �����ϴ� ������ �ڵ�
    public string itemName;//������ �̸� -> ���� ����¿� ����

    private int id = -1;//������ �������� ������ ����
                        //���߿� ���� ������ Palette�� �������� �׷��� �� �����ϱ� ���� ���

    private static Dictionary<string, int> nameToCode;
    private static int itemCode = 0;

    private void Start()
    {
        itemName = gameObject.name;

        if(nameToCode == null)//��ó���� �Ҵ��� �� �ֵ��� �Ѵ�.
        {
            Debug.Log("assigned!");
            nameToCode = new Dictionary<string, int>();
        }

        if (!nameToCode.ContainsKey(itemName))//���� �ڵ� �ο��� ���� ���� ���
        {
            nameToCode.Add(itemName, itemCode);
            code = itemCode;
            itemCode++;
        }
        else
        {
            code = nameToCode[itemName];
        }
    }

    private void OnEnable()
    {
        if (gameObject == null) return;
        itemName = gameObject.name;

        if (nameToCode == null)//��ó���� �Ҵ��� �� �ֵ��� �Ѵ�.
        {
            Debug.Log("assigned!");
            nameToCode = new Dictionary<string, int>();
        }

        if (!nameToCode.ContainsKey(itemName))//���� �ڵ� �ο��� ���� ���� ���
        {
            nameToCode.Add(itemName, itemCode);
            code = itemCode;
            itemCode++;
        }
        else
        {
            code = nameToCode[itemName];
        }
    }

    //�������� code�� name�� ����
    public void SetItemInfo(int code, string name)
    {
        this.code = code;
        this.itemName = name;
    }

    //�������� ID�� ����
    public void SetID(int id)
    {
        this.id = id;
    }

    //�������� ID ����
    public int GetID()
    {
        return id;
    }
}
