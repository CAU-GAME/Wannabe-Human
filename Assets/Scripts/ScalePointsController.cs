using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 ScalePointsController Ŭ����
- Scale Point���� ��ġ�� �����Ѵ�.
    - scale point : control line�� box�� �� �������� ��ġ�� ��
- Scale Factor�� ������ �����Ѵ�.
    - Scale Factor�� Scale ��ȭ���� ǥ���� ���̴�.
    - scale factor = ���콺 �巡�� ��ġ - ���콺 Ŭ�� ��ġ
- ���� � ��ư�� Ŭ���Ǿ����� �����Ѵ�.
 */

public class ScalePointsController : MonoBehaviour
{
    public RectTransform RightUp;//0
    public RectTransform LeftUp;//1
    public RectTransform RightDown;//2
    public RectTransform LeftDown;//3

    private int curPoint;

    //�� Scale Point���� ��ġ��Ű�� �Լ�
    //x, y�� �׻� ����� �޾ƾ� �Ѵ�. �׷��� �װ��� ���� ��Ī���� �����ϰ� ���� ������ �� �ִ�.
    public void PlacePoints(float x, float y)
    {
        RightUp.anchoredPosition = new Vector2(x, y);
        LeftUp.anchoredPosition = new Vector2(-x, y);//���� ���� y�� ��Ī�̹Ƿ� x���� -
        RightDown.anchoredPosition = new Vector2(x, -y);//������ �Ʒ��� x�� ��Ī�̹Ƿ� y���� -
        LeftDown.anchoredPosition = new Vector2(-x, -y);//���� �Ʒ��� ���� ��Ī�̹Ƿ� x,y �Ѵ� -
    }

    //������� �Է¿� ���� Scale Factor�� ������ �����Ѵ�.
    //4���� point�� ���ÿ� �����̰� �ϱ� ���� ������ �׻� (+,+)������ ���ͷ� �ٲ��ش�.
    public void SetFactor(ref Vector2 factor)
    {
        if(curPoint == 0)//Right Up Point
        {
            return;
        }
        else if(curPoint == 1)//Left Up Point
        {
            factor.x *= -1;
        }
        else if(curPoint == 2)//Right Down Point
        {
            factor.y *= -1;
        }
        else if(curPoint == 3)//Left Down Point
        {
            factor.x *= -1;
            factor.y *= -1;
        }
        return;
    }

    //�� ��ư�� Ŭ���Ǿ��� ��(Pointer Down�̺�Ʈ �߻���) ����Ǵ� �Լ�
    //�� ��ư���κ��� ID�� ���� �޴´�.
    //�� ��ư�� EventTrigger�� �̿��� �Լ��� �����Ų��. -> PointerDown�̺�Ʈ �߻��� ����ȴ�.
    public void SetCurPoint(int id)
    {
        curPoint = id;
    }
}
