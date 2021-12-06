using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 WheelScroll 클래스
- ScrollRect클래스를 상속받아서 일부기능을 없앴다.
- Wheel Scroll기능만 필요하므로 나머지 기능을 빈 함수로 오버라이딩함으로써 기능을 중지 시켰다.

- Chatting Room에 부착
- Drawn Items에 부착
 */

public class WheelScroll : ScrollRect
{
    //마우스 드래그 기능 정지
    public override void OnBeginDrag(PointerEventData eventData) { }
    public override void OnDrag(PointerEventData eventData) { }
    public override void OnEndDrag(PointerEventData eventData) { }
}
