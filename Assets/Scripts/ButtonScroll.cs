using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 ButtonScroll 클래스
- 버튼을 눌렀을 때 해당방향으로 Content를 이동시키는 컴포넌트
- Inventory Items에 부착
    - 각 버튼에 onClick에 OnClickLeft, OnClickRight함수를 등록했다.

주요기능
- 버튼을 누르면 해당 방향으로 Inventory Item들이 이동한다.
- ScrollRect를 상속받아서 모든 기능을 중지함 
    -> Scroll Rect의 기능 중에서 Content가 넘어갔을 때 관성효과만 사용하기 위해서
 */

public class ButtonScroll : ScrollRect
{
    public RectTransform Content;

    public override void OnBeginDrag(PointerEventData eventData) { }
    public override void OnDrag(PointerEventData eventData) { }
    public override void OnEndDrag(PointerEventData eventData) { }
    public override void OnScroll(PointerEventData data) { }

    private float space = 100f;

    public void OnClickLeft()//왼쪽 버튼을 눌렀을 때 실행됨
    {
        Content.anchoredPosition += new Vector2(space, 0f);
    }

    public void OnClickRight()//왼쪽 버튼을 눌렀을 때 실행됨
    {
        Content.anchoredPosition -= new Vector2(space, 0f);
    }
}
