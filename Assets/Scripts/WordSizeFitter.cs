using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 WordSizeFitter Ŭ����
- Main Stage���� ������ ������ ��, ���þ��� ���̿� ���缭 UIũ�⸦ �����Ѵ�.
- ������Ʈ�� Ȱ��ȭ �� �� �����
 */

public class WordSizeFitter : MonoBehaviour
{
    public float sidePadding = 15f;//�� �� padding�� ����
    public RectTransform content;//���ڰ� �ִ� �κ��� RectTransform
    private RectTransform rectTf;//���� ������Ʈ�� RectTransform

    private void OnEnable()
    {
        StartCoroutine("ApplyWordSize");
    }


    //�ڷ�ƾ �Լ� �����Ͽ� ��� ��� ��Ų��.
    // �ڷ�ƾ �Լ��� �������� �ʾ��� �� -> �����Ϳ� �����ִ� content���� ����Ǽ�, ������Ʈ ũ�Ⱑ ������ �ȵƴ�.
    // �׷��� content���� ����� ������ ��� �� ũ�⸦ �����Ѵ�.
    public IEnumerator ApplyWordSize()
    {
        yield return null;//���
        rectTf = GetComponent<RectTransform>();//RectTransform ����
        float width = content.rect.width + sidePadding * 2f;//�е��� ����
        rectTf.sizeDelta = new Vector2(width, rectTf.rect.height);//ũ�� ����
    }
}
