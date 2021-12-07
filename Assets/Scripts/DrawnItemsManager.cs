using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 DrawnItemsManager Ŭ����
- ����ڰ� ������ ������ ��, Inventory Items���� ������ �������� ǥ�õǴ� ����(Drawn Items)�� 
  ǥ�õ� ������(Dranw Item)�� �����ϴ� Ŭ�����̴�.
- Drawn Items�� Drawn Item�� ǥ�õȴ�.
- Drawn Item�� ����
    - ��ư : Ŭ�� �̺�Ʈ�� ���� �� �ִ�.
        - �̹���    : ���õ� ������ �̹����� ǥ���Ѵ�.
        - ����ǥ��  : ���� �������� ���õǾ� �ִ��� ǥ���Ѵ�.
        - ��ҹ�ư  : ���õ� �������� �׸��⸦ ����� �� �ִ�.
- ���õ� Drawn Item�� Palette�� Palette Item���� ǥ�õȴ�.

- Drawn Items�� ����

�ֿ���
- Waiting Items���� ���õ� �������� Drawn Items�� ǥ���Ѵ�.
    - �׿� ���ÿ� Palette�� �ش� �������� Palette Item�� ǥ�õȴ�.
- Drawn Item�� �ش� �������� �̹����� ��ҹ�ư�� �ִ�.
- Drawn Item�� ������ ���
    - Palette�� �׷��� Palette Item�� ��ġ, ȸ��, ũ�⸦ ������ �� �ִ�.
    - ��ġ, ȸ��, ũ�⸦ �ٲ� �� �ִ� UI(���� ��, Control Line)�� ǥ�õȴ�.
    - ���õ� �������� � ������ ǥ�ð� �Ǿ�� �Ѵ�.
- Drawn Item�� ��ҹ�ư�� Ŭ���� ���
    - Drawn Items��Ͽ��� Drawn Item�� �������, 
    - Palette������ �ش� �������� �������.
- Drawn Item�� Drawn Itemsǥ��â���� �Ʒ��������� ä������.
- Drawn Item�� ������ Drawn Itemsǥ��â�� ǥ�ð����� �������� ������ ���
    - ���õ� Drawn Item�� ���� ���ʿ� ǥ�õǾ�� �Ѵ�.
    - �� ��ũ���� ���� ������ Drawn Item�� ã�� �� �־�� �Ѵ�.
 */

public class DrawnItemsManager : MonoBehaviour
{
    private static int btnID = 0;//Drawn Item���� �����ϱ� ���� ������(ID)
    private static Dictionary<int, GameObject> idToDrawnItem;//ID -> Drawn Item, ID�� DrawnItem�� �����ϱ� ���� Dictionary����
    private Dictionary<int, int> idToCode;//ID -> �������� �ڵ�, ID�� �������� �ڵ带 �����ϱ� ���� ����, ID�� �ٸ����� �������� ����(�ڵ� Code)�� ���� �� �ִ�.
    //dic.Remove(key) ���� �����ϱ�

    public RectTransform content;//Item Object�� �θ� object, Drawn Item���� ��� ������Ʈ�� RectTransform
    private RectTransform viewport;//content = Drawn Items�� ��Ÿ���� ������ ���̴� �κ�, viewport�� ������ �Ѿ�� Drawn Item�� ������ �ʴ´�.
    
    public GameObject drawnItem;//Drawn Item�� ǥ���� Object, ������̴�. ���������� �غ��صξ���.
    public InventoryItemsManager inventoryItems;//Waiting Items���� �����ϴ� ������Ʈ
    public GameObject disable;//������ ���� ���ʰ� �ƴҶ� Drawn Item�� ������ �� ������ ������ Object
    public bool placeOnTop = true;//Inventory Items���� ���õ� �������� Drawn Items�� ���� ���ʿ� ����µ�,
                                  //������ �Ѿ�� ��� �� ���̰� �ȴ�.
                                  //�׷��� �׶� ���̰� �� �� ������ ����

    public PaletteManager palette;//Drawn Item�� 3D Object �̹����� �׷��� ����

    private InventoryManager inventory;//����� �κ��丮 ��ü
    private int selectedID = -1;//���� ���õ� object�� ID, -1�� ���þȵ� ����

