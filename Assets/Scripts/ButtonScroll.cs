using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 ButtonScroll Ŭ����
- ��ư�� ������ �� �ش�������� Content�� �̵���Ű�� ������Ʈ
- Inventory Items�� ����
    - �� ��ư�� onClick�� OnClickLeft, OnClickRight�Լ��� ����ߴ�.

�ֿ���
- ��ư�� ������ �ش� �������� Inventory Item���� �̵��Ѵ�.
- ScrollRect�� ��ӹ޾Ƽ� ��� ����� ������ 
    -> Scroll Rect�� ��� �߿��� Content�� �Ѿ�� �� ����ȿ���� ����ϱ� ���ؼ�
 */

public class ButtonScroll : ScrollRect
{
    public RectTransform Content;

    public override void OnBeginDrag(PointerEventData eventData) { }
    public override void OnDrag(PointerEventData eventData) { }
    public override void OnEndDrag(PointerEventData eventData) { }
    public override void OnScroll(PointerEventData data) { }

    private float space = 100f;

    public void OnClickLeft()//���� ��ư�� ������ �� �����
    {
        Content.anchoredPosition += new Vector2(space, 0f);
    }

    public void OnClickRight()//���� ��ư�� ������ �� �����
    {
        Content.anchoredPosition -= new Vector2(space, 0f);
    }
}
