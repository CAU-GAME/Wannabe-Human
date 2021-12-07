using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 ItemInfo 클래스
- 아이템의 정보를 저장하기 위한 클래스
- 아이템의 코드/이름/아이디를 저장한다.

- 각 아이템에 부착

구성요소 설명
- 아이템 코드     : 아이템의 종류를 구분한다.
- 아이템 이름     : 아이템의 이미지를 불러오기 위해 필요하다.
- 아이템 아이디   : 각각의 아이템을 구분하기 위해 필요하다. -> 나중에 Palette에 아이템을 그릴때 설정된다.
 */

public class ItemInfo : MonoBehaviour
{
    public int code;//아이템 종류를 구분하는 아이템 코드
    public string itemName;//아이템 이름 -> 파일 입출력에 사용됨

    private int id = -1;//고유의 아이템을 구분할 정보
                        //나중에 문제 출제시 Palette에 아이템이 그려질 때 구분하기 위해 사용

    private static Dictionary<string, int> nameToCode;
    private static int itemCode = 0;

    private void Start()
    {
        itemName = gameObject.name;

        if(nameToCode == null)//맨처음만 할당할 수 있도록 한다.
        {
            Debug.Log("assigned!");
            nameToCode = new Dictionary<string, int>();
        }

        if (!nameToCode.ContainsKey(itemName))//아직 코드 부여가 되지 않은 경우
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

        if (nameToCode == null)//맨처음만 할당할 수 있도록 한다.
        {
            Debug.Log("assigned!");
            nameToCode = new Dictionary<string, int>();
        }

        if (!nameToCode.ContainsKey(itemName))//아직 코드 부여가 되지 않은 경우
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

    //아이템의 code와 name을 설정
    public void SetItemInfo(int code, string name)
    {
        this.code = code;
        this.itemName = name;
    }

    //아이템의 ID를 설정
    public void SetID(int id)
    {
        this.id = id;
    }

    //아이템의 ID 참조
    public int GetID()
    {
        return id;
    }
}
