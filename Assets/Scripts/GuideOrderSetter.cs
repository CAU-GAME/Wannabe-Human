using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/*
 GuideOrderSetter 클래스
- Guide Order : 플레이어에게 문제 출제 순서를 알린다.
- Guide Order Setter는 플레이어 순서를 받아서 UI를 초기화한다.
 */

public class GuideOrderSetter : MonoBehaviour
{
    public Text orders;//순서를 알려줄 텍스트
    public Slider circleTimer;//빨간색 원이 줄어드는 애니메이션을 표시할 UI
    public Text timer;//남은 시간 텍스트

    public void SetOrders(List<PlayerInfo> players)
    {
        string result = "";
        int cnt = 1;
        foreach (var player in players)
        {
            if (cnt != players.Count) result += player.name + "\n";
            else result += player.name;
            cnt++;
        }

        orders.text = result;
    }

    //남은 시간 세팅
    public void SetTime(float time)
    {
        timer.text = ("" + ((int)time + 1) % 10);
        circleTimer.value = time;
    }
}
