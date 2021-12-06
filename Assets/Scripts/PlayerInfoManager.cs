using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 PlayerInfoManager 클래스
- 오른쪽 상단에 Player의 정보가 표시되는 UI를 관리하는 클래스

- Right Panel에 Player Infos에 부착

주요기능
- UI에 PlayerInfo를 표시한다.
- 방장에는 왕관 이미지를 표시한다.
 
 */
public class PlayerInfoManager : MonoBehaviour
{
    public GameObject[] InfoWindows;//Player의 정보(캐릭터 사진, 닉네임, 점수)를 표시할 UI
    public Sprite Crown;//방장을 표시할 왕관 이미지
    private int MaxPlayerNum = 4;//최대 참가 인원

    void Awake()
    {
        InfoWindows = new GameObject[MaxPlayerNum];
        //UI참조
        for(int i = 0; i < MaxPlayerNum; i++)
        {
            InfoWindows[i] = transform.GetChild(i).gameObject;
        }
    }

    //PlayerInfo를 받아서 UI에 Player의 정보를 초기화하는 함수
    //들어온 순서대로 왼쪽 위 -> 오른쪽 위 -> 왼쪽 아래 -> 오른쪽 아래 순으로 저장된다.
    //문제 출제 순서대로 배치하고 싶다면 idx부분을 info.order로 받으면 된다.
    public void SetPlayerInfo(PlayerInfo info)//플레이어 정보
    {
        int idx = info.enter;//출제 순서대로 위치시킬지, 아니면 들어온 순서대로 위치시킬지, order아니면 enter중 선택

        //해당하는 UI 객체 참조
        Image image = InfoWindows[idx].transform.GetChild(0).gameObject.GetComponent<Image>();//이미지 참조
        Text name = InfoWindows[idx].transform.GetChild(1).gameObject.GetComponent<Text>();//이름 참조
        Text score = InfoWindows[idx].transform.GetChild(2).gameObject.GetComponent<Text>();//점수 참조
        
        if(info.enter == 0)//첫번째로 들어온 참가자는 방장
        {
            InfoWindows[idx].transform.GetChild(3).gameObject.GetComponent<Image>().sprite = Crown;
        }

        //각각의 정보를 초기화한다.
        image.sprite = info.sprite;
        name.text = info.name;
        score.text = info.score + " 점";

        InfoWindows[idx].SetActive(true);
    }

    public void SetPlayerInfo(string playerName, int enter, int playerScore)
    {
        int idx = enter;//출제 순서대로 위치시킬지, 아니면 들어온 순서대로 위치시킬지, order아니면 enter중 선택

        //해당하는 UI 객체 참조
        Image image = InfoWindows[idx].transform.GetChild(0).gameObject.GetComponent<Image>();//이미지 참조
        Text name = InfoWindows[idx].transform.GetChild(1).gameObject.GetComponent<Text>();//이름 참조
        Text score = InfoWindows[idx].transform.GetChild(2).gameObject.GetComponent<Text>();//점수 참조

        if (enter == 0)//첫번째로 들어온 참가자는 방장
        {
            InfoWindows[idx].transform.GetChild(3).gameObject.GetComponent<Image>().sprite = Crown;
        }

        //각각의 정보를 초기화한다.
        image.sprite = Resources.Load<Sprite>(GameManager.instance.playerImagePath + "Player" + (enter + 1));
        name.text = playerName;
        score.text = playerScore + " 점";

        InfoWindows[idx].SetActive(true);
    }

    public void UpdatePlayerScore(PlayerInfo info)
    {
        int idx = info.enter;
        Text score = InfoWindows[idx].transform.GetChild(2).gameObject.GetComponent<Text>();//점수 참조

        score.text = info.score + " 점";
    }

    public void UpdatePlayerScore(int enter, int score)
    {
        int idx = enter;
        Text scoreText = InfoWindows[idx].transform.GetChild(2).gameObject.GetComponent<Text>();//점수 참조

        scoreText.text = score + " 점";
    }

    public void HideScore(PlayerInfo info)
    {
        int idx = info.enter;
        InfoWindows[idx].transform.GetChild(2).gameObject.SetActive(false);
    }

    public void HideScore(int enter)
    {
        int idx = enter;
        InfoWindows[idx].transform.GetChild(2).gameObject.SetActive(false);
    }
}