    void Start()
    {
        //�������� ������ ������ Ȯ���ϱ� ���� Inventory Manager�� �����Ѵ�.
        inventory = InventoryManager.instance;
        //�ʿ��� ��ü ����
        idToDrawnItem = new Dictionary<int, GameObject>();
        idToCode = new Dictionary<int, int>();
        //viewport�� RectTransform�� �޾ƿ´�.
        viewport = content.parent.GetComponent<RectTransform>();
    }

    //������ �̹��� ũ�� ����
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
        //������ �̹������� ���� ���� ���� �� ��ٶ� ���� Drawn Item�� ũ�⿡ �����.
        //���̰� ª�� ���� Ratio�� ���ؼ� ������ �����.
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

    //Inventory Items���� Item�� ���õǸ� ����� �Լ�
    //Drawn Items��Ͽ� Drawn Item�� �߰��Ѵ�.
    //�׸��� �׿� �ش��ϴ� ������ �ʱ�ȭ
    public void DrawItem(int code)
    {
        GameObject newObject = Instantiate(drawnItem);//���ο� Drawn Item����
        newObject.SetActive(true);//���̵��� Ȱ��ȭ
        newObject.transform.SetParent(content);//content�� Drawn Item���
        newObject.transform.localScale = Vector3.one;//ũ�Ⱑ ����Ǵ� ������ �����Ƿ� ������ �� ����

        //���õ� �������� �̹����� �����Ѵ�.
        //newObject.transform.GetChild(0).GetComponent<Image>().sprite = inventory.GetSprite(code);
        StartCoroutine(SetImage(code, newObject));

        //��ư�� ���̵� �ο��Ѵ�.
        int id = btnID;
        
        //Drawn Item�� Ŭ���Ǿ����� �̺�Ʈ���� ����Ѵ�.
        newObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => OnClickCancel(id));//��� ��ư�� ������ �� �̺�Ʈ
        newObject.GetComponent<Button>().onClick.AddListener(() => OnClickItem(id));//Drawn Item�� ������ �� �̺�Ʈ
        //Id�� �ش� �������� ������ �����Ѵ�.
        idToDrawnItem.Add(id, newObject);
        idToCode.Add(id, code);
        //�ش� �������� �����Ѵ�.
        InitializeSelection();
        SelectObject(id);
        btnID++;//���Ӱ� �߰��� ������ ID ����

        //Palette�� ���� ���õ� �������� Palette Item�� �׸���.
        palette.InitializeSelection();
        palette.DrawObject(id, code);//Palette Item�׸���
        
