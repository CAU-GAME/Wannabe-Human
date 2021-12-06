using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
WrongNoticeSetter Ŭ����
- Wrong Notice�� ���������� �ʱ�ȭ�ϴ� Ŭ����

- Notice > Wrong�� ����

�ֿ���
- ������ �����Ѵ�.
- �����̹����� �����Ѵ�.
- ���� �ҽ��� �˸���.
- ���� �ð��� �󸶳� ���Ҵ��� �˸���.
- �ð��� �پ��� �ִٴ� �ִϸ��̼��� �����ش�.
*/

public class WrongNoticeSetter : MonoBehaviour
{
    public Text answer;//���� �ؽ�Ʈ
    public Text nextTurn;//��������
    public Slider circleTime;//������ ���� �پ��� �ִϸ��̼��� ǥ���� UI
    public Text timer;//���� �ð� �ؽ�Ʈ

    //���� ����
    public void SetAnswer(string answer)
    {
        this.answer.text = answer;
    }

    //�������� ����
    public void SetNextTurn(string message)
    {
        nextTurn.text = message;
    }

    //���� �ð� ����
    public void SetTime(float time)
    {
        timer.text = ("" + ((int)time + 1) % 10);
        circleTime.value = time;
    }
}
