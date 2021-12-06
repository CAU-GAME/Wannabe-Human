using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 NamesSizeFitter 클래스
- 이 클래스는 최종 결과를 표시할 때 사용되는 클래스
- 1위를 제외한 나머지 참가자들의 순위를 보여줄 때 이름의 길이에 따라서
  UI의 크기를 결정하기 위해 만들었다. -> 가장 긴 이름을 기준으로 크기 결정
- Result Notice의 names부분에 부착되어 있다.
 */

public class NamesSizeFitter : MonoBehaviour
{
    public RectTransform[] names;//참가자 이름의 RectTransform
    private RectTransform rectTf;//이름을 담고 있는 UI의 RectTransform

    private void OnEnable()
    {
        rectTf = GetComponent<RectTransform>();
        float max = System.Single.MinValue;//가장 긴 이름의 길이를 구한다.
        for(int i = 0; i < names.Length; i++)
        {
            float width = names[i].rect.width;
            if(max < width) max = width;
        }
        rectTf.sizeDelta = new Vector2(max, rectTf.sizeDelta.y);//가장 긴 이름에 크기를 맞춘다.
    }
}
