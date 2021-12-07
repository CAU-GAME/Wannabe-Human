using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/*
 PaletteManager 클래스
- Drawn Items에 아이템이 생겼을때 동시에 생겨나는 Palette Item들을 관리하는 클래스
- Palette Item이 그려지는 공간을 Palette라고 부르도록 하자.

- Palette에 부착

주요기능
- Drawn Item이 생겼을 때, 해당하는 Palette Item도 생겨야 한다.
- 선택된 Drawn Item에 사용자 입력에 맞게 적절하게 평행이동/회전/스케일을 적용해야 한다.
    - Control Line으로부터 사용자 입력을 받아서 Palette Item에 적용한다.
- 선택된 Drawn Item에 맞게 적절한 모양의 Control Line을 보여줘야 한다.
- 현재 선택된 Drawn Item을 표시해야 한다.
    - 현재 선택된 Drawn Item의 Control Line이 오브젝트 위에 표시된다.
    - 선택이 해제 되면 해당 아이템의 Control Line은 화면에서 사라진다.
    - Palette내에서 다른 아이템을 선택할 수 있어야 한다.
    - Drawn items목록에서 다른 아이템을 선택할 수 있어야 한다.
    - 아이템이 아닌 다른 공간을 클릭하면 현재 선택된 아이템은 선택이 해제된다.
 */

public class PaletteManager : MonoBehaviour, IPointerDownHandler
{
    public ControlLineController controlLine;//복사용 원본 Control Line
    public DrawnItemsManager drawnItems;//Palette에서 아이템이 선택되는 경우가 있으므로
                                        //Drawn Items에 이 정보를 알려야 한다.

    public GameObject paletteItem;
    public RectTransform content;//palette Item이 담길 공간

    private InventoryManager inventory;//아이템의 코드와 개수가 저장된 Inventory
    private Dictionary<int, GameObject> idToObject;//Key : 아이템 Id, Value : Palette Item
    private Dictionary<int, ControlLineController> idToControl;//Key : Id, Value : 그림 그릴 때 이용할 ControlLine
    private Dictionary<int, GameObject> idToNetworkPrefabs;//네트워크 용

    private int selectedID = -1;//현재 선택된 아이템의 ID - Drawn Items에 등록된 ID와 동일하다.

    void Start()
    {
        //Inventory참조
        inventory = InventoryManager.instance;

        //객체 생성
        idToObject = new Dictionary<int, GameObject>();
        idToControl = new Dictionary<int, ControlLineController>();
        idToNetworkPrefabs = new Dictionary<int, GameObject>();
    }

    //Control Line으로부터 정보를 입력받아 Palette Item에 적용한다.
    void Update()
    {
        if (selectedID == -1) return;//현재 선택된 아이템이 없는 경우는 넘어간다.
        switch (idToControl[selectedID].GetDrawingState())//선택된 아이템의 Control Line 상태를 확인
        {
            case DrawingState.isTranslating://평행이동 상태
                idToObject[selectedID].transform.position = idToControl[selectedID].GetPosition();//Palette Item의 위치 설정
                idToNetworkPrefabs[selectedID].transform.position = idToControl[selectedID].GetWorldPosition();//네트워크용 Palette Item의 위치도 설정
                break;

            case DrawingState.isRotating://회전 상태
                idToObject[selectedID].transform.rotation = idToControl[selectedID].GetRotation();//Palette Item의 회전 설정
                idToNetworkPrefabs[selectedID].transform.rotation = idToControl[selectedID].GetRotation();//네트워크용 Palette Item의 회전 설정
                break;

            case DrawingState.isScaling://스케일링 상태
                idToObject[selectedID].transform.localScale = idToControl[selectedID].GetScale();//Palette Item의 스케일 설정
                idToNetworkPrefabs[selectedID].transform.localScale = idToControl[selectedID].GetScale();//네트워크용 Palette Item의 스케일 설정
                break;

            case DrawingState.None://조작을 안하고 있는 상태
                break;
        }
    }

