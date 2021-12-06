using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Skill
{
    Punch,
    Crush
}

public class SkillCoolTimer : MonoBehaviour
{
    public Skill skill;
    public Image coolGage;
    private float skillCoolTime;//������
    private float initialTime;//�ʱⰪ
    private bool isCool = false;//true�� ��ų ������, false�� ��ų ��밡��
    private int mouseKey = 0;//0���� Ű, 1������ Ű
    //public static bool Is_Punch = false;
    //public static bool canpunch = true;
    
    // Start is called before the first frame update
    void Start()
    {
        switch (skill)
        {
            case Skill.Punch:
                skillCoolTime = GameManager.instance.skillPunchCoolTime;
                mouseKey = 1;
                break;

            case Skill.Crush:
                skillCoolTime = GameManager.instance.skillCrushCoolTime;
                mouseKey = 0;
                break;
        }
        initialTime = skillCoolTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(mouseKey))//���콺 Ŭ���� coolŸ�� ���¸� Ų��.
        {
            isCool = true;
        }

        //�ð��� ���ҽ�Ű�� ������� �ߴµ�, ������Ű�� ��ĵ� ������ �� ����.
        if (isCool)
        {
            if (skillCoolTime > 0)//��Ÿ�� �ð��� �����ִ� ���
            {
                skillCoolTime -= Time.deltaTime;
                coolGage.fillAmount = (initialTime - skillCoolTime)/initialTime;//�������� ä���ش�.
            }
            else//�ð��� ������ �ʱ�ȭ�� �Ѵ�.
            {
                skillCoolTime = initialTime;//�ð� ����
                isCool = false;//�ٽ� ��ų�� �� �� �ְ� ����
            }
        }
    }
}
