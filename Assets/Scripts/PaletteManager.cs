using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/*
 PaletteManager Ŭ����
- Drawn Items�� �������� �������� ���ÿ� ���ܳ��� Palette Item���� �����ϴ� Ŭ����
- Palette Item�� �׷����� ������ Palette��� �θ����� ����.

- Palette�� ����

�ֿ���
- Drawn Item�� ������ ��, �ش��ϴ� Palette Item�� ���ܾ� �Ѵ�.
- ���õ� Drawn Item�� ����� �Է¿� �°� �����ϰ� �����̵�/ȸ��/�������� �����ؾ� �Ѵ�.
    - Control Line���κ��� ����� �Է��� �޾Ƽ� Palette Item�� �����Ѵ�.
- ���õ� Drawn Item�� �°� ������ ����� Control Line�� ������� �Ѵ�.
- ���� ���õ� Drawn Item�� ǥ���ؾ� �Ѵ�.
    - ���� ���õ� Drawn Item�� Control Line�� ������Ʈ ���� ǥ�õȴ�.
    - ������ ���� �Ǹ� �ش� �������� Control Line�� ȭ�鿡�� �������.
    - Palette������ �ٸ� �������� ������ �� �־�� �Ѵ�.
    - Drawn items��Ͽ��� �ٸ� �������� ������ �� �־�� �Ѵ�.
    - �������� �ƴ� �ٸ� ������ Ŭ���ϸ� ���� ���õ� �������� ������ �����ȴ�.
 */

public class PaletteManager : MonoBehaviour, IPointerDownHandler
{
    public ControlLineController controlLine;//����� ���� Control Line
    public DrawnItemsManager drawnItems;//Palette���� �������� ���õǴ� ��찡 �����Ƿ�
                                        //Drawn Items�� �� ������ �˷��� �Ѵ�.

    public GameObject paletteItem;
    public RectTransform content;//palette Item�� ��� ����

    private InventoryManager inventory;//�������� �ڵ�� ������ ����� Inventory
    private Dictionary<int, GameObject> idToObject;//Key : ������ Id, Value : Palette Item
    private Dictionary<int, ControlLineController> idToControl;//Key : Id, Value : �׸� �׸� �� �̿��� ControlLine
    private Dictionary<int, GameObject> idToNetworkPrefabs;//��Ʈ��ũ ��

    private int selectedID = -1;//���� ���õ� �������� ID - Drawn Items�� ��ϵ� ID�� �����ϴ�.

    void Start()
    {
        //Inventory����
        inventory = InventoryManager.instance;

        //��ü ����
        idToObject = new Dictionary<int, GameObject>();
        idToControl = new Dictionary<int, ControlLineController>();
        idToNetworkPrefabs = new Dictionary<int, GameObject>();
    }

    //Control Line���κ��� ������ �Է¹޾� Palette Item�� �����Ѵ�.
    void Update()
    {
        if (selectedID == -1) return;//���� ���õ� �������� ���� ���� �Ѿ��.
        switch (idToControl[selectedID].GetDrawingState())//���õ� �������� Control Line ���¸� Ȯ��
        {
            case DrawingState.isTranslating://�����̵� ����
                idToObject[selectedID].transform.position = idToControl[selectedID].GetPosition();//Palette Item�� ��ġ ����
                idToNetworkPrefabs[selectedID].transform.position = idToControl[selectedID].GetWorldPosition();//��Ʈ��ũ�� Palette Item�� ��ġ�� ����
                break;

            case DrawingState.isRotating://ȸ�� ����
                idToObject[selectedID].transform.rotation = idToControl[selectedID].GetRotation();//Palette Item�� ȸ�� ����
                idToNetworkPrefabs[selectedID].transform.rotation = idToControl[selectedID].GetRotation();//��Ʈ��ũ�� Palette Item�� ȸ�� ����
                break;

            case DrawingState.isScaling://�����ϸ� ����
                idToObject[selectedID].transform.localScale = idToControl[selectedID].GetScale();//Palette Item�� ������ ����
                idToNetworkPrefabs[selectedID].transform.localScale = idToControl[selectedID].GetScale();//��Ʈ��ũ�� Palette Item�� ������ ����
                break;

            case DrawingState.None://������ ���ϰ� �ִ� ����
                break;
        }
    }

