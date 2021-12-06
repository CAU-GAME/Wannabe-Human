using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 Inventory Items Manager 클래스
- 아이템 수집단계에서 사용자가 획득한 아이템(Inventory Item)들이 표시될 공간(Inventory Items)을 관리한다.
- Inventory Items는 획득한 Inventory Item이 표시된다.
- Inventory Item의 구조
    - 버튼 : 클릭 이벤트를 받을 수 있다.
        - 이미지 : 어떤 아이템인지 표시된다.
        - 텍스트 : 개수가 표시된다.

- Inventory Items에 부착

주요기능
- Inventory Item은 현재 어떤 종류의 아이템을 가지고 있는지, 개수가 얼마나 되는지 표시된다.
- 가지고 있지 않은 Item은 Inventory Items목록에서 삭제된다.
- Inventory Item이 선택되는 경우
    - 가지고 있는 아이템 개수가 하나 감소되어야 한다.
    - Drawn Items 목록에 해당 아이템이 추가되어야 한다.
- Drawn Items에서 취소된 아이템은 Inventory Items목록에 추가된다.
- 처음 Stage가 시작될 때, Inventory에서 수집된 아이템 종류와 개수 정보를 받아와서
  Inventory Items에 표시한다.
 */

public class InventoryItemsManager : MonoBehaviour
{
    private Dictionary<int, GameObject> CodeToInventoryItem;//아이템의 code를 Inventory Item 오브젝트로 반환하는 Dictionary

    public RectTransform content;//Inventory Item들이 담길 곳, view port를 넘어가는 content는 가려진다.
    public GameObject inventoryItem;//새로운 Inventory Item을 생성할 때 복사할 오브젝트, 프리팹으로 준비
    public DrawnItemsManager drawnItems;//선택된 Inventory Item이 표시될 공간
    public GameObject disable;//현재 참가자의 차례가 아닐 때, Inventory Items의 선택을 막을 오브젝트

    private InventoryManager inventory;//사용자가 수집한 아이템 종류와 개수에 대한 정보를 참조할 인벤토리 객체

