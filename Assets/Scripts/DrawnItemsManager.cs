using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 DrawnItemsManager 클래스
- 사용자가 문제를 출제할 때, Inventory Items에서 선택한 아이템이 표시되는 공간(Drawn Items)에 
  표시될 아이템(Dranw Item)을 관리하는 클래스이다.
- Drawn Items에 Drawn Item이 표시된다.
- Drawn Item의 구조
    - 버튼 : 클릭 이벤트를 받을 수 있다.
        - 이미지    : 선택된 아이템 이미지를 표시한다.
        - 선택표시  : 현재 아이템이 선택되어 있는지 표시한다.
        - 취소버튼  : 선택된 아이템의 그리기를 취소할 수 있다.
- 선택된 Drawn Item은 Palette에 Palette Item으로 표시된다.

- Drawn Items에 부착

주요기능
- Waiting Items에서 선택된 아이템을 Drawn Items에 표시한다.
    - 그와 동시에 Palette에 해당 아이템의 Palette Item이 표시된다.
- Drawn Item은 해당 아이템의 이미지와 취소버튼이 있다.
- Drawn Item을 선택한 경우
    - Palette에 그려진 Palette Item의 위치, 회전, 크기를 변형할 수 있다.
    - 위치, 회전, 크기를 바꿀 수 있는 UI(제어 선, Control Line)이 표시된다.
    - 선택된 아이템이 어떤 것인지 표시가 되어야 한다.
- Drawn Item의 취소버튼을 클릭한 경우
    - Drawn Items목록에서 Drawn Item이 사라지고, 
    - Palette에서도 해당 아이템이 사라진다.
- Drawn Item은 Drawn Items표시창에서 아래에서부터 채워진다.
- Drawn Item의 개수가 Drawn Items표시창에 표시가능한 개수보다 많아질 경우
    - 선택된 Drawn Item이 가장 위쪽에 표시되어야 한다.
    - 휠 스크롤을 통해 가려진 Drawn Item을 찾을 수 있어야 한다.
 */

public class DrawnItemsManager : MonoBehaviour
{
    private static int btnID = 0;//Drawn Item들을 구분하기 위한 구분자(ID)
    private static Dictionary<int, GameObject> idToDrawnItem;//ID -> Drawn Item, ID로 DrawnItem을 접근하기 위한 Dictionary변수
    private Dictionary<int, int> idToCode;//ID -> 아이템의 코드, ID로 아이템의 코드를 접근하기 위한 변수, ID는 다르지만 아이템의 종류(코드 Code)는 같을 수 있다.
    //dic.Remove(key) 원소 삭제하기

    public RectTransform content;//Item Object의 부모 object, Drawn Item들이 담길 오브젝트의 RectTransform
    private RectTransform viewport;//content = Drawn Items가 나타나는 공간이 보이는 부분, viewport의 범위를 넘어가는 Drawn Item은 보이지 않는다.
    
    public GameObject drawnItem;//Drawn Item을 표시할 Object, 복사용이다. 프리팹으로 준비해두었다.
    public InventoryItemsManager inventoryItems;//Waiting Items들을 관리하는 컴포넌트
    public GameObject disable;//본인의 출제 차례가 아닐때 Drawn Item을 선택할 수 없도록 가리는 Object
    public bool placeOnTop = true;//Inventory Items에서 선택된 아이템이 Drawn Items의 가장 위쪽에 생기는데,
                                  //범위를 넘어가는 경우 안 보이게 된다.
                                  //그래서 그때 보이게 할 지 결정할 변수

    public PaletteManager palette;//Drawn Item의 3D Object 이미지가 그려질 공간

    private InventoryManager inventory;//사용할 인벤토리 객체
    private int selectedID = -1;//현재 선택된 object의 ID, -1은 선택안된 상태

    void Start()
    {
        //아이템의 종류와 개수를 확인하기 위해 Inventory Manager를 참조한다.
        inventory = InventoryManager.instance;
        //필요한 객체 생성
        idToDrawnItem = new Dictionary<int, GameObject>();
        idToCode = new Dictionary<int, int>();
        //viewport의 RectTransform을 받아온다.
        viewport = content.parent.GetComponent<RectTransform>();
    }

