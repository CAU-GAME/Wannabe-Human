using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

/*
 GameManager Ŭ����
- ���� ��ü���� ��Ģ�� ���� ������ �����Ѵ�.

- GameManager��ü�� ����

�ֿ���
- ���ѽð�����
- �˸��ð�����
- �÷��̾��� ���Ӽ��� ����
- �÷��̾�� ���þ �ο��Ѵ�.
- �����ڵ��� ���� ������ �����ϰ� �����Ѵ�.
- ��ü �÷��̾��� �г���, ���� �÷��� ����, ����, ĳ���� �̹���, ���� ���� ������ �����ϰ� �����Ѵ�.
- ���� ������ �߿� �ǻ� ������ �Ѵ�.
    - �����ڵ��� ä������ ������ ���� Ȯ���Ͽ� ���俩�θ� �����Ѵ�.
- �����ڵ��� ������ �����Ͽ� ������ �ű��.
- ������ ����� �� �� ���� ������ �ʱ�ȭ�Ѵ�.

 PlayerInfo Ŭ����
- �����ڵ��� ���� ������ ������ Ŭ�����̴�.
 RandomWordPool Ŭ����
- ����� ���þ���� �����ϰ�, �����ϰ� �����ϴ� Ŭ����
 */
public class PlayerInfo
{
    public string name;//Player�� ���ӿ� ������ �� �Է��ϴ� Nick Name
    public int score;//Player�� ����
    public int order;//Play ������ ������ ���� -> ���°�� ������ �������� �����Ѵ�.

    public Sprite sprite;//�÷��̾� ĳ���� ����
    
    public bool isHost;//�÷��̾ ����(ȣ��Ʈ)������ ���� ����
    public int enter;//���� ������ ������ ���� -> ���߿� Player�� ǥ���� ��

    public string proposedWord;//���þ�

    public PlayerInfo(string name, int enter)
    {
        this.name = name;
        this.enter = enter;
        this.sprite = Resources.Load<Sprite>(GameManager.instance.playerImagePath + "Player" + (enter + 1));//������ ���� ����
        this.score = 0;
        if (enter == 0) this.isHost = true;
        else this.isHost = false;
    }

    public PlayerInfo(string name, int enter, int order, string word)
    {
        this.name = name;//�̸� ����
        this.score = 0;//���� �ʱ�ȭ
        
        this.isHost = false;
        if (enter == 0)
        {
            this.isHost = true;
        }
        this.enter = enter;
        this.order = order;//Play ���� ����

        this.sprite = Resources.Load<Sprite>(GameManager.instance.playerImagePath + "Player" + (enter + 1));
        this.proposedWord = word;
    }


    //���þ�� �׿� �ش��ϴ� �̹����� �����Ѵ�.
    public void SetProposed(string word)
    {
        this.proposedWord = string.Copy(word);
    }
}

/*
 ī�װ� -> ���þ��� ����
���þ�� ī�װ��� ���� �з��ȴ�.
 */

public enum Category
{
    Food = 0,
    Fruit = 1,
    Animal = 2,
    Thing = 3
}

/*
���ӿ� ���� ���þ���� ����
- �����ϰ� ����
 */
