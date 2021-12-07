using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 NoticeManager Ŭ����
- �˸� ������ �����ϴ� Ŭ����
- �ش��ϴ� �˸� ������ �����ְ� ����� ������ �Ѵ�.
- �˸� ������ �����ֱ� ���� �˸������� ������ �ʱ�ȭ�ϴ� ������ ����Ѵ�.

- Notice������Ʈ�� ����

�˸� ������ ����
Problem Stage
- � �÷��̾ ������ ������ �� -> Correct
- �ƹ��� ������ ������ ������ �� -> Wrong
- ������ ����Ǿ��� ��, ���� ��� -> Result

Main Stage
- ������ ������ ������ ��, ������ ���þ �����ش�. -> GuideWord
- ������ ������ �Ϸ�ǰ�, ������ ������ �����ش�. -> GuideOrder
- ������ ���� ��, �÷��̾�� ������ ���þ�� ���� ���� �ð��� �����ش�. -> GuideState
 */

public enum Notice
{
    Correct = 0,
    Wrong = 1,//�⺻��
    Result = 2,
    GuideWord,
    GuideOrder,
    GuideState
}

public class NoticeManager : MonoBehaviour
{
    private GameManager gameManager;

    public CorrectNoticeSetter correct;//Correct�˸�
    public WrongNoticeSetter wrong;//Wrong�˸�
    public ResultNoticeSetter result;//Result�˸�

    public GuideWordSetter guideWord;//���þ� �ȳ�
    public GuideOrderSetter guideOrder;//Quiz ���� �ȳ�
    public GuideStateSetter guideState;//Collectin Time ��, ���ѽð�/��ų/���þ� �ȳ�

    private string nextMsg = "Next Turn is 000";//���� ���� �޽���

    void Start()
    {
        gameManager = GameManager.instance;
    }

    //�־��� �˸��� ǥ���ϴ� �Լ� �˸��� ǥ���ϱ� ���� ���������� �����ؾ� �Ѵ�.
    public void ShowNotice(Notice state)
    {
        switch (state)
        {
            //Correct�˸��� ���
            case Notice.Correct:
                //���� ���� ���ʸ� �����Ѵ�.
                SetNextMessage();

                correct.SetNextTurn(nextMsg);//���� ���ʸ� ǥ���Ѵ�.
                //���� ����� �ʱ�ȭ�Ѵ�.
                correct.SetAnswer(gameManager.GetCurProposedWord());//���þ �����Ѵ�.
                correct.SetSolverAndDrawer(gameManager.GetCurSolver().name, gameManager.GetCurDrawer().name);//�����ڿ� �����ڸ� ǥ���Ѵ�.

                correct.gameObject.SetActive(true);//�˸� Ȱ��ȭ
                break;

            //Wrong�˸��� ���
            case Notice.Wrong:
                //���� ���� ���ʸ� �����Ѵ�.
                SetNextMessage();

                wrong.SetNextTurn(nextMsg);//�������� �˸�
                //���� ����� �ʱ�ȭ�Ѵ�.
                wrong.SetAnswer(gameManager.GetCurProposedWord());//���þ� ����
                wrong.gameObject.SetActive(true);//�˸� Ȱ��ȭ
                break;

            //Result�˸��� ���
            case Notice.Result:
                result.SetResult(gameManager.SortByScore());//���� ������ �����Ѵ�.
                result.gameObject.SetActive(true);//Ȱ��ȭ
                break;

            //GuideWord�� ���
            case Notice.GuideWord:
                guideWord.SetProposedWord(gameManager.localPlayer.proposedWord);//���þ� ����
                guideWord.SetLimitTimeAndItemNum(gameManager.itemLimitTime, gameManager.maxItemNum);//������ �ִ� ���� ����
                guideWord.gameObject.SetActive(true);//Ȱ��ȭ
                break;

            //GuideOrder�� ���
            case Notice.GuideOrder:
                guideOrder.SetOrders(gameManager.players);//player����� �޾Ƽ� ���� ������� �����Ѵ�.
                guideOrder.gameObject.SetActive(true);//Ȱ��ȭ
                break;

            //GuideState�� ���
            case Notice.GuideState:
                guideState.SetWord(gameManager.localPlayer.proposedWord);//���þ �ʱ�ȭ�Ѵ�.
                guideState.gameObject.SetActive(true);//Ȱ��ȭ
                break;
        }
    }

    //�ش��ϴ� �˸��� �����.
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

    //�˸� ������ ǥ�õǴ� �ð��� �����ϴ� �Լ�
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

            case Notice.Result://�Թ� ����� �˷��� ���� �ð��� �˸� �ʿ䰡 ����.
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

    //Correct�� Wrong�˸��� �ʱ�ȭ�� �� ���Ǵ� nextMessage�� �ʱ�ȭ�Ѵ�.
    public void SetNextMessage()
    {
        int curOrder;
        //StageManager���� problem count�� �ϱ� ��
        curOrder = gameManager.problemCount;//���� ī��Ʈ�� ���� ����

        if (curOrder < gameManager.totalPlayerNum - 1)
        {
            nextMsg = "���� ���ʴ� " + gameManager.players[curOrder + 1].name + " �� �Դϴ�!";//���� ���� ����
        }
        else if (curOrder == gameManager.totalPlayerNum - 1)//������ ������ ���
        {
            nextMsg = "������ ���� �Դϴ�!";
        }//�� ���� ���� curOrder == PlayerInfo.PlayerNum�� ��� -> ������ �� Ǯ�����Ƿ� ����� ��µǾ�� �Ѵ�.
    }
}
