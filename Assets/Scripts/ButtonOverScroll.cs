using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 ButtonOverScroll 클래스
- 이 클래스는 Inventory Items(Inventory Item이 담길 UI)에서 버튼의 스크롤 기능을 구현한 컴포넌트이다.
- 오른쪽 버튼, 왼쪽 버튼에 각각 부착되어 있다.

주요기능
- Inventory Items는 두 개의 버튼이 있다.
- 오른쪽 이동/ 왼쪽 이동
- 마우스가 해당 버튼 위로 Over되면(해당 버튼의 영역 안으로 들어오게 되면) 그에 맞는 방향으로
  Inventory item들을 이동시켜야 한다.
 */

public class ButtonOverScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform content;//Waiting Item들이 담긴 오브젝트
    public bool isRight;//오른쪽버튼에 Over되었을 때, false면 왼쪽으로 이동
    public float speed = 20f;//이동 속도

    private bool isScrolling = false;//스크롤을 해야되는지 아닌지 판단할 변수
                                     //마우스가 버튼의 영역안으로 들어오면 true 아니면 false

    void Update()
    {
        if (isScrolling)//스크롤을 해야되는지 판단
        {
            if (isRight)//해당 방향에 맞게 content를 이동시킨다.
                content.anchoredPosition -= new Vector2(speed, 0f);
            else
                content.anchoredPosition += new Vector2(speed, 0f);
        }
    }

    //마우스가 버튼에 over됐을 때 실행되는 이벤트 함수
    public void OnPointerEnter(PointerEventData eventData)
    {
        isScrolling = true;
    }

    //마우스가 버튼영역에서 벗어났을 때 실행되는 이벤트 함수
    public void OnPointerExit(PointerEventData eventData)
    {
        isScrolling = false;
    }
}
