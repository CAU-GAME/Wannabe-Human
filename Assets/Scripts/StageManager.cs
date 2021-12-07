using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

/*
 StageManager 클래스
- Problem Stage의 게임진행을 전반적으로 관리하는 클래스이다. 문제 출제- Quiz Time
- GameManager로부터 중요 정보를 참조하여 플레이어의 게임진행을 관리한다.
- Problem Stage는 크게 출제자와 문제를 푸는 사람으로 나뉜다.
- 출제자는 Main Stage에서 수집한 아이템을 이용해 본인의 제시어를 표현한다.
- 정답자는 출제자가 표현하는 그림을 보고 제시어를 유추해 정답을 맞춘다.

- Stage Manager에 부착

- Problem Stage는 게임의 상태가 크게 5가지가 있다.
    - Main Stage에서 Problem Stage로 넘어올 때(시작상태) -> Start
    - 본인의 아이템을 이용해 문제를 출제하고 있는 상태 -> Drawing
    - 다른 사람의 제시어를 맞추고 있는 상태 -> Solving
    - 문제출제/추리가 끝난후 정답을 공개하고 결과에 따라 점수를 부여하고 다음 사람을 알려주는 상태 -> Waiting
    - 모든 참가자의 문제 출제가 끝난 상태 -> End
- 순서도를 표시해보면
Start -> Drawing/Solving -> Waiting -> ... -> Drawing/Solving -> Waiting -> End 임을 알 수 있다.
 - 각 상태의 변화에 따라서 게임의 상태를 적절하게 초기화 해줘야 한다.

주요기능
    - 제한시간을 측정하여 문제풀이 시간을 제어한다.
    - 참가자에게 제한시간을 보여줘야 한다.
    - 문제 풀이 결과에 따라 사용자에게 적절한 안내문구를 표시해줘야 한다.
    - 안내문구를 적절한 시간에 맞춰 출력해줘야 한다.
    - 플레이어가 출제 중인 경우, 플레이어가 아이템을 선택해 그림을 그릴 수 있어야 한다.
    - 선택한 아이템을 다시 취소하여 캔버스에서 삭제할 수 있어야 한다.(캔버스 또는 팔레트 -> 출제자가 아이템을 이용해 그림을 그리는 공간)
    - 아이템은 아직 선택하지 않은 아이템(선택을 대기 중인 아이템, Inventory Items)과 선택한 아이템(그려진 아이템, Drawn Items)으로 나뉜다.
    - 플레이어는 본인이 받은 제시어를 왼쪽 상단에서 확인할 수 있어야 한다.
    - 채팅창을 이용해 사용자들과 채팅을 할 수 있어야 한다.
    - 채팅창에서 사용자의 입력을 감지하여 현재 제시어와 비교해야 한다.
    - 출제자는 문제 출제 도중 포기할 수 있어야 한다.
    - 각 플레이어의 캐릭터 이미지, 이름, 점수를 확인할 수 있어야 한다.
    - 게임이 끝난 후 다시 시작을 할 수 있도록 해야 한다.
 */
public enum PlayState
{
    Start,//제일 처음 진입할 때
    Drawing,//아이템을 이용해 제시어를 표현해야 한다.
    Solving,//다른 사람이 출제중인 문제를 맞춰야 한다.
    Waiting,//게임 안내 멘트가 나올 때
    End//게임이 종료된 상태
}

public class StageManager : MonoBehaviourPun, IPunObservable
{
    private GameManager gameManager;//게임정보를 참조할 GameManager

    //에디터에서 참조 설정
    public Text timer;//제한시간을 표시해줄 Timer
    public Button giveUp;//출제 포기 버튼
    public InputManager answer;//채팅입력창 겸 정답입력 창
    public NoticeManager noticer;//안내 문구 관리자
    public DrawnItemsManager drawnItems;//왼쪽 아이템 창, 그려진 아이템이 표시된다.
    public InventoryItemsManager inventoryItems;//위쪽 아이템 창, 아직 그려지지 않은 아이템이 표시된다.
    public PlayerInfoManager playerInfos;//플레이어 정보를 관리하고 UI에 보여준다.
    
