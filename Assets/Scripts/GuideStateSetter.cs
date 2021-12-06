using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 GuideStateSetter Ŭ����
- Guide State�� ������ ���� ��, �÷��̾�� ���� �ð��� ���þ �����ش�.
- Guide State SetterŬ������ ���þ �����ϰ�, ���� �ð��� ������ �� �ֵ��� �Ѵ�.
 */

public class GuideStateSetter : MonoBehaviour
{
    public Text timer;//���� �ð� ǥ��
    public Text proposedWord;//���þ� ǥ��
    public GameObject pressKeyGuide;//������ �ݱ� �ȳ�

    private void OnEnable()
    {
        if(pressKeyGuide != null)
        {
            pressKeyGuide.SetActive(false);
        }
    }

    //���� �ð��� �����ϰ� �����Ͽ� �˾ƺ��� ���� ǥ���Ѵ�.
    public string CalTime(float time)
    {
        time += 1;
        int min = (int)(time / 60f);
        int seconds = (int)(time - min * 60f);
        return (int)(min / 10f) + "" + (min % 10) + ":" + (int)(seconds / 10f) + "" + (seconds % 10);
    }

    public void SetTime(float time)
    {
        timer.text = "" + CalTime(time);
    }

    public void SetWord(string word)
    {
        proposedWord.text = "���þ� : " + word;
    }
}
