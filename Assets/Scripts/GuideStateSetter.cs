using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 GuideStateSetter 클래스
- Guide State는 아이템 수집 시, 플레이어에게 남은 시간과 제시어를 보여준다.
- Guide State Setter클래스는 제시어를 세팅하고, 남은 시간을 세팅할 수 있도록 한다.
 */

public class GuideStateSetter : MonoBehaviour
{
    public Text timer;//남은 시간 표시
    public Text proposedWord;//제시어 표시
    public GameObject pressKeyGuide;//아이템 줍기 안내

    private void OnEnable()
    {
        if(pressKeyGuide != null)
        {
            pressKeyGuide.SetActive(false);
        }
    }

    //남은 시간을 적절하게 변형하여 알아보기 쉽게 표시한다.
    public string CalTime(float time)
    {
        time += 1;
        int min = (int)(time / 60f);
        int seconds = (int)(time - min * 60f);
        return (int)(min / 10f) + "" + (min % 10) + ":" + (int)(seconds / 10f) + "" + (seconds % 10);
    }

    public void SetTime(float time)
    {
        timer.text = "" + CalTime(time);
    }

    public void SetWord(string word)
    {
        proposedWord.text = "제시어 : " + word;
    }
}
