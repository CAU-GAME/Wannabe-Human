using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

/*
 StageManager Ŭ����
- Problem Stage�� ���������� ���������� �����ϴ� Ŭ�����̴�. ���� ����- Quiz Time
- GameManager�κ��� �߿� ������ �����Ͽ� �÷��̾��� ���������� �����Ѵ�.
- Problem Stage�� ũ�� �����ڿ� ������ Ǫ�� ������� ������.
- �����ڴ� Main Stage���� ������ �������� �̿��� ������ ���þ ǥ���Ѵ�.
- �����ڴ� �����ڰ� ǥ���ϴ� �׸��� ���� ���þ ������ ������ �����.

- Stage Manager�� ����

- Problem Stage�� ������ ���°� ũ�� 5������ �ִ�.
    - Main Stage���� Problem Stage�� �Ѿ�� ��(���ۻ���) -> Start
    - ������ �������� �̿��� ������ �����ϰ� �ִ� ���� -> Drawing
    - �ٸ� ����� ���þ ���߰� �ִ� ���� -> Solving
    - ��������/�߸��� ������ ������ �����ϰ� ����� ���� ������ �ο��ϰ� ���� ����� �˷��ִ� ���� -> Waiting
    - ��� �������� ���� ������ ���� ���� -> End
- �������� ǥ���غ���
Start -> Drawing/Solving -> Waiting -> ... -> Drawing/Solving -> Waiting -> End ���� �� �� �ִ�.
 - �� ������ ��ȭ�� ���� ������ ���¸� �����ϰ� �ʱ�ȭ ����� �Ѵ�.

�ֿ���
    - ���ѽð��� �����Ͽ� ����Ǯ�� �ð��� �����Ѵ�.
    - �����ڿ��� ���ѽð��� ������� �Ѵ�.
    - ���� Ǯ�� ����� ���� ����ڿ��� ������ �ȳ������� ǥ������� �Ѵ�.
    - �ȳ������� ������ �ð��� ���� �������� �Ѵ�.
    - �÷��̾ ���� ���� ���, �÷��̾ �������� ������ �׸��� �׸� �� �־�� �Ѵ�.
    - ������ �������� �ٽ� ����Ͽ� ĵ�������� ������ �� �־�� �Ѵ�.(ĵ���� �Ǵ� �ȷ�Ʈ -> �����ڰ� �������� �̿��� �׸��� �׸��� ����)
    - �������� ���� �������� ���� ������(������ ��� ���� ������, Inventory Items)�� ������ ������(�׷��� ������, Drawn Items)���� ������.
    - �÷��̾�� ������ ���� ���þ ���� ��ܿ��� Ȯ���� �� �־�� �Ѵ�.
    - ä��â�� �̿��� ����ڵ�� ä���� �� �� �־�� �Ѵ�.
    - ä��â���� ������� �Է��� �����Ͽ� ���� ���þ�� ���ؾ� �Ѵ�.
    - �����ڴ� ���� ���� ���� ������ �� �־�� �Ѵ�.
    - �� �÷��̾��� ĳ���� �̹���, �̸�, ������ Ȯ���� �� �־�� �Ѵ�.
    - ������ ���� �� �ٽ� ������ �� �� �ֵ��� �ؾ� �Ѵ�.
 */
public enum PlayState
{
    Start,//���� ó�� ������ ��
    Drawing,//�������� �̿��� ���þ ǥ���ؾ� �Ѵ�.
    Solving,//�ٸ� ����� �������� ������ ����� �Ѵ�.
    Waiting,//���� �ȳ� ��Ʈ�� ���� ��
    End//������ ����� ����
}

public class StageManager : MonoBehaviourPun, IPunObservable
{
    private GameManager gameManager;//���������� ������ GameManager