        PlaceContentTop();//���� ���õ� �������� ������ �ʴ� ��� ���� ���ʿ� ���̵��� ����
    }

    //��� ��ư�� Ŭ������ �� ����� �Լ�
    //���� Drawn Items��Ͽ��� �ش� �������� ������ �����Ѵ�.
    public void EraseItem(int id)
    {
        Destroy(idToDrawnItem[id]);//��ü�� �����Ѵ�.
        idToDrawnItem.Remove(id);//��ųʸ��� ��Ҹ� �����Ѵ�.

        if (id == selectedID)//���� ���� ���û����� �������� ��ҵǴ� ���
        {
            selectedID = -1;//�ƹ��͵� ���õ��� ���� ���·� �ʱ�ȭ
        }
    }

    //Drawn Item�� ���õǾ��ٴ� ���� ǥ���� �Լ�
    //����ǥ�ø� Ȱ��ȭ�Ѵ�.
    public void SelectObject(int id)
    {
        idToDrawnItem[id].transform.GetChild(1).gameObject.SetActive(true);//���� ǥ�� �׸���
        selectedID = id;//���� ���õ� ID����
    }

    //�ٸ� �������� ���õǴ� ���
    //�������� �ƴ� �ٸ� ������ ���õǴ� ���
    //����ǥ�ø� ��Ȱ��ȭ�Ѵ�.
    public void InitializeSelection()
    {
        if(selectedID == -1)//���õǾ� �ִ� ��찡 �ƴϸ� ����
        {
            return;
        }
        //���� ���õǾ� �ִ� �������� ����ǥ�� ��Ȱ��ȭ
        idToDrawnItem[selectedID].transform.GetChild(1).gameObject.SetActive(false);
        selectedID = -1;//���õǾ� ���� ���� ���·� �ʱ�ȭ
    }

    //��ҹ�ư�� ������ �� �̺�Ʈ �Լ�
    //�ʿ���
    //Drawn Items��Ͽ��� �ش� �������� �������Ѵ�.
    //Control Line�� ������ �Ѵ�.
    //Inventory Items�� �׷����� �Ѵ�.
    public void OnClickCancel(int id)
    {
        EraseItem(id);//Drawn Items��Ͽ��� �����
        palette.EraseObject(id);//Palette�� �׷��� 3D Object�����
        inventoryItems.AddItem(idToCode[id]);//Waiting item�� �ٽ� �������� �߰��Ѵ�.
    }

    //Drawn Item�� ���õǾ��� �� �̺�Ʈ �Լ�
    //�ʿ���
    //Drawn Item�� ����ǥ�ð� Ȱ��ȭ�Ǿ�� �Ѵ�.
    //Palette���� Palette Item�� ������ �� �־�� �Ѵ�. palette item�����ϱ�
    public void OnClickItem(int id)
    {
        InitializeSelection();
        SelectObject(id);//����ǥ�� Ȱ��ȭ
        palette.InitializeSelection();
        palette.SelectObject(id);//Palette���� ������ ����
    }

    //Inventory Items���� ���õ� Drawn Items�� viewport�� ������ ���� ���
    //���õ� Drawn Item�� viewport���� ���ʿ� ��ġ���Ѿ� �Ѵ�.
    public void PlaceContentTop()
    {
        if (!placeOnTop) return;

        //content�� viewport�� ���̸� ���Ѵ���
        float contentHeight = content.rect.height + drawnItem.GetComponent<RectTransform>().rect.height;
        //content�� ���̸� ���� ��, Drawn Item�� ���̸� �� �� �� ���ϴ� ������
        //PlaceContentTop�Լ��� ����� �� �������� �� ������ ���������� �ݿ��� �ȵǱ� �����̴�.
        //���� ���̰� �ԷµǱ� ������ ���̸� ���ߴ�. �ƴϸ� �ڷ�ƾ�� �̿��� ���������� ���� �� �����ص� �� �� ����.
        float viewportHeight = viewport.rect.height;
        //���̸� ����Ѵ�. content�� ���̰� �� ū ��찡 Drawn Item�� �������� ����.
        float diff = contentHeight - viewportHeight;
        if (diff > 0)
        {
            content.anchoredPosition -= new Vector2(0f, diff * 3f);
            //���̸�ŭ�� �̵���Ű�� Scroll Rect�� ����ȿ�� ������ �� �Ⱥ��̰� �ȴ�.
            //�׷��� ���̺��� �� ���� �̵���Ű�� �ִϸ��̼� ȿ���� �ְ� �����۵� �ߺ��̰� �ȴ�.
        }
    }

    //Drawn Item�� ������ ��Ȱ��ȭ�Ѵ�.
    //��Ȱ��ȭ ǥ�� Object(Disable)�� Ȱ��ȭ �Ѵ�.
    //Disable ������Ʈ�� Drawn Items���� �ִ� Object�ν�
    //�� ������Ʈ�� Ȱ��ȭ�Ǹ� Drawn Item�� ������ ���� �ǹǷ� ��Ȱ��ȭ ȿ���� �� �� �ִ�.
    public void DisableSelect()
    {
        disable.SetActive(true);
    }

    //Drawn Item������ Ȱ��ȭ�Ѵ�.
    //Disable������Ʈ�� ��Ȱ��ȭ�Ѵ�.
    public void EnableSelect()
    {
        disable.SetActive(false);
    }

    //���� Drawn Items��Ͽ� �ִ� �����۵��� ��� �����Ѵ�.
    public void EraseAllObject()
    {
        List<int> ids = new List<int>(idToDrawnItem.Keys);
        for(int i = 0; i < ids.Count; i++)
        {
            Destroy(idToDrawnItem[ids[i]]);//��ü ����
            idToDrawnItem.Remove(ids[i]);//Dictionary���� ����
            idToCode.Remove(ids[i]);
        }
    }
}
