using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

/*
 MainStageManager 클래스
- Main Stage : 참가자가 로그인을 한 후, 들어오게 될 게임 공간
- 게임이 시작되기 전까지 아이템을 탐색하다가, 게임이 시작되면 아이템을 수집한다.

아이템 탐색 -> 아이템 수집

 Main Stage 상태 변화
    Searching : 초기 상태
    Searching -> GuidingWord : 게임 시작 버튼을 눌렀을 때
    GuidingWord -> Gathering : 알림 시간이 종료 됐을 때
    Gathering -> GuidingOrder : 게임 제한 시간이 지났을 때
    GuidingOrder -> Next Stage : 알림 시간이 종료 됐을 때
 */

public enum MainStageState
{
    None,//아무것도 아님, 초기값
    Searching,//아이템을 탐색할 때(처음 게임룸에 입장했을 때)
    Gathering,//아이템 수집할 때
    GuidingWord,//제시어 안내
    GuidingOrder,//게임 순서 안내
    NextStage//다음 게임 넘어감
}

public class MainStageManager : MonoBehaviourPun, IPunObservable
{
    private GameManager gameManager;//게임 메니저 참조
    private Camera cam;//카메라 : 아이템 탐색시, rightPanel이 보이는데, 이때 시야를 조정하기 위해 참조

    public Button gameStart;//게임 시작 버튼
    public PlayerInfoManager playerInfos;//접속한 플레이어의 이미지와 이름을 보여줄 공간

    public InventoryItemsManager inventoryItems;//inventory에 들어있는 아이템을 보여줄 공간
    public NoticeManager noticer;//알림 문구를 관리

    public RectTransform canvas;//캔버스의 RectTransform
    public RectTransform rightPanel;//playerInfos와 채팅창이 있는 UI

    private MainStageState state = MainStageState.None;
    private float limitTime;//아이템 수집을 제한할 시간
    private int maxItemNum;//아이템 개수를 제한
    private float noticeTime;//알림 문구 안내 시간

    public InputManager chatting;

    private void Awake() 
    {
        
    }

    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.mainStageManager = gameObject.GetComponent<MainStageManager>();

        cam = Camera.main;
        cam.rect = new Rect(-rightPanel.rect.width / canvas.rect.width, 0.0f, 1.0f, 1.0f);

        //상태 및 시간 초기화
        state = MainStageState.Searching;
        InitializeLimitTime();
        noticeTime = gameManager.noticeTime;
        maxItemNum = gameManager.maxItemNum;

        //플레이어 목록 초기화
        SetAllPlayerInfo();

        //게임 시작 버튼 클릭시 이벤트 등록
        gameStart.onClick.AddListener(OnClickGameStart);
        if (!PhotonNetwork.IsMasterClient)
        {
            gameStart.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case MainStageState.GuidingWord://제시어를 알려주는 상태
                if (noticeTime > 0)//알림 시간 측정
                {
                    noticeTime -= Time.deltaTime;
                    noticer.SetTime(Notice.GuideWord, noticeTime);
                }
                else
                {
                    ChangeState(MainStageState.Gathering);//아이템 수집 단계로 넘어간다.
                }
                break;

            case MainStageState.Gathering:
                if (limitTime > 0)//안내문구 출력 시간이 남아있는지 확인
                {
                    limitTime -= Time.deltaTime;
                    noticer.SetTime(Notice.GuideState, limitTime);
                }
                else
                {
                    ChangeState(MainStageState.GuidingOrder);
                }
                break;

            case MainStageState.GuidingOrder://플레이어에게 문제 출제 순서를 알려준다.
                if (noticeTime > 0)
                {
                    noticeTime -= Time.deltaTime;
                    noticer.SetTime(Notice.GuideOrder, noticeTime);
                }
                else
                {
                    ChangeState(MainStageState.NextStage);//다음 Stage로 넘어간다.(문제 출제)
                }
                break;
        }

        //디버깅용
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeState(MainStageState.GuidingOrder);
        }
    }

    //시간을 주기적으로 동기화한다.
    //그래야 모든 플레이어가 동시에 상태가 변할 수 있다.
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


    //각 상태 변화에 따른 설정 초기화
    public void ChangeState(MainStageState to)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("ApplyChangeState", RpcTarget.All, (int)to);
    }

    //호스트 -> 클라이언트
    [PunRPC]
    public void ApplyChangeState(int to)
    {
        state = (MainStageState)to;//현재 상태 변경
        switch (state)
        {
            case MainStageState.GuidingWord:
                gameManager.SetGamePlayingOrder();
                noticeTime = gameManager.noticeTime;//알림시간 초기화
                noticer.ShowNotice(Notice.GuideWord);//GuideWord알림 활성화
                break;

            case MainStageState.Gathering:

                noticer.HideNotice(Notice.GuideWord);//이전에 떠있던 알림 가리기

                InitializeLimitTime();//제한 시간 초기화
                noticer.ShowNotice(Notice.GuideState);//GuideState알림 활성화

                rightPanel.gameObject.SetActive(false);//rightPanel활성화(플레이어 목록, 채팅창)
                Camera.main.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);//카메라 시점 변경
                break;

            case MainStageState.GuidingOrder:

                noticer.HideNotice(Notice.GuideState);//이전 알림 가리기

                noticeTime = gameManager.noticeTime;//알림 시간 초기화
                noticer.ShowNotice(Notice.GuideOrder);//다음 알림 활성화
                break;

            case MainStageState.NextStage:
                InventoryManager.instance.ShowItems();//디버깅용 - 아이템 수집여부 확인
                SceneManager.LoadScene("Problem");//다음 씬 활성화
                break;
        }
    }



    //게임 시작 버튼 클릭 이벤트 함수
    //Searching -> GuidingWord
    public void OnClickGameStart()
    {
        ChangeState(MainStageState.GuidingWord);
    }

    [PunRPC]
    public void SetPlayerInfo(string name, int enter, int score)
    {
        playerInfos.SetPlayerInfo(name, enter, score);
        playerInfos.HideScore(enter);
    }

    public void ApplySetPlayerInfo(string name, int enter, int score)
    {
        photonView.RPC("SetPlayerInfo", RpcTarget.All, name, enter, score);
    }

    //참가자 정보 UI를 초기화한다.
    public void SetAllPlayerInfo()
    {
        Debug.Log("set all players info");
        foreach (var player in gameManager.players)
        {
            playerInfos.SetPlayerInfo(player);
            playerInfos.HideScore(player);
        }
    }

    public MainStageState GetCurState()
    {
        return state;
    }

    //제한 시간 초기화
    public void InitializeLimitTime()
    {
        limitTime = gameManager.itemLimitTime * 60f;
    }

    public bool AddItem(ItemInfo info)
    {
        if (inventoryItems.AddItem(info))
        {
            inventoryItems.DisableClick(info.code);
            return true;
        }
        return false;
    }
}
