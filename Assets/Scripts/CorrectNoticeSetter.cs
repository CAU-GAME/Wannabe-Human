using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 CorrectNoticeSetter Ŭ����
- Correct Notice�� ���������� �ʱ�ȭ�ϴ� Ŭ����

- Notice > Correct�� ����

�ֿ���
- ������ �����Ѵ�.
- �����̹����� �����Ѵ�.
- �����ڿ� �����ڸ� �����Ѵ�.
- ���� ���ʰ� �������� �˸���.
- ���� �ð��� �󸶳� ���Ҵ��� �˸���.
- �ð��� �پ��� �ִٴ� �ִϸ��̼��� �����ش�.
 */

public class CorrectNoticeSetter : MonoBehaviour
{
    public Text answer;//���� �ؽ�Ʈ
    public Text solver;//������
    public Text drawer;//������
    public Text nextTurn;//���� ���� �޽���
    public Slider circleTime;//�ð� Ÿ�̸� - ������ ���� �׵θ��� �پ��� ���
    public Text timer;//���� �ð��� ǥ���� �ؽ�Ʈ

    //���� ����
    public void SetAnswer(string answer)
    {
        this.answer.text = answer;
    }

    //���� ���� ����
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

    //�����ڿ� ������ ����
    public void SetSolverAndDrawer(string solver, string drawer)
    {
        this.solver.text = "������ " + solver + " ��";
        this.drawer.text = "������ " + drawer + " ��";
    }
}
