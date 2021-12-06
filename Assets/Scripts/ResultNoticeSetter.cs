using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 ResultNoticeSetter Ŭ����
- Result�˸������� �ʱ�ȭ�ϴ� Ŭ����

�ֿ���
- �Է¹��� ������� Player�� ������ ������ش�.
- ����1���� �ִ� ��� 1�� ������ ���� ���� Player�� ǥ���Ѵ�.
- ���� 2,3���� �ִ� ���� ���� ���� ������ 2nd, 3rdǥ�ø� �����ν� ������ ǥ���Ѵ�.
 */

public class ResultNoticeSetter : MonoBehaviour
{
    public Text[] names;//�÷��̾��� �̸��� ǥ�õǴ� UI
    public Image[] images;//�÷��̾� ĳ���� �̹���
    public Text[] scores;//�÷��̾��� ������ ǥ�õǴ� ��
    public Text[] ranks;//�÷��̾��� ������ ǥ�õǴ� ��
    private string[] rankText = { "1��", "2��", "3��", "4��" };//�÷��̾��� ������ ��Ÿ�� string

    //�÷��̾��� ������ �����ϴ� �Լ�
    //idx : �� ��° UI�� ������ ������
    //rank : ������ ��� �Ǵ���
    //info : �÷��̾��� ���� -> �̸�, ����, 
    public void SetPlayer(int idx, string rank, PlayerInfo info)
    {
        names[idx].text = info.name;//�̸� ����
        //���� ����
        if (idx == 0) scores[idx].text = "���� : " + info.score;
        else scores[idx].text = "" + info.score;

        //ĳ���� �̹����� ���� ����
        images[idx].sprite = info.sprite;
        ranks[idx].text = rank;

        //������Ʈ Ȱ��ȭ
        names[idx].gameObject.SetActive(true);
        images[idx].gameObject.SetActive(true);
        ranks[idx].gameObject.SetActive(true);
        scores[idx].gameObject.SetActive(true);
    }

    //����1���� �����ϱ� ���� �Լ�
    public void Add1stPlayer(PlayerInfo info)
    {
        Debug.Log("player name : " + info.name);
        names[0].text += (", " + info.name);//������� �̸��� �߰��Ѵ�.
        Debug.Log("cur name : " + names[0].text);
        GameObject newImage = Instantiate(images[0].gameObject);//���Ӱ� �߰��� �������� ĳ���� �̹����� �߰��Ѵ�.
        newImage.transform.SetParent(images[0].gameObject.transform.parent);//������Ʈ�� ��ġ ����
        newImage.transform.localScale = Vector3.one;//������ �ʱ�ȭ
        newImage.GetComponent<Image>().sprite = info.sprite;//�̹��� ����
    }

    //���ĵ� ������� PlayerInfo����Ʈ�� �޴´�.
    //�Է¹��� ������� ������ ǥ���Ѵ�.
    public void SetResult(List<PlayerInfo> players)
    {
        //���� ��� ������Ʈ�� ��Ȱ��ȭ�Ѵ�. -> ���߿� �߰��� ������Ʈ�� Ȱ��ȭ�Ѵ�.
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
        int lastScore = players[0].score;//�����ڸ� �Ǵ��ϱ� ���� ����

        SetPlayer(0, rankText[0], players[0]);//1���� ���� �����Ѵ�.
        
        int playersIdx = 1;//players�� ������ �ε���
        int objectsIdx = 1;//object�� ������ ������
        int rankIdx = 1;//rank�� ������ �ε���

        //���� 1���� ���� �����Ѵ�.
        while (playersIdx < GameManager.instance.totalPlayerNum && players[playersIdx].score == lastScore)
        {
            Add1stPlayer(players[playersIdx]);
            playersIdx++;
        }
        //���� 1���� 4���� ���, �ٷ� ��
        if (playersIdx == GameManager.instance.totalPlayerNum) return;
        
        //���� ������ ����
        lastScore = players[playersIdx].score;
        for(;playersIdx < GameManager.instance.totalPlayerNum; playersIdx++)
        {
            if (lastScore != players[playersIdx].score)//������ �ٸ� ���
            {
                lastScore = players[playersIdx].score;//lastScore ����
                rankIdx++;//rank����
            }
            SetPlayer(objectsIdx, rankText[rankIdx], players[playersIdx]);
            objectsIdx++;
        }
    }
}