    //현재 플레이어가 받은 제시어와 이미지, 왼쪽 상단에 표시된다.
    public Text proposedWord;//제시어

    //GameManager로부터 정보를 가져온다.
    public float limitTime;//제한시간이 저장되는 변수, 단위 : 초
    public float noticeTime;//안내문구가 표시되는 시간을 저장할 변수, 단위 : 초

    private PlayState state = PlayState.Start;//현재 문제를 출제중인지, 맞추는 중인지, 게임이 종료됐는지 구분
    private Notice notice = Notice.Wrong;//어떤 알림을 보여줘야 하는 지 구분

    private int enterNum = 0;

    void Start()
    {
        gameManager = GameManager.instance;//게임 정보를 참조하기 위한 GameManager
        gameManager.stageManager = gameObject.GetComponent<StageManager>();

        photonView.RPC("CountEnterNum", RpcTarget.MasterClient);

        InitializeLimitTime();//제한시간 세팅
        InitializeNoticeTime();//알림시간 세팅
        timer.text = CalTime(limitTime);//타이머에 제한시간 표시

        giveUp.onClick.AddListener(OnClickGiveUp);//출제 포기 버튼 클릭시 이벤트 등록
        //전송 버튼 클릭시 이벤트 등록
        //answer.button.onClick.AddListener(() =>
        //{
        //    CheckAnswerDelay(answer.input);
        //});
        //Click이벤트는 버튼을 눌렀다 뗏을때 실행된다.
        //Pointer Down은 버튼을 누르면 바로 실행된다. -> InputManager에서 pointer down이벤트로 처리해서 여기도 pointer down이벤트로 바꿨는데
        //화면 전환이 너무 빨리 되는 듯한 느낌이 있는 것 같다.
        //그런데 문제 검사하는 것과 채팅창이 업데이트 되는 부분은 네트워킹이 필요한 부분이므로
        //네트워킹을 구현했을 때 실행 순서를 잘 살펴봐야 될 것 같다.
        // -> 채팅창은 업데이트 됐는데, 정답체크가 다른 사람보다 느릴 수도 있을 것 같아서
        //   (전송버튼을 비정상적으로 오래 누르고 있는 경우 -> 버튼을 클릭하자마자 채팅창은 업데이트 됐는데,
        //   버튼을 떼기 전에, 다른사람이 버튼을 또 누르면 다른사람이 맞추게 될 수 있음)
        // -> 만약 이렇다면, 정답을 체크하는 것과 채팅창 업데이트를 같은 이벤트에 처리해야 될 수도 있을 것 같다.
        EventTrigger eventTrigger = answer.button.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entryPointerDown);

