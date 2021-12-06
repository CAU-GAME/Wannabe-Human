using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 SelectInitializer Ŭ����
- ����ڰ� ��Ʈ�� ������ Ŭ������ �ʰ� �ٸ� ���� Ŭ�������� Drawn Item�� ������ �����ϱ����� Ŭ�����̴�.
- ���� ��� ���þ�/�����̹����� Ŭ���ϰų�, ä��â�̳� �����ڵ� ������ ������ �κ��� Ŭ���ϸ�
  ���� ���õ� Drawn Item�� ������ ������Ų��.

- Canvas Object�� �����Ǿ� �ִ�.
 */
public class SelectInitializer : MonoBehaviour, IPointerDownHandler
{
    public DrawnItemsManager drawnItems;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        drawnItems.palette.InitializeSelection();
        drawnItems.InitializeSelection();
    }
}
