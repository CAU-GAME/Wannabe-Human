using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 WordSizeFitter 클래스
- Main Stage에서 아이템 수집할 때, 제시어의 길이에 맞춰서 UI크기를 조정한다.
- 오브젝트가 활성화 될 때 적용됨
 */

public class WordSizeFitter : MonoBehaviour
{
    public float sidePadding = 15f;//양 옆 padding값 조절
    public RectTransform content;//글자가 있는 부분의 RectTransform
    private RectTransform rectTf;//현재 오브젝트의 RectTransform

    private void OnEnable()
    {
        StartCoroutine("ApplyWordSize");
    }


    //코루틴 함수 적용하여 잠깐 대기 시킨다.
    // 코루틴 함수를 적용하지 않았을 때 -> 에디터에 남아있던 content값이 적용되서, 오브젝트 크기가 조정이 안됐다.
    // 그래서 content값이 변경될 때까지 대기 후 크기를 조정한다.
    public IEnumerator ApplyWordSize()
    {
        yield return null;//대기
        rectTf = GetComponent<RectTransform>();//RectTransform 참조
        float width = content.rect.width + sidePadding * 2f;//패딩값 적용
        rectTf.sizeDelta = new Vector2(width, rectTf.rect.height);//크기 조절
    }
}
