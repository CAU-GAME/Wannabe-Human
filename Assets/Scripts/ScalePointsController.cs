using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 ScalePointsController 클래스
- Scale Point들의 위치를 조정한다.
    - scale point : control line의 box의 각 꼭지점에 위치한 원
- Scale Factor의 방향을 세팅한다.
    - Scale Factor는 Scale 변화량을 표현한 말이다.
    - scale factor = 마우스 드래그 위치 - 마우스 클릭 위치
- 현재 어떤 버튼이 클릭되었는지 감지한다.
 */

public class ScalePointsController : MonoBehaviour
{
    public RectTransform RightUp;//0
    public RectTransform LeftUp;//1
    public RectTransform RightDown;//2
    public RectTransform LeftDown;//3

    private int curPoint;

    //각 Scale Point들을 위치시키는 함수
    //x, y는 항상 양수로 받아야 한다. 그래야 네개의 점에 대칭으로 동일하게 값을 전달할 수 있다.
    public void PlacePoints(float x, float y)
    {
        RightUp.anchoredPosition = new Vector2(x, y);
        LeftUp.anchoredPosition = new Vector2(-x, y);//왼쪽 위는 y축 대칭이므로 x값에 -
        RightDown.anchoredPosition = new Vector2(x, -y);//오른쪽 아래는 x축 대칭이므로 y값에 -
        LeftDown.anchoredPosition = new Vector2(-x, -y);//왼쪽 아래는 원점 대칭이므로 x,y 둘다 -
    }

    //사용자의 입력에 따라서 Scale Factor의 방향을 설정한다.
    //4가지 point를 동시에 움직이게 하기 위해 변위를 항상 (+,+)방향의 벡터로 바꿔준다.
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

    //각 버튼이 클릭되었을 때(Pointer Down이벤트 발생시) 실행되는 함수
    //각 버튼으로부터 ID를 전달 받는다.
    //각 버튼에 EventTrigger를 이용해 함수를 실행시킨다. -> PointerDown이벤트 발생시 실행된다.
    public void SetCurPoint(int id)
    {
        curPoint = id;
    }
}
