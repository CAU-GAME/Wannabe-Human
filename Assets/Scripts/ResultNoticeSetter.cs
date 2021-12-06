using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 ResultNoticeSetter 클래스
- Result알림문구를 초기화하는 클래스

주요기능
- 입력받은 순서대로 Player의 정보를 출력해준다.
- 공동1등이 있는 경우 1등 공간에 여러 명의 Player를 표시한다.
- 공동 2,3등이 있는 경우는 각각 왼쪽 순위에 2nd, 3rd표시를 함으로써 순위를 표시한다.
 */

public class ResultNoticeSetter : MonoBehaviour
{
    public Text[] names;//플레이어의 이름이 표시되는 UI
    public Image[] images;//플레이어 캐릭터 이미지
    public Text[] scores;//플레이어의 점수가 표시되는 곳
    public Text[] ranks;//플레이어의 순위가 표시되는 곳
    private string[] rankText = { "1등", "2등", "3등", "4등" };//플레이어의 순위를 나타낼 string

    //플레이어의 정보를 세팅하는 함수
    //idx : 몇 번째 UI에 저장할 것인지
    //rank : 순위가 어떻게 되는지
    //info : 플레이어의 정보 -> 이름, 점수, 
    public void SetPlayer(int idx, string rank, PlayerInfo info)
    {
        names[idx].text = info.name;//이름 저장
        //점수 저장
        if (idx == 0) scores[idx].text = "점수 : " + info.score;
        else scores[idx].text = "" + info.score;

        //캐릭터 이미지와 순위 저장
        images[idx].sprite = info.sprite;
        ranks[idx].text = rank;

        //오브젝트 활성화
        names[idx].gameObject.SetActive(true);
        images[idx].gameObject.SetActive(true);
        ranks[idx].gameObject.SetActive(true);
        scores[idx].gameObject.SetActive(true);
    }

    //공동1등을 저장하기 위한 함수
    public void Add1stPlayer(PlayerInfo info)
    {
        Debug.Log("player name : " + info.name);
        names[0].text += (", " + info.name);//우승자의 이름을 추가한다.
        Debug.Log("cur name : " + names[0].text);
        GameObject newImage = Instantiate(images[0].gameObject);//새롭게 추가된 참가자의 캐릭터 이미지를 추가한다.
        newImage.transform.SetParent(images[0].gameObject.transform.parent);//오브젝트의 위치 설정
        newImage.transform.localScale = Vector3.one;//스케일 초기화
        newImage.GetComponent<Image>().sprite = info.sprite;//이미지 설정
    }

    //정렬된 순서대로 PlayerInfo리스트를 받는다.
    //입력받은 순서대로 순위를 표시한다.
    public void SetResult(List<PlayerInfo> players)
    {
        //먼저 모든 오브젝트를 비활성화한다. -> 나중에 추가된 오브젝트만 활성화한다.
        for(int i = 0; i < names.Length; i++)
        {
            names[i].text = "";
            scores[i].text = "";

            names[i].gameObject.SetActive(false);
            scores[i].gameObject.SetActive(false);
            images[i].gameObject.SetActive(false);
            ranks[i].gameObject.SetActive(false);
        }

        Debug.Log("Set Result");
        int lastScore = players[0].score;//동점자를 판단하기 위한 변수

        SetPlayer(0, rankText[0], players[0]);//1등을 먼저 저장한다.
        
        int playersIdx = 1;//players에 접근할 인덱스
        int objectsIdx = 1;//object에 접근할 인젝스
        int rankIdx = 1;//rank에 접근할 인덱스

        //공동 1등을 먼저 저장한다.
        while (playersIdx < GameManager.instance.totalPlayerNum && players[playersIdx].score == lastScore)
        {
            Add1stPlayer(players[playersIdx]);
            playersIdx++;
        }
        //공동 1등이 4명인 경우, 바로 끝
        if (playersIdx == GameManager.instance.totalPlayerNum) return;
        
        //이전 점수를 저장
        lastScore = players[playersIdx].score;
        for(;playersIdx < GameManager.instance.totalPlayerNum; playersIdx++)
        {
            if (lastScore != players[playersIdx].score)//점수가 다른 경우
            {
                lastScore = players[playersIdx].score;//lastScore 갱신
                rankIdx++;//rank증가
            }
            SetPlayer(objectsIdx, rankText[rankIdx], players[playersIdx]);
            objectsIdx++;
        }
    }
}