    //�����Ϳ��� ���� ����
    public Text timer;//���ѽð��� ǥ������ Timer
    public Button giveUp;//���� ���� ��ư
    public InputManager answer;//ä���Է�â �� �����Է� â
    public NoticeManager noticer;//�ȳ� ���� ������
    public DrawnItemsManager drawnItems;//���� ������ â, �׷��� �������� ǥ�õȴ�.
    public InventoryItemsManager inventoryItems;//���� ������ â, ���� �׷����� ���� �������� ǥ�õȴ�.
    public PlayerInfoManager playerInfos;//�÷��̾� ������ �����ϰ� UI�� �����ش�.
    
    //���� �÷��̾ ���� ���þ�� �̹���, ���� ��ܿ� ǥ�õȴ�.
    public Text proposedWord;//���þ�

    //GameManager�κ��� ������ �����´�.
    public float limitTime;//���ѽð��� ����Ǵ� ����, ���� : ��
    public float noticeTime;//�ȳ������� ǥ�õǴ� �ð��� ������ ����, ���� : ��

    private PlayState state = PlayState.Start;//���� ������ ����������, ���ߴ� ������, ������ ����ƴ��� ����
    private Notice notice = Notice.Wrong;//� �˸��� ������� �ϴ� �� ����

    private int enterNum = 0;

    void Start()
    {
        gameManager = GameManager.instance;//���� ������ �����ϱ� ���� GameManager
        gameManager.stageManager = gameObject.GetComponent<StageManager>();

        photonView.RPC("CountEnterNum", RpcTarget.MasterClient);

        InitializeLimitTime();//���ѽð� ����
        InitializeNoticeTime();//�˸��ð� ����
        timer.text = CalTime(limitTime);//Ÿ�̸ӿ� ���ѽð� ǥ��

        giveUp.onClick.AddListener(OnClickGiveUp);//���� ���� ��ư Ŭ���� �̺�Ʈ ���
        //���� ��ư Ŭ���� �̺�Ʈ ���
        //answer.button.onClick.AddListener(() =>
        //{
        //    CheckAnswerDelay(answer.input);
        //});
        //Click�̺�Ʈ�� ��ư�� ������ ������ ����ȴ�.
        //Pointer Down�� ��ư�� ������ �ٷ� ����ȴ�. -> InputManager���� pointer down�̺�Ʈ�� ó���ؼ� ���⵵ pointer down�̺�Ʈ�� �ٲ�µ�
        //ȭ�� ��ȯ�� �ʹ� ���� �Ǵ� ���� ������ �ִ� �� ����.
        //�׷��� ���� �˻��ϴ� �Ͱ� ä��â�� ������Ʈ �Ǵ� �κ��� ��Ʈ��ŷ�� �ʿ��� �κ��̹Ƿ�
        //��Ʈ��ŷ�� �������� �� ���� ������ �� ������� �� �� ����.
        // -> ä��â�� ������Ʈ �ƴµ�, ����üũ�� �ٸ� ������� ���� ���� ���� �� ���Ƽ�
        //   (���۹�ư�� ������������ ���� ������ �ִ� ��� -> ��ư�� Ŭ�����ڸ��� ä��â�� ������Ʈ �ƴµ�,
        //   ��ư�� ���� ����, �ٸ������ ��ư�� �� ������ �ٸ������ ���߰� �� �� ����)
        // -> ���� �̷��ٸ�, ������ üũ�ϴ� �Ͱ� ä��â ������Ʈ�� ���� �̺�Ʈ�� ó���ؾ� �� ���� ���� �� ����.
        EventTrigger eventTrigger = answer.button.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entryPointerDown);