        //엔터키 클릭시 이벤트 등록
        answer.inputField.onEndEdit.AddListener((val) =>
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                CheckAnswerDelay(answer.input);//여기서 val을 넣어도 될 것 같다.
        });
        //Input Manager의 생성시점이 StageManager의 생성시점보다 빨라야한다. 그래서 Input Manager의 생성함수를 Awake로 변경함
        //그래야 Input Manager의 이벤트 등록함수인 Send가 먼저 실행되서 Answer.input을 제대로 받아올 수 있다.

        //플레이어의 제시어와 제시이미지 세팅
        proposedWord.text = gameManager.localPlayer.proposedWord;

        SetPlayersInfo();//현재 플레이어들의 정보를 화면에 보여준다.
    }

    void Update()
    {
        switch (state)//현재 플레이 상태 확인
        {
            case PlayState.Start://시작
                //PhotonView does not exist 에러때문에 추가했다.
                //아마도, ChangeState를 실행할 때, 다른 클라이언트에서 StageManager가 생성되기 전에, RPC를 실행해 생기는 문제인 것 같다.
                //그래서 모든 클라이언트가 확실하게 StageManager가 생성됐을 때 게임이 실행될 수 있도록 했다.
                //https://stackoverflow.com/questions/66952744/punrpc-not-getting-called
                if (enterNum == gameManager.totalPlayerNum)
                {
                    ChangeState();
                }
                //ChangeState();
                break;
            //안내문구 출력 상태
            case PlayState.Waiting:
                if (noticeTime > 0)//안내문구 출력 시간이 남아있는지 확인
                {
                    noticeTime -= Time.deltaTime;
                    //안내 표시에 시간을 업데이트해줘서 사용자에게 보여주어야 한다.
                    noticer.SetTime(notice, noticeTime);
                }
                else//안내가 끝난 경우
                {
                    ChangeState();
                }
                break;

                //문제 출제/ 또는 맞추기 상태
            case PlayState.Drawing:
            case PlayState.Solving:
                if (limitTime > 0)//시간 측정
                {
                    limitTime -= Time.deltaTime;
                    timer.text = CalTime(limitTime);//Timer에 제한시간을 표시한다.
                }
                else ChangeState();//시간이 다되면 안내문구를 출력해주는 상태로 변환
                break;

            case PlayState.End:
                break;
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("limit time : " + limitTime);
        //    Debug.Log("play state : " + state);
        //    Debug.Log("game manager problem count : " + gameManager.problemCount);
        //    Debug.Log("local player order : " + gameManager.localPlayer.order);
        //}
    }

    //시간을 주기적으로 동기화한다.
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(limitTime);
            stream.SendNext(noticeTime);
        }
        else
        {
            limitTime = (float)stream.ReceiveNext();
            noticeTime = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void CountEnterNum()
    {
        enterNum += 1;
    }

    //남은 시간을 적절하게 변형하여 알아보기 쉽게 표시한다.
    public string CalTime(float time)
    {
        time += 1;
        int min = (int)(time / 60f);
        int seconds = (int)(time - min * 60f);
        return (int)(min / 10f) + "" + (min % 10) + ":" + (int)(seconds / 10f) + "" + (seconds % 10);
    }

    //GameManager로부터 제한시간을 받아 초기화한다.
    public void InitializeLimitTime()
    {
        limitTime = gameManager.limitTime * 60f;
    }

    //GameManager로부터 안내시간을 받아 초기화한다.
    public void InitializeNoticeTime()
    {
        noticeTime = gameManager.noticeTime;
    }

    //출제 포기 버튼을 클릭했을 때 실행할 함수
    public void OnClickGiveUp()
    {
        Debug.Log("Give Up");
        notice = Notice.Wrong;//틀렸을 때 안내문구를 출력
        photonView.RPC("ChangeState", RpcTarget.MasterClient);
    }

    public PlayState DecideState(PlayState state)
    {
        PlayState to;

        if (state == PlayState.Start)
        {
            if (gameManager.IsMyTurn())
            {
                to = PlayState.Drawing;
            }
            else
            {
                to = PlayState.Solving;
            }
        }
        else if (state == PlayState.Waiting)
        {
            if (gameManager.IsGameFinished())
            {
                to = PlayState.End;
            }
            else
            {
                if (gameManager.IsMyTurn())
                {
                    to = PlayState.Drawing;
                }
                else
                {
                    to = PlayState.Solving;
                }
            }
        }
        else if (state == PlayState.Drawing || state == PlayState.Solving)
        {
            to = PlayState.Waiting;
        }
        else
        {
            to = PlayState.End;
        }
        return to;
    }

    [PunRPC]
    public void ChangeState()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        ApplyChangeState();
        photonView.RPC("ApplyChangeState", RpcTarget.Others);
    }

    [PunRPC]
    public void ChangeStateCorrectAnswer()
    {
        notice = Notice.Correct;
        ApplyChangeState();

        playerInfos.UpdatePlayerScore(gameManager.GetCurSolver());
        playerInfos.UpdatePlayerScore(gameManager.GetCurDrawer());
    }

    public void ApplyChangeStateCorrectAnswer()
    {
        photonView.RPC("ChangeStateCorrectAnswer", RpcTarget.Others);
    }

    [PunRPC]
    public void ApplyChangeState()//변경될 상태
    {
        PlayState to = DecideState(state);
        Debug.Log("to : " + to);
        state = to;//state를 갱신한다.
        switch (state)
        {
            case PlayState.Waiting://Waiting상태로 변하는 경우 -> 안내가 표시되는 경우
                InitializeLimitTime();//제한 시간 초기화
                noticer.ShowNotice(notice);//알림문구 출력

                drawnItems.palette.EraseAllObject();//palette에 남아있는 Object를 지운다.

                answer.DisableInput();//채팅창 입력 비활성화

                gameManager.CountProblem();//문제 수 카운트
                break;

            case PlayState.End://게임 종료가 되는 경우
                noticer.HideNotice(notice);//표시되고 있던 안내문구를 감추고
                notice = Notice.Result;//게임 최종 결과 문구를 표시한다.
                noticer.ShowNotice(notice);
                break;

            default://Drawing상태 또는 Solving상태로 변하는 경우
                InitializeNoticeTime();//알림시간 초기화
                noticer.HideNotice(notice);//알림문구 감추기
                notice = Notice.Wrong;//알림문구상태 초기화

                if (state == PlayState.Drawing)
                {
                    //아이템 선택 활성화
                    drawnItems.EnableSelect();
                    inventoryItems.EnableSelect();

                    giveUp.gameObject.SetActive(true);//출제 포기 버튼 활성화
                    answer.DisableInput();//채팅창 입력 비활성화
                }
                else if (state == PlayState.Solving)
                {
                    //아이템 선택 비활성화
                    drawnItems.DisableSelect();
                    inventoryItems.DisableSelect();

                    giveUp.gameObject.SetActive(false);//출제 포기 버튼 비활성화
                    answer.EnableInput();//채팅창 입력 활성화
                }
                //디버깅 용
                Debug.Log("current word : " + gameManager.players[gameManager.problemCount].proposedWord);
                break;
        }
    }

    public IEnumerator Delay()
    {
        yield return null;
    }

    //채팅창에서 받은 입력값을 확인한다.
    //입력값이 제시어와 동일할 경우, Waiting상태로 변경하고 알맞은 안내문구를 출력한다.
    //public void CheckAnswer(string input) yield return null;부분 변경됨
    //코루틴을 바로 종료 하려면 yield break를 해야한다
    public void CheckAnswer(string input)//정답이 맞는지 확인한다. Player의 점수도 바꿔야 하므로 Player정보도 필요하다.
    {
        if (input.Length == 0) return;

        if (gameManager.CheckAnswer(input, gameManager.localPlayer))//정답이 맞은 경우
        {
            //Debug.Log("(StageManager)correct !");
        }
    }

    public void CheckAnswerDelay(string input)
    {
        Delay();
        CheckAnswer(input);
    }
    //코루틴을 이용한 이유
    //엔터키를 눌러서 Send를 실행하는 것은 에러없이 실행이 된다. 그런데 버튼을 클릭했을 때
    //Attempting to select while already selecting an object에러가 발생했다.
    //에러 원인은 inputField가 선택된 상태에서 Button이 또 선택됐기 때문인 것 같다. 한프레임 내에서?
    // 두개의 UI가 동시에 선택되면 안되는 것 같다.
    //https://answers.unity.com/questions/1315276/event-system-setselectedgameobject-error-but-code.html
    //여기를 참고해보니 두번째 버튼이 실행될 때 delay를 주면 된다고 해서 일단 그렇게 오류를 해결했다.
    //에러가 있어도 게임진행에 큰 문제는 없었지만 혹시나 해서 해결했다.
    //일단 onEndEdit이벤트 발생 시에는 문제가 없었지만 Button이벤트와 같은 시점에 실행되도록 똑같이 CheckAnswerDelay 함수를 등록했다.


    //오른쪽 상단에 현재 플레이어들의 상태를 초기화한다.
    public void SetPlayersInfo()
    {
        foreach(var player in gameManager.players)
        {
            playerInfos.SetPlayerInfo(player);
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        CheckAnswerDelay(answer.input);
    }
}