    ////Drawn Item으로부터 id와 code를 입력받아서 그에 맞는 Palette Item을 생성한다.
    public void DrawObject(int id, int code)//id는 오브젝트의 고유한 구분자, code는 아이템 종류 구분자
    {
        //현재 선택된 아이템 최신화
        selectedID = id;

        //새로운 Control Line생성
        ControlLineController newControl;
        newControl = Instantiate(controlLine);//Control Line 복사생성 
        newControl.InitializeTransform();//Control Line의 Transform 초기화
        newControl.Draw();//Control Line 표시

        GameObject newNetworkPrefab = PhotonNetwork.Instantiate(GameManager.instance.paletteItemPath + "Palette Item Network", newControl.GetWorldPosition(), Quaternion.Euler(Vector3.zero));
        newNetworkPrefab.transform.localScale = Vector3.one;//스케일 초기화
        newNetworkPrefab.GetComponent<SpriteRenderer>().sprite = inventory.GetSprite(code);
        newNetworkPrefab.GetComponent<ItemInfo>().SetItemInfo(code, InventoryManager.instance.GetItemName(code));
        newNetworkPrefab.GetComponent<ItemInfo>().SetID(id);

        GameObject newObject = Instantiate(paletteItem);//palette 아이템 UI 불러오기, UI는 PhotonNetwork.Instantiate로 생성되지 않는다
        newObject.transform.SetParent(content);

        newObject.GetComponent<ItemInfo>().SetItemInfo(code, InventoryManager.instance.GetItemName(code));
        newObject.GetComponent<ItemInfo>().SetID(id);//아이템에 ID부여,
                                                     //나중에 Drawn Items목록에서 아이템을 선택하는 것이 아니라
                                                     //Palette에서 아이템을 직접 선택할때
                                                     //아이템을 구분할 구분자가 필요하다.

        //새로운 아이템의 transform을 초기화한다.
        newObject.transform.position = newControl.GetPosition();//Control Line이 있는 곳으로 오브젝트를 옮긴다. 실제 Palette의 중앙
        newObject.transform.rotation = Quaternion.Euler(Vector3.zero);//회전 초기화
        newObject.transform.localScale = Vector3.one;//스케일 초기화


        newObject.GetComponent<Button>().onClick.AddListener(() => SelectObject(id));
        newObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 1;

        //이미지 설정
        Image image = newObject.GetComponent<Image>();
        image.sprite = inventory.GetSprite(code);
        SpriteRenderer sr = newNetworkPrefab.GetComponent<SpriteRenderer>();
        Vector2 max = Camera.main.WorldToScreenPoint(sr.bounds.max);
        Vector2 min = Camera.main.WorldToScreenPoint(sr.bounds.min);
        image.rectTransform.sizeDelta = newControl.GetScaleScreenToUI(new Vector2(max.x - min.x, max.y - min.y));
        RectTransform tempRectTf = newObject.GetComponent<RectTransform>();
        Vector2 point = new Vector2(tempRectTf.position.x, tempRectTf.position.y) + tempRectTf.rect.size / 2;
        newControl.SetInitialSize(point);
        newObject.SetActive(true);//활성화

        //Client에 이미지 적용
        newNetworkPrefab.GetComponent<PaletteItemView>().SetImageOnClient(InventoryManager.instance.GetItemName(code));

        idToObject.Add(id, newObject);//ID와 아이템 저장
        idToControl.Add(id, newControl);//ID와 Control Line저장
        idToNetworkPrefabs.Add(id, newNetworkPrefab);//ID와 네트워크용 프리팹 저장
    }

    //그려져 있는 아이템을 지운다.
    public void EraseObject(int id)
    {
        Destroy(idToObject[id]);//아이템 삭제
        idToObject.Remove(id);//딕셔너리에서도 삭제

        Destroy(idToControl[id].gameObject);//Control Line도 삭제
        idToControl.Remove(id);//딕셔너리에서도 삭제

        PhotonNetwork.Destroy(idToNetworkPrefabs[id]);
        idToNetworkPrefabs.Remove(id);

        if (id == selectedID)//만약 현재 선택된 아이템을 삭제하는 경우
        {
            selectedID = -1;//선택된 아이템의 ID를 초기화한다. -> 아무것도 선택 안된 상태로 초기화
        }
    }

    //Palette에 그려져 있는 모든 아이템을 삭제한다.
    public void EraseAllObject()
    {
        List<int> ids = new List<int>(idToObject.Keys);
        for (int i = 0; i < ids.Count; i++)
        {
            EraseObject(ids[i]);
        }
    }

    //아이템을 선택하는 함수
    //이전에 표시된 Control Line을 숨기고 새롭게 선택된 아이템의 Control Line을 그린다.
    public void SelectObject(int id)
    {
        InitializeSelection();
        idToControl[id].Draw();//새롭게 선택된 아이템의 Control Line표시

        drawnItems.InitializeSelection();
        drawnItems.SelectObject(id);

        selectedID = id;//선택된 ID갱신
    }

    //선택을 해제하는 함수
    public void InitializeSelection()
    {
        //이전에 선택된 아이템이 없다면 실행될 필요가 없다.
        if (selectedID == -1) return;

        idToControl[selectedID].Hide();//이전에 선택된 아이템의 Control Line 감추기
        selectedID = -1;//선택ID초기화
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        //선택 초기화
        InitializeSelection();
        drawnItems.InitializeSelection();
    }
}