    //아이템 이미지 크기 조절
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
        //아이템 이미지에서 가로 세로 길이 중 기다란 쪽을 Drawn Item의 크기에 맞춘다.
        //길이가 짧은 쪽은 Ratio를 곱해서 비율을 맞춘다.
        if (imageSize.x > imageSize.y)
        {
            image.rectTransform.sizeDelta = new Vector2(objectSize.x, objectSize.x / ratio);//x가 더 긴 경우
        }
        else
        {
            image.rectTransform.sizeDelta = new Vector2(objectSize.y * ratio, objectSize.y);//y가 더 긴 경우
        }
        image.rectTransform.position = tempRectTf.position;
    }

    //Inventory Items에서 Item이 선택되면 실행될 함수
    //Drawn Items목록에 Drawn Item을 추가한다.
    //그리고 그에 해당하는 설정값 초기화
    public void DrawItem(int code)
    {
        GameObject newObject = Instantiate(drawnItem);//새로운 Drawn Item생성
        newObject.SetActive(true);//보이도록 활성화
        newObject.transform.SetParent(content);//content에 Drawn Item담기
        newObject.transform.localScale = Vector3.one;//크기가 변경되는 현상이 있으므로 스케일 값 조정

        //선택된 아이템의 이미지를 설정한다.
        //newObject.transform.GetChild(0).GetComponent<Image>().sprite = inventory.GetSprite(code);
        StartCoroutine(SetImage(code, newObject));

        //버튼의 아이디를 부여한다.
        int id = btnID;
        
        //Drawn Item이 클릭되었을때 이벤트들을 등록한다.
        newObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => OnClickCancel(id));//취소 버튼을 눌렀을 때 이벤트
        newObject.GetComponent<Button>().onClick.AddListener(() => OnClickItem(id));//Drawn Item을 눌렀을 때 이벤트
        //Id와 해당 아이템의 정보를 저장한다.
        idToDrawnItem.Add(id, newObject);
        idToCode.Add(id, code);
        //해당 아이템을 선택한다.
        InitializeSelection();
        SelectObject(id);
        btnID++;//새롭게 추가될 아이템 ID 갱신

        //Palette에 현재 선택된 아이템의 Palette Item을 그린다.
        palette.InitializeSelection();
        palette.DrawObject(id, code);//Palette Item그리기
        
        PlaceContentTop();//현재 선택된 아이템이 보이지 않는 경우 가장 위쪽에 보이도록 설정
    }

    //취소 버튼을 클릭했을 때 실행될 함수
    //현재 Drawn Items목록에서 해당 아이템의 정보를 삭제한다.
    public void EraseItem(int id)
    {
        Destroy(idToDrawnItem[id]);//객체를 삭제한다.
        idToDrawnItem.Remove(id);//딕셔너리의 요소를 삭제한다.

        if (id == selectedID)//만약 현재 선택상태인 아이템이 취소되는 경우
        {
            selectedID = -1;//아무것도 선택되지 않은 상태로 초기화
        }
    }

    //Drawn Item이 선택되었다는 것을 표시할 함수
    //선택표시를 활성화한다.
    public void SelectObject(int id)
    {
        idToDrawnItem[id].transform.GetChild(1).gameObject.SetActive(true);//선택 표시 그리기
        selectedID = id;//현재 선택된 ID갱신
    }

    //다른 아이템이 선택되는 경우
    //아이템이 아닌 다른 공간을 선택되는 경우
    //선택표시를 비활성화한다.
    public void InitializeSelection()
    {
        if(selectedID == -1)//선택되어 있는 경우가 아니면 종료
        {
            return;
        }
        //현재 선택되어 있는 아이템의 선택표시 비활성화
        idToDrawnItem[selectedID].transform.GetChild(1).gameObject.SetActive(false);
        selectedID = -1;//선택되어 있지 않은 상태로 초기화
    }

    //취소버튼을 눌렀을 때 이벤트 함수
    //필요기능
    //Drawn Items목록에서 해당 아이템을 지워야한다.
    //Control Line을 지워야 한다.
    //Inventory Items에 그려져야 한다.
    public void OnClickCancel(int id)
    {
        EraseItem(id);//Drawn Items목록에서 지우기
        palette.EraseObject(id);//Palette에 그려진 3D Object지우기
        inventoryItems.AddItem(idToCode[id]);//Waiting item에 다시 아이템을 추가한다.
    }

    //Drawn Item이 선택되었을 때 이벤트 함수
    //필요기능
    //Drawn Item의 선택표시가 활성화되어야 한다.
    //Palette에서 Palette Item을 편집할 수 있어야 한다. palette item선택하기
    public void OnClickItem(int id)
    {
        InitializeSelection();
        SelectObject(id);//선택표시 활성화
        palette.InitializeSelection();
        palette.SelectObject(id);//Palette에서 아이템 선택
    }

    //Inventory Items에서 선택된 Drawn Items가 viewport에 보이지 않을 경우
    //선택된 Drawn Item을 viewport가장 위쪽에 배치시켜야 한다.
    public void PlaceContentTop()
    {
        if (!placeOnTop) return;

        //content와 viewport의 높이를 구한다음
        float contentHeight = content.rect.height + drawnItem.GetComponent<RectTransform>().rect.height;
        //content의 높이를 구할 때, Drawn Item의 높이를 한 번 더 더하는 이유는
        //PlaceContentTop함수가 실행될 때 프레임이 덜 끝나서 높이정보가 반영이 안되기 때문이다.
        //이전 높이가 입력되기 때문에 높이를 더했다. 아니면 코루틴을 이용해 한프레임이 끝난 후 실행해도 될 것 같다.
        float viewportHeight = viewport.rect.height;
        //차이를 계산한다. content의 높이가 더 큰 경우가 Drawn Item이 가려지는 경우다.
        float diff = contentHeight - viewportHeight;
        if (diff > 0)
        {
            content.anchoredPosition -= new Vector2(0f, diff * 3f);
            //차이만큼만 이동시키면 Scroll Rect의 관성효과 때문에 잘 안보이게 된다.
            //그래서 차이보다 더 많이 이동시키면 애니메이션 효과도 있고 아이템도 잘보이게 된다.
        }
    }

    //Drawn Item의 선택을 비활성화한다.
    //비활성화 표시 Object(Disable)를 활성화 한다.
    //Disable 오브젝트는 Drawn Items위에 있는 Object로써
    //이 오브젝트가 활성화되면 Drawn Item의 선택을 막게 되므로 비활성화 효과를 줄 수 있다.
    public void DisableSelect()
    {
        disable.SetActive(true);
    }

    //Drawn Item선택을 활성화한다.
    //Disable오브젝트를 비활성화한다.
    public void EnableSelect()
    {
        disable.SetActive(false);
    }

    //현재 Drawn Items목록에 있는 아이템들을 모두 삭제한다.
    public void EraseAllObject()
    {
        List<int> ids = new List<int>(idToDrawnItem.Keys);
        for(int i = 0; i < ids.Count; i++)
        {
            Destroy(idToDrawnItem[ids[i]]);//객체 삭제
            idToDrawnItem.Remove(ids[i]);//Dictionary에서 삭제
            idToCode.Remove(ids[i]);
        }
    }
}
