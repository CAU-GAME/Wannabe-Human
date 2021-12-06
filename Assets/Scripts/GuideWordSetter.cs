using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 GuideWordSetter Ŭ����
- GuideWord : �÷��̾� 4���� ���� ��, ������ ���۵� ��, �����ڿ��� ���þ �˷��ش�. 
              �׿� ���ѽð� ��, ȹ�氡���� �������� �ִ� �������� �ȳ��Ѵ�.
���
 - ���þ� �ʱ�ȭ
 - ���� �ð� �ʱ�ȭ
 - ������ �ִ� ������ �ʱ�ȭ
 - �˸� �ð� �ʱ�ȭ
 */

public class GuideWordSetter : MonoBehaviour
{
    public Text proposedWord;//���þ �ȳ��ϴ� ����
    public Text limitTime;//���ѽð��� �ȳ��ϴ� ����
    public Text maxItemNum;//ȹ���� �� �ִ� �������� �ִ� ������
    public Slider circleTimer;//������ ���� �پ��� �ִϸ��̼��� ǥ���� UI
    public Text timer;//���� �ð� �ؽ�Ʈ

    public void SetProposedWord(string word)
    {
        proposedWord.text = word;
    }

    public void SetLimitTimeAndItemNum(float time, int num)
    {
        limitTime.text = "" + (int)time;
        maxItemNum.text = "" + num;
    }

    //���� �ð� ����
    public void SetTime(float time)
    {
        timer.text = ("" + ((int)time + 1) % 10);
        circleTimer.value = time;
    }
}
