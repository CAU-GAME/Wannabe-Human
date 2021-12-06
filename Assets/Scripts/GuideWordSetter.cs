using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 GuideWordSetter 클래스
- GuideWord : 플레이어 4명이 모인 후, 게임이 시작될 때, 참가자에게 제시어를 알려준다. 
              그외 제한시간 및, 획득가능한 아이템의 최대 가지수를 안내한다.
기능
 - 제시어 초기화
 - 제한 시간 초기화
 - 아이템 최대 가지수 초기화
 - 알림 시간 초기화
 */

public class GuideWordSetter : MonoBehaviour
{
    public Text proposedWord;//제시어를 안내하는 문구
    public Text limitTime;//제한시간을 안내하는 문구
    public Text maxItemNum;//획득할 수 있는 아이템의 최대 가지수
    public Slider circleTimer;//빨간색 원이 줄어드는 애니메이션을 표시할 UI
    public Text timer;//남은 시간 텍스트

    public void SetProposedWord(string word)
    {
        proposedWord.text = word;
    }

    public void SetLimitTimeAndItemNum(float time, int num)
    {
        limitTime.text = "" + (int)time;
        maxItemNum.text = "" + num;
    }

    //남은 시간 세팅
    public void SetTime(float time)
    {
        timer.text = ("" + ((int)time + 1) % 10);
        circleTimer.value = time;
    }
}
