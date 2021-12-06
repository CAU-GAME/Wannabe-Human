using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 InventoryManager Ŭ����
- �� Ŭ������ stage1(������ ���� �ܰ�)���� �������� ������ ��, � �������� �󸶳� ��Ҵ��� �����Ѵ�.

- Stage1 ���� Inventory Manager��ü�� ����

�ֿ���
- � ������(�ڵ�� ����)�� �󸶳� ��Ҵ��� �� �� �־�� �Ѵ�.
- �������� �̸��� �� �� �־�� �Ѵ�.
    - �ش� �������� �̹����� 3D ������Ʈ�� Stage2���� �ʿ��ϴ�.
    - �̸��� �̹����� 3D ������Ʈ�� �ҷ����� ���� �ʿ��ϴ�.
 */

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    //InventoryManager�� ���������� �����ϱ� ���� �̱��� ���� ����

    private Dictionary<int, int> itemCount;//�ڵ�(������ ����) -> ���� ������ �ִ� ������ ����
    private Dictionary<int, string> Lookup;//�ڵ� -> ������ �̸�

    private int maxItemKinds;//ȹ�� ������ �������� �ִ� ������
    private int howManyKinds = 0;//�� ������ �������� �����ߴ��� Ȯ���ϱ� ���� ����

    void Start()
    {
        if(instance == null)
        {
            instance = this;//�ڱ� �ڽ� ����

            //��ü �ʱ�ȭ
            itemCount = new Dictionary<int, int>();
            Lookup = new Dictionary<int, string>();

            maxItemKinds = GameManager.instance.maxItemNum;//ȹ�� ������ �������� �ִ� ������ ����

            DontDestroyOnLoad(gameObject);//�ٸ� �������� �� �� �ֵ���
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    //�κ��丮�� �������� �߰��Ѵ�.
    //ItemInfo�� �����ۿ� ���� ������ ������ �ִ� Ŭ�����̴�.
    //�������� �̸��� �������� �ڵ带 �����ϰ� �ִ�.
    //Stage1���� ����ϴ� �Լ�
    public bool AddItem(ItemInfo item)
    {
        //�ش� �������� �κ��丮�� �ִ� ���, ���� ������ �������� ���
        if (itemCount.ContainsKey(item.code))
        {
            //������ �ϳ� �����Ѵ�.
            itemCount[item.code] += 1;
        }
        else//ó�� ������ �������� ���
        {
            if (howManyKinds == maxItemKinds) return false;//�̹� �ִ� ��������ŭ �������� ���� ���� �߰��� �� ����.
            itemCount.Add(item.code, 1);//�κ��丮�� ���Ӱ� �������� �߰��Ѵ�.
            Lookup.Add(item.code, item.itemName);//������ �̸��� �� �� �ֵ��� �̸��� �߰�
            howManyKinds += 1;
        }
        return true;
    }

    //�κ��丮�� �������� ������ٰ� �ٽ� ä������ ��� -> Drawn Items ��Ͽ��� �߰� ��ȣ�� ���� ���
    //�̶��� code�� �־ �ȴ�.
    //�� �Լ��� �κ��丮�� �߰��ϴ� ���� �����ϴ�. ������ �ڵ常 �̿��� �߰��Ѵ�.
    //Stage2���� ����ϱ� ���� �Լ�
    public bool AddItem(int code)
    {
        if (itemCount.ContainsKey(code))
        {
            itemCount[code] += 1;
        }
        else//������ٰ� ä������ ���
        {
            if (howManyKinds == maxItemKinds) return false;//�̹� �ִ� ��������ŭ �������� ���� ���� �߰��� �� ����.
            itemCount.Add(code, 1);
            howManyKinds += 1;
        }
        return true;
    }

    //�κ��丮���� �������� ������.
    //������ �ϳ� ���ҽ�Ų��.
    //�������� �� ������ true �ƴϸ� false
    public bool PickupItem(int code)
    {
        if (itemCount.ContainsKey(code))//�������� ���� Inventory�� �����ִ� ���
        {
            //������ 0�� �ƴ� ��쿡�� ���� ����
            if (itemCount[code] > 0) itemCount[code] -= 1;
            //������ ���� �Ŀ� ������ 0�� �Ǹ� inventory���� �����Ѵ�.
            if (itemCount[code] == 0)
            {
                itemCount.Remove(code);
                howManyKinds -= 1;
            }
            return true;
        }
        return false;
    }

    //���� �������� �� ���� �ִ��� Ȯ���ϴ� �Լ�
    public int GetItemCount(int code)
    {
        //�������� ���� �κ��丮�� �����ִ� ��� ������ ��ȯ
        if (itemCount.ContainsKey(code)) return itemCount[code];
        else return 0;//���� ���� 0�� ��ȯ
    }

    //�������� �̸��� �˾Ƴ��� ���� �Լ�
    public string GetItemName(int code)
    {
        //�������� �ִ� ��� Lookup ���̺��� �̸��� ��ȯ�Ѵ�.
        if (Lookup.ContainsKey(code)) return Lookup[code];
        return "";//���� ���� ""��ȯ
    }

    //�������� �̹����� �ҷ��´�.
    public Sprite GetSprite(int code)
    {
        Sprite sprite = Resources.Load<Sprite>(GameManager.instance.itemImagePath + GetItemName(code));
        return sprite;
    }

    //���� ������ �ִ� �������� �ڵ带 ����Ʈ ���·� ��ȯ�Ѵ�.
    public List<int> GetKeys()
    {
        return new List<int>(itemCount.Keys);
    }

    //���� � �������� ����Ǿ� �ִ��� ��� - ������
    public void ShowItems()
    {
        string result = "\n";
        foreach (KeyValuePair<int, int> item in itemCount)
        {
            result += "key : " + item.Key + " value : " + item.Value + "\n";
        }
        Debug.Log(result);
    }

    //������ �ִ� �������� �ʱ�ȭ�Ѵ�.
    public void ClearItems()
    {
        itemCount.Clear();
        Lookup.Clear();
    }

    public int GetHowManyKinds()
    {
        return howManyKinds;
    }
}