public class RandomWordPool
{
    private static string[] food = {"¥���","ġŲ","����","������","����","����","����","������","�����ʹ�","���","���"
                               ,"����","�ܹ���","����Ķ���","�Ľ�Ÿ","�ʹ�","����","�佺Ʈ","����","�ֵ���","��ġ"
                               , "����","��ī��","����ũ","����","���̽�ũ��","Ÿ�ھ߳�"};
    private static string[] fruit = {"���","������","����","��","����","����","�ٳ���","����","���","��","����","���ξ���"
                              ,"��纣��","ü��","����","Ű��","������","�ڸ�","����ƾ","�Ѷ��","��ȭ��","�θ���"};
    private static string[] animal = {"�ڳ���","�⸰","��","�����","����","ȣ����","��","����","������","�δ���","��","���"
                               ,"����","����","��","�䳢","��","��","����","Ÿ��","��踻","�ظ�","��","��","��","��"
                                , "����","�罿","����ġ","�ξ���"};
    private static string[] someth = {"��Ʈ��","����","����Ʈ��","����","����","�Ȱ�","����","�̾���","��Ź��","�����","������"
                               ,"å","�����","â��","�������","����","��Ǫ","�ʰ���","������","�ȭ","����","����","����ũ"
                               ,"�Ͱ���","��ǻ��","ĩ��","����","ħ��","�ſ�","����","���콺","����","���","û�ұ�","�׸�","��ũ"
                               ,"������","����","��ǳ��"};

    private static bool[] foodCheck;
    private static bool[] fruitCheck;
    private static bool[] animalCheck;
    private static bool[] somethCheck;

    private int wordNum;//�ʿ��� �ܾ��� ����
    private string[] selectedWords;//���õ� �ܾ�

    public RandomWordPool()
    {
        wordNum = GameManager.instance.maxPlayerNum;//�÷��̾� ����ŭ �ܾ �ʿ��ϴ�.

        foodCheck = new bool[food.Length];
        fruitCheck = new bool[fruit.Length];
        animalCheck = new bool[animal.Length];
        somethCheck = new bool[someth.Length];

        for(int i = 0; i < food.Length; i++) foodCheck[i] = false;
        for (int i = 0; i < fruit.Length; i++) fruitCheck[i] = false;
        for (int i = 0; i < animal.Length; i++) animalCheck[i] = false;
        for (int i = 0; i < someth.Length; i++) somethCheck[i] = false;
    }

    //�����ϰ� �ܾ���� �����ϴ� �Լ�
    public void SelectWords(ref string[] words)
    {
        System.Random random = new System.Random();
        Debug.Log("word num : " + wordNum);

        for(int i = 0; i < wordNum; i++)
        {
            Category category = (Category)random.Next(0, 3);//ī�װ� ����
            int idx = 0;
            switch (category)
            {
                case Category.Food:
                    while (foodCheck[idx = Random.Range(0, food.Length)]) ;//�����ϰ� ����
                    foodCheck[idx] = true;
                    words[i] = food[idx];
                    break;
                case Category.Fruit:
                    while (fruitCheck[idx = Random.Range(0, fruit.Length)]) ;
                    fruitCheck[idx] = true;
                    words[i] = fruit[idx];
                    break;
                case Category.Animal:
                    while (animalCheck[idx = Random.Range(0, animal.Length)]) ;
                    animalCheck[idx] = true;
                    words[i] = animal[idx];
                    break;
                case Category.Thing:
                    while (somethCheck[idx = Random.Range(0, someth.Length)]) ;
                    somethCheck[idx] = true;
                    words[i] = someth[idx];
                    break;
            }
            Debug.Log("selected words(random word pool) " + words[i] + " idx " + idx);
        }
    }
}

public class GameManager : MonoBehaviourPun
{
    public static GameManager instance;//���� ������ �ڱ� �ڽ��� �����ϱ� ���� ����
    //�ٸ� �������� ���������� GameManager�� ������ �� �ִ�.
    //�ڱ� �ڽ��� �����ϴ� ����� �̱��� �����̶� �Ѵ�. ���α׷� ���� �����ϰ� �ϳ��� ��ü�� ������ �� �ִ�.

    public float itemLimitTime = 3f;//������ ������ ������ �ð�
    public float limitTime = 2f;//���� ����/���߱� �ð��� ������ �ð�
    public float noticeTime = 5f;//�ȳ������� ��µ� �ð�
    public int maxItemNum = 10;//ȹ�� ������ �������� �ִ� ������
    public int maxPlayerNum = 4;//�ִ� Player ��
    public int totalPlayerNum = 0;//���� Player ��

