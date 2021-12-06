using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
WrongNoticeSetter 클래스
- Wrong Notice의 관련정보를 초기화하는 클래스

- Notice > Wrong에 부착

주요기능
- 정답을 공개한다.
- 정답이미지를 공개한다.
- 실패 소식을 알린다.
- 현재 시간이 얼마나 남았는지 알린다.
- 시간이 줄어들고 있다는 애니메이션을 보여준다.
*/

public class WrongNoticeSetter : MonoBehaviour
{
    public Text answer;//정답 텍스트
    public Text nextTurn;//다음차례
    public Slider circleTime;//빨간색 원이 줄어드는 애니메이션을 표시할 UI
    public Text timer;//남은 시간 텍스트

    //정답 세팅
    public void SetAnswer(string answer)
    {
        this.answer.text = answer;
    }

    //다음차례 세팅
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
}
