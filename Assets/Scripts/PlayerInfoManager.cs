using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 PlayerInfoManager Ŭ����
- ������ ��ܿ� Player�� ������ ǥ�õǴ� UI�� �����ϴ� Ŭ����

- Right Panel�� Player Infos�� ����

�ֿ���
- UI�� PlayerInfo�� ǥ���Ѵ�.
- ���忡�� �հ� �̹����� ǥ���Ѵ�.
 
 */
public class PlayerInfoManager : MonoBehaviour
{
    public GameObject[] InfoWindows;//Player�� ����(ĳ���� ����, �г���, ����)�� ǥ���� UI
    public Sprite Crown;//������ ǥ���� �հ� �̹���
    private int MaxPlayerNum = 4;//�ִ� ���� �ο�

    void Awake()
    {
        InfoWindows = new GameObject[MaxPlayerNum];
        //UI����
        for(int i = 0; i < MaxPlayerNum; i++)
        {
            InfoWindows[i] = transform.GetChild(i).gameObject;
        }
    }

    //PlayerInfo�� �޾Ƽ� UI�� Player�� ������ �ʱ�ȭ�ϴ� �Լ�
    //���� ������� ���� �� -> ������ �� -> ���� �Ʒ� -> ������ �Ʒ� ������ ����ȴ�.
    //���� ���� ������� ��ġ�ϰ� �ʹٸ� idx�κ��� info.order�� ������ �ȴ�.
    public void SetPlayerInfo(PlayerInfo info)//�÷��̾� ����
    {
        int idx = info.enter;//���� ������� ��ġ��ų��, �ƴϸ� ���� ������� ��ġ��ų��, order�ƴϸ� enter�� ����

        //�ش��ϴ� UI ��ü ����
        Image image = InfoWindows[idx].transform.GetChild(0).gameObject.GetComponent<Image>();//�̹��� ����
        Text name = InfoWindows[idx].transform.GetChild(1).gameObject.GetComponent<Text>();//�̸� ����
        Text score = InfoWindows[idx].transform.GetChild(2).gameObject.GetComponent<Text>();//���� ����
        
        if(info.enter == 0)//ù��°�� ���� �����ڴ� ����
        {
            InfoWindows[idx].transform.GetChild(3).gameObject.GetComponent<Image>().sprite = Crown;
        }

        //������ ������ �ʱ�ȭ�Ѵ�.
        image.sprite = info.sprite;
        name.text = info.name;
        score.text = info.score + " ��";

        InfoWindows[idx].SetActive(true);
    }

    public void SetPlayerInfo(string playerName, int enter, int playerScore)
    {
        int idx = enter;//���� ������� ��ġ��ų��, �ƴϸ� ���� ������� ��ġ��ų��, order�ƴϸ� enter�� ����

        //�ش��ϴ� UI ��ü ����
        Image image = InfoWindows[idx].transform.GetChild(0).gameObject.GetComponent<Image>();//�̹��� ����
        Text name = InfoWindows[idx].transform.GetChild(1).gameObject.GetComponent<Text>();//�̸� ����
        Text score = InfoWindows[idx].transform.GetChild(2).gameObject.GetComponent<Text>();//���� ����

        if (enter == 0)//ù��°�� ���� �����ڴ� ����
        {
            InfoWindows[idx].transform.GetChild(3).gameObject.GetComponent<Image>().sprite = Crown;
        }

        //������ ������ �ʱ�ȭ�Ѵ�.
        image.sprite = Resources.Load<Sprite>(GameManager.instance.playerImagePath + "Player" + (enter + 1));
        name.text = playerName;
        score.text = playerScore + " ��";

        InfoWindows[idx].SetActive(true);
    }

    public void UpdatePlayerScore(PlayerInfo info)
    {
        int idx = info.enter;
        Text score = InfoWindows[idx].transform.GetChild(2).gameObject.GetComponent<Text>();//���� ����

        score.text = info.score + " ��";
    }

    public void UpdatePlayerScore(int enter, int score)
    {
        int idx = enter;
        Text scoreText = InfoWindows[idx].transform.GetChild(2).gameObject.GetComponent<Text>();//���� ����

        scoreText.text = score + " ��";
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
