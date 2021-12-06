using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 NamesSizeFitter Ŭ����
- �� Ŭ������ ���� ����� ǥ���� �� ���Ǵ� Ŭ����
- 1���� ������ ������ �����ڵ��� ������ ������ �� �̸��� ���̿� ����
  UI�� ũ�⸦ �����ϱ� ���� �������. -> ���� �� �̸��� �������� ũ�� ����
- Result Notice�� names�κп� �����Ǿ� �ִ�.
 */

public class NamesSizeFitter : MonoBehaviour
{
    public RectTransform[] names;//������ �̸��� RectTransform
    private RectTransform rectTf;//�̸��� ��� �ִ� UI�� RectTransform

    private void OnEnable()
    {
        rectTf = GetComponent<RectTransform>();
        float max = System.Single.MinValue;//���� �� �̸��� ���̸� ���Ѵ�.
        for(int i = 0; i < names.Length; i++)
        {
            float width = names[i].rect.width;
            if(max < width) max = width;
        }
        rectTf.sizeDelta = new Vector2(max, rectTf.sizeDelta.y);//���� �� �̸��� ũ�⸦ �����.
    }
}
