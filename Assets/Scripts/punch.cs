using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class punch : MonoBehaviour
{
    public Image img_punch;
    public static bool Is_Punch = false;
    public static bool canpunch = true;
    private float initialTime;

    void Start()
    {
        initialTime = GameManager.instance.skillPunchCoolTime;
    }
    void Update()
    {
        
        if(Input.GetMouseButtonDown(1) && Is_Punch == true)//마우스 우클릭시
        {
            
            StartCoroutine(CoolTime(initialTime));//코루틴 함수 실행->쿨타임 실행
            
        }
    }
    IEnumerator CoolTime(float cool)
    {
        
        while(cool>0.3f)
        {
            cool -= Time.deltaTime;
            img_punch.fillAmount = (1.0f / cool);
            Is_Punch = false;
            canpunch = false;
            yield return null;
            canpunch = true;
        }
    }
}


