using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

/*
 GameManager 클래스
- 게임 전체적인 규칙에 대한 정보를 관리한다.

- GameManager객체에 부착

주요기능
- 제한시간설정
- 알림시간설정
- 플레이어의 게임순서 세팅
- 플레이어에게 제시어를 부여한다.
- 참가자들의 게임 순서를 랜덤하게 지정한다.
- 전체 플레이어의 닉네임, 게임 플레이 순서, 점수, 캐릭터 이미지, 게임 참가 순서를 저장하고 관리한다.
- 게임 내에서 중요 의사 결정을 한다.
    - 참가자들이 채팅으로 보내는 말을 확인하여 정답여부를 판정한다.
- 참가자들의 점수를 정렬하여 순위를 매긴다.
- 게임을 재시작 할 때 게임 정보를 초기화한다.

 PlayerInfo 클래스
- 참가자들의 게임 정보를 관리할 클래스이다.
 RandomWordPool 클래스
- 사용할 제시어들을 저장하고, 랜덤하게 선택하는 클래스
 */
public class PlayerInfo
{
    public string name;//Player가 게임에 참가할 때 입력하는 Nick Name
    public int score;//Player의 점수
    public int order;//Play 순서를 저장할 변수 -> 몇번째로 문제를 출제할지 결정한다.

    public Sprite sprite;//플레이어 캐릭터 사진
    
    public bool isHost;//플레이어가 방장(호스트)인지에 대한 여부
    public int enter;//입장 순서를 저장할 변수 -> 나중에 Player를 표시할 때

    public string proposedWord;//제시어

    public PlayerInfo(string name, int enter)
    {
        this.name = name;
        this.enter = enter;
        this.sprite = Resources.Load<Sprite>(GameManager.instance.playerImagePath + "Player" + (enter + 1));//프로필 사진 설정
        this.score = 0;
        if (enter == 0) this.isHost = true;
        else this.isHost = false;
    }

    public PlayerInfo(string name, int enter, int order, string word)
    {
        this.name = name;//이름 저장
        this.score = 0;//점수 초기화
        
        this.isHost = false;
        if (enter == 0)
        {
            this.isHost = true;
        }
        this.enter = enter;
        this.order = order;//Play 순서 저장

        this.sprite = Resources.Load<Sprite>(GameManager.instance.playerImagePath + "Player" + (enter + 1));
        this.proposedWord = word;
    }


    //제시어와 그에 해당하는 이미지를 세팅한다.
    public void SetProposed(string word)
    {
        this.proposedWord = string.Copy(word);
    }
}

/*
 카테고리 -> 제시어의 주제
제시어는 카테고리에 따라 분류된다.
 */

public enum Category
{
    Food = 0,
    Fruit = 1,
    Animal = 2,
    Thing = 3
}

/*
게임에 사용될 제시어들을 관리
- 랜덤하게 설정
 */
public class RandomWordPool
{
    private static string[] food = {"짜장면","치킨","피자","떡볶이","파전","산적","족발","샐러드","유부초밥","김밥","라면"
                               ,"사탕","햄버거","계란후라이","파스타","초밥","삼겹살","토스트","만두","핫도그","김치"
                               , "와플","마카롱","케이크","빙수","아이스크림","타코야끼"};
    private static string[] fruit = {"사과","복숭아","딸기","배","수박","참외","바나나","망고","용과","귤","포도","파인애플"
                              ,"블루베리","체리","레몬","키위","오렌지","자몽","망고스틴","한라봉","무화과","두리안"};
    private static string[] animal = {"코끼리","기린","개","고양이","사자","호랑이","양","고릴라","원숭이","두더지","새","펭귄"
                               ,"여우","늑대","말","토끼","고래","닭","문어","타조","얼룩말","해마","쥐","소","곰","뱀"
                                , "돼지","사슴","고슴도치","부엉이"};
    private static string[] someth = {"노트북","연필","스마트폰","의자","바지","안경","액자","이어폰","세탁기","냉장고","휴지통"
                               ,"책","어댑터","창문","손톱깎이","휴지","샴푸","옷걸이","건전지","운동화","변기","욕조","마스크"
                               ,"귀걸이","디퓨저","칫솔","소파","침대","거울","가방","마우스","립밤","향수","청소기","그릇","포크"
                               ,"숟가락","지갑","선풍기"};

    private static bool[] foodCheck;
    private static bool[] fruitCheck;
    private static bool[] animalCheck;
    private static bool[] somethCheck;

    private int wordNum;//필요한 단어의 개수
    private string[] selectedWords;//선택된 단어

    public RandomWordPool()
    {
        wordNum = GameManager.instance.maxPlayerNum;//플레이어 수만큼 단어가 필요하다.

        foodCheck = new bool[food.Length];
        fruitCheck = new bool[fruit.Length];
        animalCheck = new bool[animal.Length];
        somethCheck = new bool[someth.Length];

        for(int i = 0; i < food.Length; i++) foodCheck[i] = false;
        for (int i = 0; i < fruit.Length; i++) fruitCheck[i] = false;
        for (int i = 0; i < animal.Length; i++) animalCheck[i] = false;
        for (int i = 0; i < someth.Length; i++) somethCheck[i] = false;
    }