    void Start()
    {
        inventory = InventoryManager.instance;//참조할 인벤토리 정보를 가져온다.
        CodeToInventoryItem = new Dictionary<int, GameObject>();//필요한 객체 생성, Code -> Inventory Item

        //화면이 전환되고 나서 UI를 초기화하는 과정
        List<int> codes = inventory.GetKeys();//Inventory에서 현재 가지고 있는 아이템의 코드를 가져온다.
        for(int i = 0; i < codes.Count; i++)//순서대로 UI에 표시한다.
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
            image.rectTransform.sizeDelta = new Vector2(objectSize.x, objectSize.x / ratio);//x가 더 긴 경우
        }
        else
        {
            image.rectTransform.sizeDelta = new Vector2(objectSize.y * ratio, objectSize.y);//y가 더 긴 경우
        }
        image.rectTransform.position = tempRectTf.position;
    }

    //UI(Inventory Items 목록)에 아이템을 표시하는 함수
    //미리 준비해둔 객체(Inventory Item)를 복사하여 자리를 만들고 아이템 정보를 표시한다.
    public void DrawItem(int code)//아이템이 아직 표시되지 않은 상태에서 실행되는 함수
    {
        GameObject newObject = Instantiate(inventoryItem);//원본 Inventory Item을 복사함
        newObject.SetActive(true);//활성화
        newObject.transform.SetParent(content);//부모 객체의 자식으로 만든다. Content에 Inventory Item을 넣는다.
        newObject.transform.localScale = Vector3.one;//스케일을 조정한다. 갑자기 작아지는 경우가 있어서 초기화

        //인벤토리에서 필요한 정보를 불러온다.
        //아이템의 이미지 설정
        StartCoroutine(SetImage(code, newObject));

        newObject.transform.GetChild(1).GetComponent<Text>().text = "x" + inventory.GetItemCount(code);//아이템 개수를 표시한다.
        
        //아이템이 클릭되었을 때 이벤트를 등록한다.
        newObject.GetComponent<Button>().onClick.AddListener(() => OnClickItem(code));

        //코드와 Inventory Item을 관리하는 Dictionary에 현재 아이템을 추가한다.
        CodeToInventoryItem.Add(code, newObject);//Code -> Inventory Item
    }

    //Inventory Items 목록에 해당 아이템을 추가하는 함수 -> Drawn Items 목록에서 특정 아이템이 취소될 때 실행된다.
    //Inventory Items 목록에 이미 그려져 있는 상태인 경우 -> 아직 아이템을 덜 사용한 경우
    //또는 그려지지 않은 상태에서 추가되는 경우가 있다. -> 아이템을 모두 사용했던 경우
    public bool AddItem(int code)
    {
        //인벤토리에 아이템을 추가한다.
        if (inventory.AddItem(code) == false) return false;//아이템이 다 찬 경우는 추가하지 않는다.
        
        int num = inventory.GetItemCount(code);//인벤토리의 아이템 개수를 확인
        if(num == 1)//아이템이 없었던 경우(아이템을 모두 사용한 경우), UI에 아이템이 표시되어 있지 않았었으므로 다시 그려야한다.
        {
            DrawItem(code);//다시 아이템 그리기
        }
        else//그렇지 않은 경우(아이템을 아직 덜 사용한 경우)는 개수 표시만 갱신해주면 된다.
        {
            CodeToInventoryItem[code].transform.GetChild(1).GetComponent<Text>().text = "x" + num;//아이템 개수를 표시한다.
        }
        return true;
    }

    public bool AddItem(ItemInfo info)
    {
        //인벤토리에 아이템을 추가한다.
        if (inventory.AddItem(info) == false) return false;//아이템이 다 찬 경우는 추가하지 않는다.

        int num = inventory.GetItemCount(info.code);//인벤토리의 아이템 개수를 확인
        if (num == 1)//아이템이 없었던 경우(아이템을 모두 사용한 경우), UI에 아이템이 표시되어 있지 않았었으므로 다시 그려야한다.
        {
            DrawItem(info.code);//다시 아이템 그리기
        }
        else//그렇지 않은 경우(아이템을 아직 덜 사용한 경우)는 개수 표시만 갱신해주면 된다.
        {
            CodeToInventoryItem[info.code].transform.GetChild(1).GetComponent<Text>().text = "x" + num;//아이템 개수를 표시한다.
        }

        return true;
    }

    //Inventory Items 목록에 그려져 있는 아이템 정보를 지운다.
    public void EraseItem(int code)
    {
        Destroy(CodeToInventoryItem[code]);//객체를 지운다.
        CodeToInventoryItem.Remove(code);//딕셔너리의 요소를 지운다.
        //둘 중에 아무거나 하나만 하면 되는 것 같다.
    }

    //Inventory Item을 클릭했을 때 실행시킬 함수
    //아이템 개수가 하나 줄어들어야 한다.
    public void SelectItem(int code)//아이템 선택 시 개수를 하나 줄여야 한다.
    {
        bool has = inventory.PickupItem(code);//아이템 하나를 꺼내온다.
        if (!has) return;//가져오는데 실패한 경우

        int num = inventory.GetItemCount(code);//현재 아이템 개수를 확인한다.
        if (num == 0)//개수가 0이면 현재 그려져 있는 오브젝트를 지운다. -> 인벤토리에 아이템이 없음
        {
            EraseItem(code);
        }
        else
        {
            //아직 0이 아닌 경우는 개수를 갱신해준다.
            CodeToInventoryItem[code].transform.GetChild(1).GetComponent<Text>().text = "x" + num;//글자를 갱신한다.
        }
    }

    //Inventory Item의 선택을 비활성화한다.
    //비활성화 표시 Object(Disable)를 활성화 한다.
    //Disable 오브젝트는 Inventory Items위에 있는 Object로써
    //이 오브젝트가 활성화되면 Inventory Item의 선택을 막게 되므로 비활성화 효과를 줄 수 있다.
    public void DisableSelect()
    {
        disable.SetActive(true);
    }

    //Inventory Item선택을 활성화한다.
    //Disable오브젝트를 비활성화한다.
    public void EnableSelect()
    {
        disable.SetActive(false);
    }

    //Inventory Item이 클릭됐을 때 실행되는 함수 -> 이벤트 등록함수
    //필요기능
    //Inventory Item이 선택되고 그에 맞는 동작이 실행되어야 한다.
    //Drawn Items 목록에 해당 아이템을 추가해야 된다.
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