    ////Drawn Item���κ��� id�� code�� �Է¹޾Ƽ� �׿� �´� Palette Item�� �����Ѵ�.
    public void DrawObject(int id, int code)//id�� ������Ʈ�� ������ ������, code�� ������ ���� ������
    {
        //���� ���õ� ������ �ֽ�ȭ
        selectedID = id;

        //���ο� Control Line����
        ControlLineController newControl;
        newControl = Instantiate(controlLine);//Control Line ������� 
        newControl.InitializeTransform();//Control Line�� Transform �ʱ�ȭ
        newControl.Draw();//Control Line ǥ��

        GameObject newNetworkPrefab = PhotonNetwork.Instantiate(GameManager.instance.paletteItemPath + "Palette Item Network", newControl.GetWorldPosition(), Quaternion.Euler(Vector3.zero));
        newNetworkPrefab.transform.localScale = Vector3.one;//������ �ʱ�ȭ
        newNetworkPrefab.GetComponent<SpriteRenderer>().sprite = inventory.GetSprite(code);
        newNetworkPrefab.GetComponent<ItemInfo>().SetItemInfo(code, InventoryManager.instance.GetItemName(code));
        newNetworkPrefab.GetComponent<ItemInfo>().SetID(id);

        GameObject newObject = Instantiate(paletteItem);//palette ������ UI �ҷ�����, UI�� PhotonNetwork.Instantiate�� �������� �ʴ´�
        newObject.transform.SetParent(content);

        newObject.GetComponent<ItemInfo>().SetItemInfo(code, InventoryManager.instance.GetItemName(code));
        newObject.GetComponent<ItemInfo>().SetID(id);//�����ۿ� ID�ο�,
                                                     //���߿� Drawn Items��Ͽ��� �������� �����ϴ� ���� �ƴ϶�
                                                     //Palette���� �������� ���� �����Ҷ�
                                                     //�������� ������ �����ڰ� �ʿ��ϴ�.

        //���ο� �������� transform�� �ʱ�ȭ�Ѵ�.
        newObject.transform.position = newControl.GetPosition();//Control Line�� �ִ� ������ ������Ʈ�� �ű��. ���� Palette�� �߾�
        newObject.transform.rotation = Quaternion.Euler(Vector3.zero);//ȸ�� �ʱ�ȭ
        newObject.transform.localScale = Vector3.one;//������ �ʱ�ȭ


        newObject.GetComponent<Button>().onClick.AddListener(() => SelectObject(id));
        newObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 1;

        //�̹��� ����
        Image image = newObject.GetComponent<Image>();
        image.sprite = inventory.GetSprite(code);
        SpriteRenderer sr = newNetworkPrefab.GetComponent<SpriteRenderer>();
        Vector2 max = Camera.main.WorldToScreenPoint(sr.bounds.max);
        Vector2 min = Camera.main.WorldToScreenPoint(sr.bounds.min);
        image.rectTransform.sizeDelta = newControl.GetScaleScreenToUI(new Vector2(max.x - min.x, max.y - min.y));
        RectTransform tempRectTf = newObject.GetComponent<RectTransform>();
        Vector2 point = new Vector2(tempRectTf.position.x, tempRectTf.position.y) + tempRectTf.rect.size / 2;
        newControl.SetInitialSize(point);
        newObject.SetActive(true);//Ȱ��ȭ

        //Client�� �̹��� ����
        newNetworkPrefab.GetComponent<PaletteItemView>().SetImageOnClient(InventoryManager.instance.GetItemName(code));

        idToObject.Add(id, newObject);//ID�� ������ ����
        idToControl.Add(id, newControl);//ID�� Control Line����
        idToNetworkPrefabs.Add(id, newNetworkPrefab);//ID�� ��Ʈ��ũ�� ������ ����
    }

    //�׷��� �ִ� �������� �����.
    public void EraseObject(int id)
    {
        Destroy(idToObject[id]);//������ ����
        idToObject.Remove(id);//��ųʸ������� ����

        Destroy(idToControl[id].gameObject);//Control Line�� ����
        idToControl.Remove(id);//��ųʸ������� ����

        PhotonNetwork.Destroy(idToNetworkPrefabs[id]);
        idToNetworkPrefabs.Remove(id);

        if (id == selectedID)//���� ���� ���õ� �������� �����ϴ� ���
        {
            selectedID = -1;//���õ� �������� ID�� �ʱ�ȭ�Ѵ�. -> �ƹ��͵� ���� �ȵ� ���·� �ʱ�ȭ
        }
    }

    //Palette�� �׷��� �ִ� ��� �������� �����Ѵ�.
    public void EraseAllObject()
    {
        List<int> ids = new List<int>(idToObject.Keys);
        for (int i = 0; i < ids.Count; i++)
        {
            EraseObject(ids[i]);
        }
    }

    //�������� �����ϴ� �Լ�
    //������ ǥ�õ� Control Line�� ����� ���Ӱ� ���õ� �������� Control Line�� �׸���.
    public void SelectObject(int id)
    {
        InitializeSelection();
        idToControl[id].Draw();//���Ӱ� ���õ� �������� Control Lineǥ��

        drawnItems.InitializeSelection();
        drawnItems.SelectObject(id);

        selectedID = id;//���õ� ID����
    }

    //������ �����ϴ� �Լ�
    public void InitializeSelection()
    {
        //������ ���õ� �������� ���ٸ� ����� �ʿ䰡 ����.
        if (selectedID == -1) return;

        idToControl[selectedID].Hide();//������ ���õ� �������� Control Line ���߱�
        selectedID = -1;//����ID�ʱ�ȭ
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //���� �ʱ�ȭ
        InitializeSelection();
        drawnItems.InitializeSelection();
    }
}