    public string playerImagePath = "Images/Players/";//�÷��̾� �̹����� �ִ� ���
    public string itemImagePath = "Images/Items/";//������ ������Ʈ�� �̹����� ����Ǿ� �ִ� ���
    public string paletteItemPath = "Prefabs/Item UI/";

    public float skillPunchCoolTime = 10f;
    public float skillCrushCoolTime = 6f;

    //���ݱ��� �� ������ Ǯ������ ī�����ϴ� ����, �� ��° ������ �÷��̾ ������ �ؾߵǴ��� �� �� �ִ�.
    public int problemCount = 0;//0 : ���ۻ���, ù��° ����,
                                 //1 : �ι�° ����,
                                 //2 : ����° ����,
                                 //3 : �׹�° ����,
                                 //4 : ���� ���� ��

    public List<PlayerInfo> players;//���Ӽ���(int) -> Player�� ���� ����(PlayerInfo)
    private int[] randomOrder;//������ ������ ����Ǿ� �ִ� ����
    private bool[] usedOrder;//�����ϰ� ������ ���ϱ� ���� ����, ������ ������ true�� ǥ�õȴ�.

    public PlayerInfo localPlayer;//Local Player�� �����ϱ� ���� ����
    private int curSolverOrder = -1;//�������� ����
    private int curDrawerOrder = -1;//�������� ����
    private bool alreadyCheck = false;//������ �¾Ҵ��� Ȯ���ϴ� ����
    private PhotonView pv;

    RandomWordPool randomWordPool;
    string[] randomWords;

    public MainStageManager mainStageManager;
    public StageManager stageManager;

