using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 ��ư �̹������� �������� �κи� �����ϵ��� �׽�Ʈ�� ��ũ��Ʈ
 */

public class ButtonTester : MonoBehaviour
{
    /*
     ������ �κ��� ������ �κ�(�������� �κи�) �����ϴ� ���
      
     Texture Import Inspector
        Sprite MeshType �� FullRect�� ����
        Advanced -> Read/Write Enable üũ

     button.GetComponent<Image>().alphaHitTestMinimumThreshold = 1 ���� ����
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
