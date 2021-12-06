using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class crush : MonoBehaviour
{
    public Image img_crush;
    public static  bool Is_crush = false;
    public static bool cancrush = true;
    void Start()
    {

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Is_crush == true)//마우스 좌클릭시
        { 
            
            StartCoroutine(CoolTime(6f));//코루틴 함수 실행->쿨타임 실행
            
        }
    }
    IEnumerator CoolTime(float cool)
    {
        while (cool > 0.3f)
        {
            
            cool -= Time.deltaTime;
            img_crush.fillAmount = (1.0f / cool);
            Is_crush = false;
            cancrush = false;
            yield return new WaitForFixedUpdate();
            cancrush=true;
        }
    }
}
