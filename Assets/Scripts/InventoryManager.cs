using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 InventoryManager 클래스
- 이 클래스는 stage1(아이템 수집 단계)에서 아이템을 수집할 때, 어떤 아이템을 얼마나 모았는지 관리한다.

- Stage1 씬의 Inventory Manager객체에 부착

주요기능
- 어떤 아이템(코드로 구분)을 얼마나 모았는지 알 수 있어야 한다.
- 아이템의 이름을 알 수 있어야 한다.
    - 해당 아이템의 이미지와 3D 오브젝트가 Stage2에서 필요하다.
    - 이름은 이미지와 3D 오브젝트를 불러오기 위해 필요하다.
 */

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    //InventoryManager도 전역적으로 관리하기 위해 싱글턴 패턴 적용

    private Dictionary<int, int> itemCount;//코드(아이템 종류) -> 현재 가지고 있는 아이템 개수
    private Dictionary<int, string> Lookup;//코드 -> 아이템 이름

    private int maxItemKinds;//획득 가능한 아이템의 최대 가지수
    private int howManyKinds = 0;//몇 종류의 아이템을 수집했는지 확인하기 위한 변수

    void Start()
    {
        if(instance == null)
        {
            instance = this;//자기 자신 참조

            //객체 초기화
            itemCount = new Dictionary<int, int>();
            Lookup = new Dictionary<int, string>();

            maxItemKinds = GameManager.instance.maxItemNum;//획득 가능한 아이템의 최대 가지수 설정

            DontDestroyOnLoad(gameObject);//다른 씬에서도 쓸 수 있도록
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    //인벤토리에 아이템을 추가한다.
    //ItemInfo는 아이템에 대한 정보를 가지고 있는 클래스이다.
    //아이템의 이름과 아이템의 코드를 저장하고 있다.
    //Stage1에서 사용하는 함수
    public bool AddItem(ItemInfo item)
    {
        //해당 아이템이 인벤토리에 있는 경우, 전에 수집된 아이템인 경우
        if (itemCount.ContainsKey(item.code))
        {
            //개수를 하나 증가한다.
            itemCount[item.code] += 1;
        }
        else//처음 수집한 아이템인 경우
        {
            if (howManyKinds == maxItemKinds) return false;//이미 최대 가지수만큼 아이템을 모은 경우는 추가할 수 없다.
            itemCount.Add(item.code, 1);//인벤토리에 새롭게 아이템을 추가한다.
            Lookup.Add(item.code, item.itemName);//아이템 이름을 알 수 있도록 이름도 추가
            howManyKinds += 1;
        }
        return true;
    }

    //인벤토리에 아이템이 비어졌다가 다시 채워지는 경우 -> Drawn Items 목록에서 추가 신호가 오는 경우
    //이때는 code만 있어도 된다.
    //이 함수는 인벤토리에 추가하는 것은 동일하다. 하지만 코드만 이용해 추가한다.
    //Stage2에서 사용하기 위한 함수
    public bool AddItem(int code)
    {
        if (itemCount.ContainsKey(code))
        {
            itemCount[code] += 1;
        }
        else//비어졌다가 채워지는 경우
        {
            if (howManyKinds == maxItemKinds) return false;//이미 최대 가지수만큼 아이템을 모은 경우는 추가할 수 없다.
            itemCount.Add(code, 1);
            howManyKinds += 1;
        }
        return true;
    }

    //인벤토리에서 아이템을 꺼낸다.
    //개수를 하나 감소시킨다.
    //아이템을 잘 꺼내면 true 아니면 false
    public bool PickupItem(int code)
    {
        if (itemCount.ContainsKey(code))//아이템이 아직 Inventory에 남아있는 경우
        {
            //개수가 0이 아닌 경우에만 개수 감소
            if (itemCount[code] > 0) itemCount[code] -= 1;
            //개수를 줄인 후에 개수가 0이 되면 inventory에서 삭제한다.
            if (itemCount[code] == 0)
            {
                itemCount.Remove(code);
                howManyKinds -= 1;
            }
            return true;
        }
        return false;
    }

    //현재 아이템이 몇 개나 있는지 확인하는 함수
    public int GetItemCount(int code)
    {
        //아이템이 아직 인벤토리에 남아있는 경우 개수를 반환
        if (itemCount.ContainsKey(code)) return itemCount[code];
        else return 0;//없는 경우는 0을 반환
    }

    //아이템의 이름을 알아내기 위한 함수
    public string GetItemName(int code)
    {
        //아이템이 있는 경우 Lookup 테이블에서 이름을 반환한다.
        if (Lookup.ContainsKey(code)) return Lookup[code];
        return "";//없는 경우는 ""반환
    }

    //아이템의 이미지를 불러온다.
    public Sprite GetSprite(int code)
    {
        Sprite sprite = Resources.Load<Sprite>(GameManager.instance.itemImagePath + GetItemName(code));
        return sprite;
    }

    //현재 가지고 있는 아이템의 코드를 리스트 형태로 반환한다.
    public List<int> GetKeys()
    {
        return new List<int>(itemCount.Keys);
    }

    //현재 어떤 아이템이 저장되어 있는지 출력 - 디버깅용
    public void ShowItems()
    {
        string result = "\n";
        foreach (KeyValuePair<int, int> item in itemCount)
        {
            result += "key : " + item.Key + " value : " + item.Value + "\n";
        }
        Debug.Log(result);
    }

    //가지고 있는 아이템을 초기화한다.
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