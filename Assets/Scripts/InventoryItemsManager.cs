using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 Inventory Items Manager Ŭ����
- ������ �����ܰ迡�� ����ڰ� ȹ���� ������(Inventory Item)���� ǥ�õ� ����(Inventory Items)�� �����Ѵ�.
- Inventory Items�� ȹ���� Inventory Item�� ǥ�õȴ�.
- Inventory Item�� ����
    - ��ư : Ŭ�� �̺�Ʈ�� ���� �� �ִ�.
        - �̹��� : � ���������� ǥ�õȴ�.
        - �ؽ�Ʈ : ������ ǥ�õȴ�.

- Inventory Items�� ����

�ֿ���
- Inventory Item�� ���� � ������ �������� ������ �ִ���, ������ �󸶳� �Ǵ��� ǥ�õȴ�.
- ������ ���� ���� Item�� Inventory Items��Ͽ��� �����ȴ�.
- Inventory Item�� ���õǴ� ���
    - ������ �ִ� ������ ������ �ϳ� ���ҵǾ�� �Ѵ�.
    - Drawn Items ��Ͽ� �ش� �������� �߰��Ǿ�� �Ѵ�.
- Drawn Items���� ��ҵ� �������� Inventory Items��Ͽ� �߰��ȴ�.
- ó�� Stage�� ���۵� ��, Inventory���� ������ ������ ������ ���� ������ �޾ƿͼ�
  Inventory Items�� ǥ���Ѵ�.
 */

public class InventoryItemsManager : MonoBehaviour
{
    private Dictionary<int, GameObject> CodeToInventoryItem;//�������� code�� Inventory Item ������Ʈ�� ��ȯ�ϴ� Dictionary

    public RectTransform content;//Inventory Item���� ��� ��, view port�� �Ѿ�� content�� ��������.
    public GameObject inventoryItem;//���ο� Inventory Item�� ������ �� ������ ������Ʈ, ���������� �غ�
    public DrawnItemsManager drawnItems;//���õ� Inventory Item�� ǥ�õ� ����
    public GameObject disable;//���� �������� ���ʰ� �ƴ� ��, Inventory Items�� ������ ���� ������Ʈ

    private InventoryManager inventory;//����ڰ� ������ ������ ������ ������ ���� ������ ������ �κ��丮 ��ü

    void Start()
    {
        inventory = InventoryManager.instance;//������ �κ��丮 ������ �����´�.
        CodeToInventoryItem = new Dictionary<int, GameObject>();//�ʿ��� ��ü ����, Code -> Inventory Item

        //ȭ���� ��ȯ�ǰ� ���� UI�� �ʱ�ȭ�ϴ� ����
        List<int> codes = inventory.GetKeys();//Inventory���� ���� ������ �ִ� �������� �ڵ带 �����´�.
        for(int i = 0; i < codes.Count; i++)//������� UI�� ǥ���Ѵ�.
        {
            DrawItem(codes[i]);
        }
    }

    public IEnumerator SetImage(int code, GameObject newObject)
    {
        yield return null;
        Image image = newObject.transform.GetChild(0).GetComponent<Image>();
        image.sprite = inventory.GetSprite(code);
        image.SetNativeSize();
        RectTransform tempRectTf = newObject.GetComponent<RectTransform>();
        Vector2 objectSize = tempRectTf.rect.size;
        Vector2 imageSize = image.rectTransform.rect.size;
        float ratio = imageSize.x / imageSize.y;
        if (imageSize.x > imageSize.y)
        {
            image.rectTransform.sizeDelta = new Vector2(objectSize.x, objectSize.x / ratio);//x�� �� �� ���
        }
        else
        {
            image.rectTransform.sizeDelta = new Vector2(objectSize.y * ratio, objectSize.y);//y�� �� �� ���
        }
        image.rectTransform.position = tempRectTf.position;
    }

    //UI(Inventory Items ���)�� �������� ǥ���ϴ� �Լ�
    //�̸� �غ��ص� ��ü(Inventory Item)�� �����Ͽ� �ڸ��� ����� ������ ������ ǥ���Ѵ�.
    public void DrawItem(int code)//�������� ���� ǥ�õ��� ���� ���¿��� ����Ǵ� �Լ�
    {
        GameObject newObject = Instantiate(inventoryItem);//���� Inventory Item�� ������
        newObject.SetActive(true);//Ȱ��ȭ
        newObject.transform.SetParent(content);//�θ� ��ü�� �ڽ����� �����. Content�� Inventory Item�� �ִ´�.
        newObject.transform.localScale = Vector3.one;//�������� �����Ѵ�. ���ڱ� �۾����� ��찡 �־ �ʱ�ȭ

        //�κ��丮���� �ʿ��� ������ �ҷ��´�.
        //�������� �̹��� ����
        StartCoroutine(SetImage(code, newObject));

        newObject.transform.GetChild(1).GetComponent<Text>().text = "x" + inventory.GetItemCount(code);//������ ������ ǥ���Ѵ�.
        
        //�������� Ŭ���Ǿ��� �� �̺�Ʈ�� ����Ѵ�.
        newObject.GetComponent<Button>().onClick.AddListener(() => OnClickItem(code));

        //�ڵ�� Inventory Item�� �����ϴ� Dictionary�� ���� �������� �߰��Ѵ�.
        CodeToInventoryItem.Add(code, newObject);//Code -> Inventory Item
    }

