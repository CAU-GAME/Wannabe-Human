using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 CorrectNoticeSetter 클래스
- Correct Notice의 관련정보를 초기화하는 클래스

- Notice > Correct에 부착

주요기능
- 정답을 공개한다.
- 정답이미지를 공개한다.
- 정답자와 출제자를 공개한다.
- 다음 차례가 누구인지 알린다.
- 현재 시간이 얼마나 남았는지 알린다.
- 시간이 줄어들고 있다는 애니메이션을 보여준다.
 */

public class CorrectNoticeSetter : MonoBehaviour
{
    public Text answer;//정답 텍스트
    public Text solver;//정답자
    public Text drawer;//출제자
    public Text nextTurn;//다음 차례 메시지
    public Slider circleTime;//시간 타이머 - 빨간색 원의 테두리가 줄어드는 모양
    public Text timer;//남은 시간을 표시할 텍스트

    //정답 세팅
    public void SetAnswer(string answer)
    {
        this.answer.text = answer;
    }

    //다음 차례 세팅
    public void SetNextTurn(string message)
    {
        nextTurn.text = message;
    }

    //남은 시간 세팅
    public void SetTime(float time)
    {
        timer.text = ("" + ((int)time + 1) % 10);
        circleTime.value = time;
    }

    //정답자와 출제자 세팅
    public void SetSolverAndDrawer(string solver, string drawer)
    {
        this.solver.text = "정답자 " + solver + " 님";
        this.drawer.text = "출제자 " + drawer + " 님";
    }
}
