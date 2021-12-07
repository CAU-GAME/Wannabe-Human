using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 ButtonOverScroll Ŭ����
- �� Ŭ������ Inventory Items(Inventory Item�� ��� UI)���� ��ư�� ��ũ�� ����� ������ ������Ʈ�̴�.
- ������ ��ư, ���� ��ư�� ���� �����Ǿ� �ִ�.

�ֿ���
- Inventory Items�� �� ���� ��ư�� �ִ�.
- ������ �̵�/ ���� �̵�
- ���콺�� �ش� ��ư ���� Over�Ǹ�(�ش� ��ư�� ���� ������ ������ �Ǹ�) �׿� �´� ��������
  Inventory item���� �̵����Ѿ� �Ѵ�.
 */

public class ButtonOverScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform content;//Waiting Item���� ��� ������Ʈ
    public bool isRight;//�����ʹ�ư�� Over�Ǿ��� ��, false�� �������� �̵�
    public float speed = 20f;//�̵� �ӵ�

    private bool isScrolling = false;//��ũ���� �ؾߵǴ��� �ƴ��� �Ǵ��� ����
                                     //���콺�� ��ư�� ���������� ������ true �ƴϸ� false

    void Update()
    {
        if (isScrolling)//��ũ���� �ؾߵǴ��� �Ǵ�
        {
            if (isRight)//�ش� ���⿡ �°� content�� �̵���Ų��.
                content.anchoredPosition -= new Vector2(speed, 0f);
            else
                content.anchoredPosition += new Vector2(speed, 0f);
        }
    }

    //���콺�� ��ư�� over���� �� ����Ǵ� �̺�Ʈ �Լ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        isScrolling = true;
    }

    //���콺�� ��ư�������� ����� �� ����Ǵ� �̺�Ʈ �Լ�
    public void OnPointerExit(PointerEventData eventData)
    {
        isScrolling = false;
    }
}
