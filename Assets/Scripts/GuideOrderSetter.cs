using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/*
 GuideOrderSetter Ŭ����
- Main Stage���� ���δ�. Collecting Time�� �Ǳ� ���� Quiz������ �˸���.
- Guide Order : �÷��̾�� Quiz ���� ������ �˸���. GameManager�� ���� �÷��̾� ���� ������ �޾ƿͼ� UI�� �����ش�.
- Guide Order Setter�� �÷��̾� ������ �޾Ƽ� UI�� �ʱ�ȭ�Ѵ�.
 */

public class GuideOrderSetter : MonoBehaviour
{
    public Text orders;//������ �˷��� �ؽ�Ʈ
    public Slider circleTimer;//������ ���� �پ��� �ִϸ��̼��� ǥ���� UI
    public Text timer;//���� �ð� �ؽ�Ʈ

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

    //���� �ð� ����
    public void SetTime(float time)
    {
        timer.text = ("" + ((int)time + 1) % 10);
        circleTimer.value = time;
    }
}
