using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 버튼 이미지에서 불투명한 부분만 선택하도록 테스트한 스크립트
 */

public class ButtonTester : MonoBehaviour
{
    /*
     투명한 부분을 제외한 부분(불투명한 부분만) 선택하는 방법
      
     Texture Import Inspector
        Sprite MeshType 을 FullRect로 설정
        Advanced -> Read/Write Enable 체크

     button.GetComponent<Image>().alphaHitTestMinimumThreshold = 1 투명도 설정
     */

    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickButton);
        button.GetComponent<Image>().alphaHitTestMinimumThreshold = 1;
    }

    public void OnClickButton()
    {
        Debug.Log(gameObject.name + " Button is clicked");
    }
}