        //����Ű Ŭ���� �̺�Ʈ ���
        answer.inputField.onEndEdit.AddListener((val) =>
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                CheckAnswerDelay(answer.input);//���⼭ val�� �־ �� �� ����.
        });
        //Input Manager�� ���������� StageManager�� ������������ ������Ѵ�. �׷��� Input Manager�� �����Լ��� Awake�� ������
        //�׷��� Input Manager�� �̺�Ʈ ����Լ��� Send�� ���� ����Ǽ� Answer.input�� ����� �޾ƿ� �� �ִ�.

        //�÷��̾��� ���þ�� �����̹��� ����
        proposedWord.text = gameManager.localPlayer.proposedWord;

        SetPlayersInfo();//���� �÷��̾���� ������ ȭ�鿡 �����ش�.
    }

    void Update()
    {
        switch (state)//���� �÷��� ���� Ȯ��
        {
            case PlayState.Start://����
                //PhotonView does not exist ���������� �߰��ߴ�.
                //�Ƹ���, ChangeState�� ������ ��, �ٸ� Ŭ���̾�Ʈ���� StageManager�� �����Ǳ� ����, RPC�� ������ ����� ������ �� ����.
                //�׷��� ��� Ŭ���̾�Ʈ�� Ȯ���ϰ� StageManager�� �������� �� ������ ����� �� �ֵ��� �ߴ�.
                //https://stackoverflow.com/questions/66952744/punrpc-not-getting-called
                if (enterNum == gameManager.totalPlayerNum)
                {
                    ChangeState();
                }
                //ChangeState();
                break;
            //�ȳ����� ��� ����
            case PlayState.Waiting:
                if (noticeTime > 0)//�ȳ����� ��� �ð��� �����ִ��� Ȯ��
                {
                    noticeTime -= Time.deltaTime;
                    //�ȳ� ǥ�ÿ� �ð��� ������Ʈ���༭ ����ڿ��� �����־�� �Ѵ�.
                    noticer.SetTime(notice, noticeTime);
                }
                else//�ȳ��� ���� ���
                {
                    ChangeState();
                }
                break;

                //���� ����/ �Ǵ� ���߱� ����
            case PlayState.Drawing:
            case PlayState.Solving:
                if (limitTime > 0)//�ð� ����
                {
                    limitTime -= Time.deltaTime;
                    timer.text = CalTime(limitTime);//Timer�� ���ѽð��� ǥ���Ѵ�.
                }
                else ChangeState();//�ð��� �ٵǸ� �ȳ������� ������ִ� ���·� ��ȯ
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

    //�ð��� �ֱ������� ����ȭ�Ѵ�.
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

    //���� �ð��� �����ϰ� �����Ͽ� �˾ƺ��� ���� ǥ���Ѵ�.
    public string CalTime(float time)
    {
        time += 1;
        int min = (int)(time / 60f);
        int seconds = (int)(time - min * 60f);
        return (int)(min / 10f) + "" + (min % 10) + ":" + (int)(seconds / 10f) + "" + (seconds % 10);
    }

    //GameManager�κ��� ���ѽð��� �޾� �ʱ�ȭ�Ѵ�.
    public void InitializeLimitTime()
    {
        limitTime = gameManager.limitTime * 60f;
    }

    //GameManager�κ��� �ȳ��ð��� �޾� �ʱ�ȭ�Ѵ�.
    public void InitializeNoticeTime()
    {
        noticeTime = gameManager.noticeTime;
    }

    //���� ���� ��ư�� Ŭ������ �� ������ �Լ�
    public void OnClickGiveUp()
    {
        Debug.Log("Give Up");
        notice = Notice.Wrong;//Ʋ���� �� �ȳ������� ���
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
    public void ApplyChangeState()//����� ����
    {
        PlayState to = DecideState(state);
        Debug.Log("to : " + to);
        state = to;//state�� �����Ѵ�.
        switch (state)
        {
            case PlayState.Waiting://Waiting���·� ���ϴ� ��� -> �ȳ��� ǥ�õǴ� ���
                InitializeLimitTime();//���� �ð� �ʱ�ȭ
                noticer.ShowNotice(notice);//�˸����� ���

                drawnItems.palette.EraseAllObject();//palette�� �����ִ� Object�� �����.

                answer.DisableInput();//ä��â �Է� ��Ȱ��ȭ

                gameManager.CountProblem();//���� �� ī��Ʈ
                break;

            case PlayState.End://���� ���ᰡ �Ǵ� ���
                noticer.HideNotice(notice);//ǥ�õǰ� �ִ� �ȳ������� ���߰�
                notice = Notice.Result;//���� ���� ��� ������ ǥ���Ѵ�.
                noticer.ShowNotice(notice);
                break;

            default://Drawing���� �Ǵ� Solving���·� ���ϴ� ���
                InitializeNoticeTime();//�˸��ð� �ʱ�ȭ
                noticer.HideNotice(notice);//�˸����� ���߱�
                notice = Notice.Wrong;//�˸��������� �ʱ�ȭ

                if (state == PlayState.Drawing)
                {
                    //������ ���� Ȱ��ȭ
                    drawnItems.EnableSelect();
                    inventoryItems.EnableSelect();

                    giveUp.gameObject.SetActive(true);//���� ���� ��ư Ȱ��ȭ
                    answer.DisableInput();//ä��â �Է� ��Ȱ��ȭ
                }
                else if (state == PlayState.Solving)
                {
                    //������ ���� ��Ȱ��ȭ
                    drawnItems.DisableSelect();
                    inventoryItems.DisableSelect();

                    giveUp.gameObject.SetActive(false);//���� ���� ��ư ��Ȱ��ȭ
                    answer.EnableInput();//ä��â �Է� Ȱ��ȭ
                }
                //����� ��
                Debug.Log("current word : " + gameManager.players[gameManager.problemCount].proposedWord);
                break;
        }
    }

    public IEnumerator Delay()
    {
        yield return null;
    }

    //ä��â���� ���� �Է°��� Ȯ���Ѵ�.
    //�Է°��� ���þ�� ������ ���, Waiting���·� �����ϰ� �˸��� �ȳ������� ����Ѵ�.
    //public void CheckAnswer(string input) yield return null;�κ� �����
    //�ڷ�ƾ�� �ٷ� ���� �Ϸ��� yield break�� �ؾ��Ѵ�
    public void CheckAnswer(string input)//������ �´��� Ȯ���Ѵ�. Player�� ������ �ٲ�� �ϹǷ� Player������ �ʿ��ϴ�.
    {
        if (input.Length == 0) return;

        if (gameManager.CheckAnswer(input, gameManager.localPlayer))//������ ���� ���
        {
            //Debug.Log("(StageManager)correct !");
        }
    }

    public void CheckAnswerDelay(string input)
    {
        Delay();
        CheckAnswer(input);
    }
    //�ڷ�ƾ�� �̿��� ����
    //����Ű�� ������ Send�� �����ϴ� ���� �������� ������ �ȴ�. �׷��� ��ư�� Ŭ������ ��
    //Attempting to select while already selecting an object������ �߻��ߴ�.
    //���� ������ inputField�� ���õ� ���¿��� Button�� �� ���õƱ� ������ �� ����. �������� ������?
    // �ΰ��� UI�� ���ÿ� ���õǸ� �ȵǴ� �� ����.
    //https://answers.unity.com/questions/1315276/event-system-setselectedgameobject-error-but-code.html
    //���⸦ �����غ��� �ι�° ��ư�� ����� �� delay�� �ָ� �ȴٰ� �ؼ� �ϴ� �׷��� ������ �ذ��ߴ�.
    //������ �־ �������࿡ ū ������ �������� Ȥ�ó� �ؼ� �ذ��ߴ�.
    //�ϴ� onEndEdit�̺�Ʈ �߻� �ÿ��� ������ �������� Button�̺�Ʈ�� ���� ������ ����ǵ��� �Ȱ��� CheckAnswerDelay �Լ��� ����ߴ�.


    //������ ��ܿ� ���� �÷��̾���� ���¸� �ʱ�ȭ�Ѵ�.
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