    void Awake()
    {
        if (instance == null)//�� ó�� ��ü�� ������ ��, �ڱ� �ڽ��� ������ ��� ���������� ����� �� �ֵ��� �Ѵ�.
        {
            instance = this;//�ڱ� �ڽ� ����
            pv = GetComponent<PhotonView>();

            if (PhotonNetwork.IsMasterClient)//ȣ��Ʈ������ ������ ���þ ������ �� �ֵ����Ѵ�.
            {
                Debug.Log("random setting");

                randomWords = new string[maxPlayerNum];
                randomWordPool = new RandomWordPool();
                randomWordPool.SelectWords(ref randomWords);

                usedOrder = new bool[maxPlayerNum];
                randomOrder = new int[maxPlayerNum];
                for(int i = 0; i < maxPlayerNum; i++)
                {
                    int order;
                    while (usedOrder[order = Random.Range(0, maxPlayerNum)]) ;//�����ϰ� ������ �������ش�.
                    usedOrder[order] = true;//���� ������ true�� ����

                    randomOrder[i] = order;
                    Debug.Log("random order : " + order);
                }

                for (int i = 0; i < randomWords.Length; i++) Debug.Log("selected word " + randomWords[i]);
            }


            //��ü ����
            //players = new Dictionary<int, PlayerInfo>();
            players = new List<PlayerInfo>();

            Debug.Log("game manager awake");

            photonView.RPC("AddPlayer", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName);
            
            Debug.Log("Done apply all players");

            DontDestroyOnLoad(instance);//�ٸ� �������� �� �� �ֵ���

            int num = PhotonNetwork.PlayerList.Count<Player>();
            PhotonNetwork.Instantiate("Player" + num, new Vector3(2.02f, 0.15f, 10f * Random.Range(-30, 30) / 30f), Quaternion.identity);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DestroyToAll(int _idx)
    {
        pv.RPC("LetDestroyRPC", RpcTarget.AllBuffered, _idx);
    }

    [PunRPC]
    public void LetDestroyRPC(int idx)
    {
        Destroy(GameObject.Find("object").transform.GetChild(idx).gameObject);
    }

    //�÷��̾ ���ӿ� ����
    //ȣ��Ʈ������ ����ȴ�.
    [PunRPC]
    public void AddPlayer(string name)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("add player");

        players.Add(new PlayerInfo(name, totalPlayerNum, randomOrder[totalPlayerNum], randomWords[totalPlayerNum]));//players�� �߰��Ѵ�. ���� ���� ������� ����
        if (name.Equals(PhotonNetwork.LocalPlayer.NickName))
        {
            localPlayer = players[totalPlayerNum];//���� �߰��Ǵ� �÷��̾ localPlayer�� �ȴ�.
        }

        for (int i = 0; i < players.Count; i++)
        {
            PlayerInfo player = players[i];
            photonView.RPC("UpdatePlayerInfo", RpcTarget.Others, player.name, player.enter, player.order, player.proposedWord);
            if (mainStageManager != null)
            {
                mainStageManager.ApplySetPlayerInfo(player.name, player.enter, player.score);
            }
        }
        totalPlayerNum += 1;
    }

    //ȣ��Ʈ -> �ٸ� Ŭ���̾�Ʈ�� ������Ʈ�ϴ� �Լ�
    [PunRPC]
    public void UpdatePlayerInfo(string name, int enter, int order, string word)
    {
        bool hasName = false;
        for(int i = 0; i < players.Count; i++)
        {
            if (name.Equals(players[i].name))
            {
                hasName = true;
                break;
            }
        }
        if(hasName == false)
        {
            players.Add(new PlayerInfo(name, enter, order, word));
            if (name.Equals(PhotonNetwork.LocalPlayer.NickName))
            {
                localPlayer = players[totalPlayerNum];//���� �߰��Ǵ� �÷��̾ localPlayer�� �ȴ�.
            }
            totalPlayerNum += 1;
        }
    }


    public void SetGamePlayingOrder()
    {
        Debug.Log("Set GamePlaying Order");

        players = players.OrderBy(x => x.order).ToList<PlayerInfo>();
        for(int i = 0; i < players.Count; i++)
        {
            players[i].order = i;
            Debug.Log("player order : " + players[i].name + " " + players[i].order);
            if (players[i].name == localPlayer.name) localPlayer.order = i;
        }
    }

    //���ݱ��� �� ������ Ǯ������ ����Ѵ�.
    //���� Ǭ ���� ī�����ϴ� ���� ���� ������ �����ϴ� ���̹Ƿ� Dictionary�� ���� �ش� �����ڿ� �ٷ� ������ �� �ִ�.
    public void CountProblem()
    {
        problemCount += 1;
        Debug.Log("problem count : " + problemCount);

        if(PhotonNetwork.IsMasterClient)
            alreadyCheck = false;
    }

    //���� �ڽ��� ���� �������� �˷��ִ� �Լ�
    //problemCount(���� ���� ����)�� ������ �������� ������ ���Ͽ� ������ �÷����� �������� �ƴ��� �Ǵ��Ѵ�.
    public bool IsMyTurn()
    {
        Debug.Log("is my turn? : " + problemCount + " " + localPlayer.order);
        if(problemCount == localPlayer.order)//���� ������ ����(localPlayer)�� ������ ���Ѵ�.
        {
            return true;
        }
        return false;
    }

    //��� �����ڰ� ������ �����ߴ��� �Ǵ��Ѵ�.
    //problemCount�� ������ ��(�Ǵ� ��ü ���� ��)�� ���ϸ� �� �� �ִ�.
    public bool IsGameFinished()
    {
        if(problemCount == totalPlayerNum)//problemCount�� 4(player��)�� �Ǵ� ���� ����ȴ�.
        {
            return true;
        }
        return false;
    }

    //Play ������� �������� ������ ����Ѵ�.
    //������ �� ����Ǿ����� ������
    public void ShowPlayerInfo()
    {
        foreach (var player in players)
        {
            string log = "order : " + player.order + " name : " + player.name;
            if (player.isHost) log += " Host";
            Debug.Log(log);
        }
    }

    //������ �������� Player���� �����Ͽ� List���·� ��ȯ�Ѵ�.
    //������ ���� ������ �������� ���ĵȴ�.
    public List<PlayerInfo> SortByScore()//������ ����
    {
        return players.OrderByDescending(i => i.score).ToList();
    }

    //������ �����Ͽ� �ֿܼ� ������ش�. ������
    public void ShowSortedScore()//������ �����ؼ� �����ش�.
    {
        foreach(var player in SortByScore())
        {
            Debug.Log(player.name + " score : " + player.score);
        }
    }

    //�Է¹��� ���ڿ��� ���� ���� ���� ���þ �������� �Ǵ��Ѵ�.
    //���� ������ ��� �����ڿ� �������� ������ ���� 1���� �÷��ش�.
    //������ ���� ���
    //  - �����ڿ� �������� ������ ���� 1���� �÷��ش�.
    //  - true ��ȯ
    //������ �� ���� ��� - false ��ȯ
    public bool CheckAnswer(string answer, PlayerInfo info)
    {
        int order = info.order;//�������� ����(ID)
        if (problemCount == totalPlayerNum) return false;

        answer = answer.ToLower();//�Է¹��� ���� �ҹ��ڷ� ��ȯ
        Debug.Log("input : " + answer);
        string curProposedWord = players[problemCount].proposedWord.ToLower();
        
        if (answer.Equals(curProposedWord))//������ ���� ���
        {
            Debug.Log("(GameManager)correct!");

            photonView.RPC("ApplyAlreadyCheck", RpcTarget.MasterClient, order);//ȣ��Ʈ�� �˻縦 ��û��.

            return true;
        }
        return false;
    }

    [PunRPC]
    public void UpdateScoreAndPeople(int solverOrder, int drawerOrder)
    {
        players[solverOrder].score += 1;//�������� ������ �÷��ش�.
        players[drawerOrder].score += 1;//���� �������� ������ �÷��ش�.
        this.curSolverOrder = solverOrder;//���� �����ڸ� ��� -> ���߿� �����ڿ� �����ڸ� ������ �� �ʿ��ϴ�.
        this.curDrawerOrder = drawerOrder;//���� �����ڸ� ���

        stageManager.ChangeStateCorrectAnswer();
    }

    [PunRPC]
    public void ApplyAlreadyCheck(int order)//���� üũ�� �ߴ��� ���ߴ��� �˻���.
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (alreadyCheck == false)//���� �˻縦 ���� ���
        {
            alreadyCheck = true;

            //ȣ��Ʈ ���� ������Ʈ
            players[order].score += 1;//�������� ������ �÷��ش�.
            players[problemCount].score += 1;//���� �������� ������ �÷��ش�.
            this.curSolverOrder = order;//���� �����ڸ� ��� -> ���߿� �����ڿ� �����ڸ� ������ �� �ʿ��ϴ�.
            this.curDrawerOrder = problemCount;//���� �����ڸ� ���

            stageManager.ChangeStateCorrectAnswer();

            //Ŭ���̾�Ʈ ������Ʈ
            photonView.RPC("UpdateScoreAndPeople", RpcTarget.Others, curSolverOrder, curDrawerOrder);//GameManager���� ������ ������Ʈ �Ѵ�.
        }
        //�̹� �˻縦 �� ���� ������Ʈ �� �ʿ䰡 ����.
    }

    //���� ���þ �������� �˷��ش�.
    public string GetCurProposedWord()
    {
        return players[problemCount].proposedWord;
    }

    //���� ������ �����ڸ� �˷��ش�. ���� ���� ī�����ϱ� ���� �����ؾߵȴ�.
    public PlayerInfo GetCurSolver()
    {
        return players[curSolverOrder];
    }

    //���� ������ �����ڸ� �˷��ش�.
    public PlayerInfo GetCurDrawer()
    {
        return players[curDrawerOrder];
    }
}
