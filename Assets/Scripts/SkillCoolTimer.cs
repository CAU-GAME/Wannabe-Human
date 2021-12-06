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
    private float skillCoolTime;//측정값
    private float initialTime;//초기값
    private bool isCool = false;//true면 스킬 사용못함, false면 스킬 사용가능
    private int mouseKey = 0;//0왼쪽 키, 1오른쪽 키
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
        if (Input.GetMouseButtonDown(mouseKey))//마우스 클릭시 cool타임 상태를 킨다.
        {
            isCool = true;
        }

        //시간을 감소시키는 방식으로 했는데, 증가시키는 방식도 괜찮을 것 같다.
        if (isCool)
        {
            if (skillCoolTime > 0)//쿨타임 시간이 남아있는 경우
            {
                skillCoolTime -= Time.deltaTime;
                coolGage.fillAmount = (initialTime - skillCoolTime)/initialTime;//게이지를 채워준다.
            }
            else//시간이 지나면 초기화를 한다.
            {
                skillCoolTime = initialTime;//시간 설정
                isCool = false;//다시 스킬을 쓸 수 있게 설정
            }
        }
    }
}
