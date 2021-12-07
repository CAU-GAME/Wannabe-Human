using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 NoticeManager 클래스
- 알림 문구를 관리하는 클래스
- 해당하는 알림 문구를 보여주고 숨기는 역할을 한다.
- 알림 문구를 보여주기 전에 알림문구의 정보를 초기화하는 역할을 담당한다.

- Notice오브젝트에 부착

알림 문구의 종류
Problem Stage
- 어떤 플레이어가 정답을 맞췄을 때 -> Correct
- 아무도 정답을 맞추지 못했을 때 -> Wrong
- 게임이 종료되었을 때, 최종 결과 -> Result

Main Stage
- 아이템 수집을 시작할 때, 게임의 제시어를 말해준다. -> GuideWord
- 아이템 수집이 완료되고, 게임의 순서를 말해준다. -> GuideOrder
- 아이템 수집 시, 플레이어에게 게임의 제시어와 남은 제한 시간을 말해준다. -> GuideState
 */

public enum Notice
{
    Correct = 0,
    Wrong = 1,//기본값
    Result = 2,
    GuideWord,
    GuideOrder,
    GuideState
}

public class NoticeManager : MonoBehaviour
{
    private GameManager gameManager;

    public CorrectNoticeSetter correct;//Correct알림
    public WrongNoticeSetter wrong;//Wrong알림
    public ResultNoticeSetter result;//Result알림

    public GuideWordSetter guideWord;//제시어 안내
    public GuideOrderSetter guideOrder;//Quiz 순서 안내
    public GuideStateSetter guideState;//Collectin Time 때, 제한시간/스킬/제시어 안내

    private string nextMsg = "Next Turn is 000";//다음 차례 메시지

    void Start()
    {
        gameManager = GameManager.instance;
    }

    //주어진 알림을 표시하는 함수 알림을 표시하기 전에 관련정보를 세팅해야 한다.
    public void ShowNotice(Notice state)
    {
        switch (state)
        {
            //Correct알림인 경우
            case Notice.Correct:
                //먼저 다음 차례를 세팅한다.
                SetNextMessage();

                correct.SetNextTurn(nextMsg);//다음 차례를 표시한다.
                //정답 결과를 초기화한다.
                correct.SetAnswer(gameManager.GetCurProposedWord());//제시어를 공개한다.
                correct.SetSolverAndDrawer(gameManager.GetCurSolver().name, gameManager.GetCurDrawer().name);//정답자와 출제자를 표시한다.

                correct.gameObject.SetActive(true);//알림 활성화
                break;

            //Wrong알림인 경우
            case Notice.Wrong:
                //먼저 다음 차례를 세팅한다.
                SetNextMessage();

                wrong.SetNextTurn(nextMsg);//다음차례 알림
                //정답 결과를 초기화한다.
                wrong.SetAnswer(gameManager.GetCurProposedWord());//제시어 공개
                wrong.gameObject.SetActive(true);//알림 활성화
                break;

            //Result알림인 경우
            case Notice.Result:
                result.SetResult(gameManager.SortByScore());//점수 순으로 세팅한다.
                result.gameObject.SetActive(true);//활성화
                break;

            //GuideWord인 경우
            case Notice.GuideWord:
                guideWord.SetProposedWord(gameManager.localPlayer.proposedWord);//제시어 세팅
                guideWord.SetLimitTimeAndItemNum(gameManager.itemLimitTime, gameManager.maxItemNum);//아이템 최대 개수 세팅
                guideWord.gameObject.SetActive(true);//활성화
                break;

            //GuideOrder인 경우
            case Notice.GuideOrder:
                guideOrder.SetOrders(gameManager.players);//player목록을 받아서 출제 순서대로 세팅한다.
                guideOrder.gameObject.SetActive(true);//활성화
                break;

            //GuideState인 경우
            case Notice.GuideState:
                guideState.SetWord(gameManager.localPlayer.proposedWord);//제시어를 초기화한다.
                guideState.gameObject.SetActive(true);//활성화
                break;
        }
    }

    //해당하는 알림을 숨긴다.
    public void HideNotice(Notice state)
    {
        switch (state)
        {
            case Notice.Correct:
                correct.gameObject.SetActive(false);
                break;

            case Notice.Wrong:
                wrong.gameObject.SetActive(false);
                break;

            case Notice.Result:
                result.gameObject.SetActive(false);
                break;

            case Notice.GuideWord:
                guideWord.gameObject.SetActive(false);
                break;

            case Notice.GuideOrder:
                guideOrder.gameObject.SetActive(false);
                break;

            case Notice.GuideState:
                guideState.gameObject.SetActive(false);
                break;
        }
    }

    //알림 문구가 표시되는 시간을 세팅하는 함수
    public void SetTime(Notice notice, float time)
    {
        switch (notice)
        {
            case Notice.Correct:
                correct.SetTime(time);
                break;

            case Notice.Wrong:
                wrong.SetTime(time);
                break;

            case Notice.Result://게밍 결과를 알려줄 때는 시간을 알릴 필요가 없다.
                break;

            case Notice.GuideWord:
                guideWord.SetTime(time);
                break;

            case Notice.GuideOrder:
                guideOrder.SetTime(time);
                break;

            case Notice.GuideState:
                guideState.SetTime(time);
                break;
        }
    }

    //Correct와 Wrong알림을 초기화할 때 사용되는 nextMessage를 초기화한다.
    public void SetNextMessage()
    {
        int curOrder;
        //StageManager에서 problem count를 하기 전
        curOrder = gameManager.problemCount;//현재 카운트는 현재 순번

        if (curOrder < gameManager.totalPlayerNum - 1)
        {
            nextMsg = "다음 차례는 " + gameManager.players[curOrder + 1].name + " 님 입니다!";//다음 차례 세팅
        }
        else if (curOrder == gameManager.totalPlayerNum - 1)//마지막 문제인 경우
        {
            nextMsg = "마지막 문제 입니다!";
        }//그 외의 경우는 curOrder == PlayerInfo.PlayerNum인 경우 -> 문제를 다 풀었으므로 결과가 출력되어야 한다.
    }
}