    //랜덤하게 단어들을 선택하는 함수
    public void SelectWords(ref string[] words)
    {
        System.Random random = new System.Random();
        Debug.Log("word num : " + wordNum);

        for(int i = 0; i < wordNum; i++)
        {
            Category category = (Category)random.Next(0, 3);//카테고리 선정
            int idx = 0;
            switch (category)
            {
                case Category.Food:
                    while (foodCheck[idx = Random.Range(0, food.Length)]) ;//랜덤하게 설정
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
    public static GameManager instance;//전역 변수로 자기 자신을 참조하기 위한 변수
    //다른 씬에서도 전역적으로 GameManager를 참조할 수 있다.
    //자기 자신을 참조하는 방식을 싱글턴 패턴이라 한다. 프로그램 내에 유일하게 하나의 객체만 존재할 수 있다.

    public float itemLimitTime = 3f;//아이템 수집을 제한할 시간
    public float limitTime = 2f;//문제 출제/맞추기 시간을 제한할 시간
    public float noticeTime = 5f;//안내문구가 출력될 시간
    public int maxItemNum = 10;//획득 가능한 아이템의 최대 가지수
    public int maxPlayerNum = 4;//최대 Player 수
    public int totalPlayerNum = 0;//현재 Player 수

    public string playerImagePath = "Images/Players/";//플레이어 이미지가 있는 경로
    public string itemImagePath = "Images/Items/";//아이템 오브젝트의 이미지가 저장되어 있는 경로
    public string paletteItemPath = "Prefabs/Item UI/";

    public float skillPunchCoolTime = 10f;
    public float skillCrushCoolTime = 6f;

    //지금까지 몇 문제를 풀었는지 카운팅하는 변수, 몇 번째 순서의 플레이어가 출제를 해야되는지 알 수 있다.
    public int problemCount = 0;//0 : 시작상태, 첫번째 문제,
                                 //1 : 두번째 문제,
                                 //2 : 세전째 문제,
                                 //3 : 네번째 문제,
                                 //4 : 문제 출제 끝

    public List<PlayerInfo> players;//게임순서(int) -> Player에 대한 정보(PlayerInfo)
    private int[] randomOrder;//랜덤한 순서가 저장되어 있는 변수
    private bool[] usedOrder;//랜덤하게 순서를 정하기 위한 변수, 지정된 순서는 true로 표시된다.

    public PlayerInfo localPlayer;//Local Player를 저장하기 위한 변수
    private int curSolverOrder = -1;//정답자의 순서
    private int curDrawerOrder = -1;//출제자의 순서
    private bool alreadyCheck = false;//정답이 맞았는지 확인하는 변수
    private PhotonView pv;

    RandomWordPool randomWordPool;
    string[] randomWords;

    public MainStageManager mainStageManager;
    public StageManager stageManager;

    void Awake()
    {
        if (instance == null)//맨 처음 객체가 생성될 때, 자기 자신을 참조해 어느 씬에서든지 사용할 수 있도록 한다.
        {
            instance = this;//자기 자신 참조
            pv = GetComponent<PhotonView>();

            if (PhotonNetwork.IsMasterClient)//호스트에서만 순서와 제시어를 세팅할 수 있도록한다.
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
                    while (usedOrder[order = Random.Range(0, maxPlayerNum)]) ;//랜덤하게 순서를 결정해준다.
                    usedOrder[order] = true;//사용된 순서는 true로 변경

                    randomOrder[i] = order;
                    Debug.Log("random order : " + order);
                }

                for (int i = 0; i < randomWords.Length; i++) Debug.Log("selected word " + randomWords[i]);
            }


            //객체 생성
            //players = new Dictionary<int, PlayerInfo>();
            players = new List<PlayerInfo>();

            Debug.Log("game manager awake");

            photonView.RPC("AddPlayer", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName);
            
            Debug.Log("Done apply all players");

            DontDestroyOnLoad(instance);//다른 씬에서도 쓸 수 있도록

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

    //플레이어가 게임에 참가
    //호스트에서만 실행된다.
    [PunRPC]
    public void AddPlayer(string name)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("add player");

        players.Add(new PlayerInfo(name, totalPlayerNum, randomOrder[totalPlayerNum], randomWords[totalPlayerNum]));//players에 추가한다. 먼저 들어온 순서대로 저장
        if (name.Equals(PhotonNetwork.LocalPlayer.NickName))
        {
            localPlayer = players[totalPlayerNum];//현재 추가되는 플레이어가 localPlayer가 된다.
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

    //호스트 -> 다른 클라이언트로 업데이트하는 함수
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
                localPlayer = players[totalPlayerNum];//현재 추가되는 플레이어가 localPlayer가 된다.
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

    //지금까지 몇 문제를 풀었는지 기록한다.
    //문제 푼 것을 카운팅하는 것이 현재 순서를 저장하는 것이므로 Dictionary를 통해 해당 참가자에 바로 접근할 수 있다.
    public void CountProblem()
    {
        problemCount += 1;
        Debug.Log("problem count : " + problemCount);

        if(PhotonNetwork.IsMasterClient)
            alreadyCheck = false;
    }

    //현재 자신이 게임 순서인지 알려주는 함수
    //problemCount(현재 게임 순서)와 본인이 지정받은 순서를 비교하여 본인이 플레이할 순서인지 아닌지 판단한다.
    public bool IsMyTurn()
    {
        Debug.Log("is my turn? : " + problemCount + " " + localPlayer.order);
        if(problemCount == localPlayer.order)//게임 순서와 본인(localPlayer)의 정보를 비교한다.
        {
            return true;
        }
        return false;
    }

    //모든 참가자가 문제를 출제했는지 판단한다.
    //problemCount와 참가자 수(또는 전체 문제 수)와 비교하면 알 수 있다.
    public bool IsGameFinished()
    {
        if(problemCount == totalPlayerNum)//problemCount가 4(player수)가 되는 순간 종료된다.
        {
            return true;
        }
        return false;
    }

    //Play 순서대로 참가자의 정보를 출력한다.
    //정보가 잘 저장되었는지 디버깅용
    public void ShowPlayerInfo()
    {
        foreach (var player in players)
        {
            string log = "order : " + player.order + " name : " + player.name;
            if (player.isHost) log += " Host";
            Debug.Log(log);
        }
    }

    //점수를 기준으로 Player들을 정렬하여 List형태로 반환한다.
    //점수가 높은 순으로 내림차순 정렬된다.
    public List<PlayerInfo> SortByScore()//점수를 정렬
    {
        return players.OrderByDescending(i => i.score).ToList();
    }

    //점수를 정렬하여 콘솔에 출력해준다. 디버깅용
    public void ShowSortedScore()//점수를 정렬해서 보여준다.
    {
        foreach(var player in SortByScore())
        {
            Debug.Log(player.name + " score : " + player.score);
        }
    }

    //입력받은 문자열과 현재 출제 중인 제시어가 동일한지 판단한다.
    //만약 동일할 경우 출제자와 정답자의 점수를 각각 1점씩 올려준다.
    //정답을 맞춘 경우
    //  - 정답자와 출제자의 점수를 각각 1점씩 올려준다.
    //  - true 반환
    //정답을 못 맞춘 경우 - false 반환
    public bool CheckAnswer(string answer, PlayerInfo info)
    {
        int order = info.order;//정답자의 순서(ID)
        if (problemCount == totalPlayerNum) return false;

        answer = answer.ToLower();//입력받은 값을 소문자로 변환
        Debug.Log("input : " + answer);
        string curProposedWord = players[problemCount].proposedWord.ToLower();
        
        if (answer.Equals(curProposedWord))//정답이 맞은 경우
        {
            Debug.Log("(GameManager)correct!");

            photonView.RPC("ApplyAlreadyCheck", RpcTarget.MasterClient, order);//호스트에 검사를 요청함.

            return true;
        }
        return false;
    }

    [PunRPC]
    public void UpdateScoreAndPeople(int solverOrder, int drawerOrder)
    {
        players[solverOrder].score += 1;//정답자의 점수를 올려준다.
        players[drawerOrder].score += 1;//현재 출제자의 점수를 올려준다.
        this.curSolverOrder = solverOrder;//현재 정답자를 기록 -> 나중에 정답자와 출제자를 공개할 때 필요하다.
        this.curDrawerOrder = drawerOrder;//현재 출제자를 기록

        stageManager.ChangeStateCorrectAnswer();
    }

    [PunRPC]
    public void ApplyAlreadyCheck(int order)//정답 체크를 했는지 안했는지 검사함.
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (alreadyCheck == false)//아직 검사를 안한 경우
        {
            alreadyCheck = true;

            //호스트 정보 업데이트
            players[order].score += 1;//정답자의 점수를 올려준다.
            players[problemCount].score += 1;//현재 출제자의 점수를 올려준다.
            this.curSolverOrder = order;//현재 정답자를 기록 -> 나중에 정답자와 출제자를 공개할 때 필요하다.
            this.curDrawerOrder = problemCount;//현재 출제자를 기록

            stageManager.ChangeStateCorrectAnswer();

            //클라이언트 업데이트
            photonView.RPC("UpdateScoreAndPeople", RpcTarget.Others, curSolverOrder, curDrawerOrder);//GameManager내의 점수를 업데이트 한다.
        }
        //이미 검사를 한 경우는 업데이트 할 필요가 없음.
    }

    //현재 제시어가 무엇인지 알려준다.
    public string GetCurProposedWord()
    {
        return players[problemCount].proposedWord;
    }

    //현재 문제의 정답자를 알려준다. 문제 수를 카운팅하기 전에 접근해야된다.
    public PlayerInfo GetCurSolver()
    {
        return players[curSolverOrder];
    }

    //현재 문제의 출제자를 알려준다.
    public PlayerInfo GetCurDrawer()
    {
        return players[curDrawerOrder];
    }
}