    //Inventory Items ��Ͽ� �ش� �������� �߰��ϴ� �Լ� -> Drawn Items ��Ͽ��� Ư�� �������� ��ҵ� �� ����ȴ�.
    //Inventory Items ��Ͽ� �̹� �׷��� �ִ� ������ ��� -> ���� �������� �� ����� ���
    //�Ǵ� �׷����� ���� ���¿��� �߰��Ǵ� ��찡 �ִ�. -> �������� ��� ����ߴ� ���
    public bool AddItem(int code)
    {
        //�κ��丮�� �������� �߰��Ѵ�.
        if (inventory.AddItem(code) == false) return false;//�������� �� �� ���� �߰����� �ʴ´�.
        
        int num = inventory.GetItemCount(code);//�κ��丮�� ������ ������ Ȯ��
        if(num == 1)//�������� ������ ���(�������� ��� ����� ���), UI�� �������� ǥ�õǾ� ���� �ʾҾ����Ƿ� �ٽ� �׷����Ѵ�.
        {
            DrawItem(code);//�ٽ� ������ �׸���
        }
        else//�׷��� ���� ���(�������� ���� �� ����� ���)�� ���� ǥ�ø� �������ָ� �ȴ�.
        {
            CodeToInventoryItem[code].transform.GetChild(1).GetComponent<Text>().text = "x" + num;//������ ������ ǥ���Ѵ�.
        }
        return true;
    }

    public bool AddItem(ItemInfo info)
    {
        //�κ��丮�� �������� �߰��Ѵ�.
        if (inventory.AddItem(info) == false) return false;//�������� �� �� ���� �߰����� �ʴ´�.

        int num = inventory.GetItemCount(info.code);//�κ��丮�� ������ ������ Ȯ��
        if (num == 1)//�������� ������ ���(�������� ��� ����� ���), UI�� �������� ǥ�õǾ� ���� �ʾҾ����Ƿ� �ٽ� �׷����Ѵ�.
        {
            DrawItem(info.code);//�ٽ� ������ �׸���
        }
        else//�׷��� ���� ���(�������� ���� �� ����� ���)�� ���� ǥ�ø� �������ָ� �ȴ�.
        {
            CodeToInventoryItem[info.code].transform.GetChild(1).GetComponent<Text>().text = "x" + num;//������ ������ ǥ���Ѵ�.
        }

        return true;
    }

    //Inventory Items ��Ͽ� �׷��� �ִ� ������ ������ �����.
    public void EraseItem(int code)
    {
        Destroy(CodeToInventoryItem[code]);//��ü�� �����.
        CodeToInventoryItem.Remove(code);//��ųʸ��� ��Ҹ� �����.
        //�� �߿� �ƹ��ų� �ϳ��� �ϸ� �Ǵ� �� ����.
    }

    //Inventory Item�� Ŭ������ �� �����ų �Լ�
    //������ ������ �ϳ� �پ���� �Ѵ�.
    public void SelectItem(int code)//������ ���� �� ������ �ϳ� �ٿ��� �Ѵ�.
    {
        bool has = inventory.PickupItem(code);//������ �ϳ��� �����´�.
        if (!has) return;//�������µ� ������ ���

        int num = inventory.GetItemCount(code);//���� ������ ������ Ȯ���Ѵ�.
        if (num == 0)//������ 0�̸� ���� �׷��� �ִ� ������Ʈ�� �����. -> �κ��丮�� �������� ����
        {
            EraseItem(code);
        }
        else
        {
            //���� 0�� �ƴ� ���� ������ �������ش�.
            CodeToInventoryItem[code].transform.GetChild(1).GetComponent<Text>().text = "x" + num;//���ڸ� �����Ѵ�.
        }
    }

    //Inventory Item�� ������ ��Ȱ��ȭ�Ѵ�.
    //��Ȱ��ȭ ǥ�� Object(Disable)�� Ȱ��ȭ �Ѵ�.
    //Disable ������Ʈ�� Inventory Items���� �ִ� Object�ν�
    //�� ������Ʈ�� Ȱ��ȭ�Ǹ� Inventory Item�� ������ ���� �ǹǷ� ��Ȱ��ȭ ȿ���� �� �� �ִ�.
    public void DisableSelect()
    {
        disable.SetActive(true);
    }

    //Inventory Item������ Ȱ��ȭ�Ѵ�.
    //Disable������Ʈ�� ��Ȱ��ȭ�Ѵ�.
    public void EnableSelect()
    {
        disable.SetActive(false);
    }

    //Inventory Item�� Ŭ������ �� ����Ǵ� �Լ� -> �̺�Ʈ ����Լ�
    //�ʿ���
    //Inventory Item�� ���õǰ� �׿� �´� ������ ����Ǿ�� �Ѵ�.
    //Drawn Items ��Ͽ� �ش� �������� �߰��ؾ� �ȴ�.
    public void OnClickItem(int code)
    {
        SelectItem(code);
        drawnItems.DrawItem(code);
    }

    public void DisableClick(int code)
    {
        CodeToInventoryItem[code].gameObject.GetComponent<Button>().interactable = false;
    }
}
