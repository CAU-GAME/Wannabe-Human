using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

/*
 MainStageManager Ŭ����
- Main Stage : �����ڰ� �α����� �� ��, ������ �� ���� ����
- ������ ���۵Ǳ� ������ �������� Ž���ϴٰ�, ������ ���۵Ǹ� �������� �����Ѵ�.

������ Ž�� -> ������ ����

 Main Stage ���� ��ȭ
    Searching : �ʱ� ����
    Searching -> GuidingWord : ���� ���� ��ư�� ������ ��
    GuidingWord -> Gathering : �˸� �ð��� ���� ���� ��
    Gathering -> GuidingOrder : ���� ���� �ð��� ������ ��
    GuidingOrder -> Next Stage : �˸� �ð��� ���� ���� ��
 */

public enum MainStageState
{
    None,//�ƹ��͵� �ƴ�, �ʱⰪ
    Searching,//�������� Ž���� ��(ó�� ���ӷ뿡 �������� ��)
    Gathering,//������ ������ ��
    GuidingWord,//���þ� �ȳ�
    GuidingOrder,//���� ���� �ȳ�
    NextStage//���� ���� �Ѿ
}

public class MainStageManager : MonoBehaviourPun, IPunObservable
{
    private GameManager gameManager;//���� �޴��� ����
    private Camera cam;//ī�޶� : ������ Ž����, rightPanel�� ���̴µ�, �̶� �þ߸� �����ϱ� ���� ����

    public Button gameStart;//���� ���� ��ư
    public PlayerInfoManager playerInfos;//������ �÷��̾��� �̹����� �̸��� ������ ����

    public InventoryItemsManager inventoryItems;//inventory�� ����ִ� �������� ������ ����
    public NoticeManager noticer;//�˸� ������ ����

    public RectTransform canvas;//ĵ������ RectTransform
    public RectTransform rightPanel;//playerInfos�� ä��â�� �ִ� UI

    private MainStageState state = MainStageState.None;
    private float limitTime;//������ ������ ������ �ð�
    private int maxItemNum;//������ ������ ����
    private float noticeTime;//�˸� ���� �ȳ� �ð�

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

        //���� �� �ð� �ʱ�ȭ
        state = MainStageState.Searching;
        InitializeLimitTime();
        noticeTime = gameManager.noticeTime;
        maxItemNum = gameManager.maxItemNum;

        //�÷��̾� ��� �ʱ�ȭ
        SetAllPlayerInfo();

        //���� ���� ��ư Ŭ���� �̺�Ʈ ���
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
            case MainStageState.GuidingWord://���þ �˷��ִ� ����
                if (noticeTime > 0)//�˸� �ð� ����
                {
                    noticeTime -= Time.deltaTime;
                    noticer.SetTime(Notice.GuideWord, noticeTime);
                }
                else
                {
                    ChangeState(MainStageState.Gathering);//������ ���� �ܰ�� �Ѿ��.
                }
                break;

            case MainStageState.Gathering:
                if (limitTime > 0)//�ȳ����� ��� �ð��� �����ִ��� Ȯ��
                {
                    limitTime -= Time.deltaTime;
                    noticer.SetTime(Notice.GuideState, limitTime);
                }
                else
                {
                    ChangeState(MainStageState.GuidingOrder);
                }
                break;

            case MainStageState.GuidingOrder://�÷��̾�� ���� ���� ������ �˷��ش�.
                if (noticeTime > 0)
                {
                    noticeTime -= Time.deltaTime;
                    noticer.SetTime(Notice.GuideOrder, noticeTime);
                }
                else
                {
                    ChangeState(MainStageState.NextStage);//���� Stage�� �Ѿ��.(���� ����)
                }
                break;
        }

        //������
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeState(MainStageState.GuidingOrder);
        }
    }

    //�ð��� �ֱ������� ����ȭ�Ѵ�.
    //�׷��� ��� �÷��̾ ���ÿ� ���°� ���� �� �ִ�.
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


    //�� ���� ��ȭ�� ���� ���� �ʱ�ȭ
    public void ChangeState(MainStageState to)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("ApplyChangeState", RpcTarget.All, (int)to);
    }

    //ȣ��Ʈ -> Ŭ���̾�Ʈ
    [PunRPC]
    public void ApplyChangeState(int to)
    {
        state = (MainStageState)to;//���� ���� ����
        switch (state)
        {
            case MainStageState.GuidingWord:
                gameManager.SetGamePlayingOrder();
                noticeTime = gameManager.noticeTime;//�˸��ð� �ʱ�ȭ
                noticer.ShowNotice(Notice.GuideWord);//GuideWord�˸� Ȱ��ȭ
                break;

            case MainStageState.Gathering:

                noticer.HideNotice(Notice.GuideWord);//������ ���ִ� �˸� ������

                InitializeLimitTime();//���� �ð� �ʱ�ȭ
                noticer.ShowNotice(Notice.GuideState);//GuideState�˸� Ȱ��ȭ

                rightPanel.gameObject.SetActive(false);//rightPanelȰ��ȭ(�÷��̾� ���, ä��â)
                Camera.main.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);//ī�޶� ���� ����
                break;

            case MainStageState.GuidingOrder:

                noticer.HideNotice(Notice.GuideState);//���� �˸� ������

                noticeTime = gameManager.noticeTime;//�˸� �ð� �ʱ�ȭ
                noticer.ShowNotice(Notice.GuideOrder);//���� �˸� Ȱ��ȭ
                break;

            case MainStageState.NextStage:
                InventoryManager.instance.ShowItems();//������ - ������ �������� Ȯ��
                SceneManager.LoadScene("Problem");//���� �� Ȱ��ȭ
                break;
        }
    }



    //���� ���� ��ư Ŭ�� �̺�Ʈ �Լ�
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

    //������ ���� UI�� �ʱ�ȭ�Ѵ�.
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

    //���� �ð� �ʱ�ȭ
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
