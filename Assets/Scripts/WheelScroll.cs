using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 WheelScroll Ŭ����
- ScrollRectŬ������ ��ӹ޾Ƽ� �Ϻα���� ���ݴ�.
- Wheel Scroll��ɸ� �ʿ��ϹǷ� ������ ����� �� �Լ��� �������̵������ν� ����� ���� ���״�.

- Chatting Room�� ����
- Drawn Items�� ����
 */

public class WheelScroll : ScrollRect
{
    //���콺 �巡�� ��� ����
    public override void OnBeginDrag(PointerEventData eventData) { }
    public override void OnDrag(PointerEventData eventData) { }
    public override void OnEndDrag(PointerEventData eventData) { }
}
