using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
button과 inputField에 부착해가며 테스트해본 스크립트 

문제점
 1. 입력상태일 때, awsd키를 누르면 캐릭터도 움직이고, 입력창에 글자도 입력된다. -> 플레이어 조작 시 입력창이 활성화 됐을 때, 안 움직이도록 조치해야함
 2. InputField가 선택상태일때, 화살표 키를 눌렀더니, Game Start버튼이 클릭되거나, 전송 버튼이 클릭되는 경우가 발생 -> Navigation을 None으로 변경해야 된다.
 3. 버튼을 한번 클릭하고 나서, 이후에 엔터키를 누르면 전송버튼이 또다시 클릭되는 현상
 4. InputField의 입력상태가 종료됐을 때(하지만 선택상태일 때), 엔터키를 누르면 다시 입력상태로 변경된다.
    -> 3,4번은 UI의 선택상태를 해제해야 한다. 
    -> button은 onClick이벤트 발생시 함수를 실행한 후에, inputField는 onEndEdit실행 후에 선택된 UI를 해제해주어야 될 것 같다.

 Button이나 InputField같은 UI 오브젝트들은 클릭되면 선택상태가 된다.
inspector창을 보면 Navigation을 설정하는 부분이 있는데, Automatic이 기본값이다.

Navigation은 선택된 상태에서 화살표키를 누르면 방향을 따라서 선택상태가 전이되도록 하는 특성인 것 같다.
버튼을 한번 클릭하면 선택상태로 변하고 이 상태에서 화살표키를 누르면 바로 옆에 있던 inputField가 선택상태로 변하게 된다.
화살표는 Navigation밑 부분에 있는 Visualize를 누르면 에디터 상에서 표시된다.
Navigation을 None으로 변경하면 화살표를 눌러도 UI 선택 상태가 다른 UI로 전이 되지 않는다.

버튼이 선택된 상태에서 화살표키를 눌러 상태를 전이시켰을 때, InputField가 선택되는 경우
InputField가 선택되면 입력상태로 곧바로 변하게 된다. 그 다음 키보드를 누르면 무조건 입력창으로 입력값이 전달되는 것 같다.
onEndEdit이벤트가 호출된 후에야 입력상태가 해제된다. 입력상태가 해제되도 여전히 선택된 상태로 남아있다.
입력상태가 해제되면 화살표를 눌렀을 때(오로지 선택상태일 때에만), 상태가 전이된다.

버튼이 선택 상태일 때는 엔터키를 눌렀을 때, 클릭이벤트가 발생하게 된다.
InputField가 선택 상태일 때 엔터키를 누르면, 입력상태로 전이된다. -> 커서가 깜빡거리게 된다.
입력상태인지 아닌지를 확인하는 것은 isFocused를 보면된다.

선택 상태일 때, 엔터키 입력을 방지하기 위해서는 선택을 해제해야 한다.
EventSystem.current.SetSelectedGameObject(null);
버튼인 경우에는 Navigation을 None으로만 해줘도 선택상태로 변경이 안되는 것 같다.

현재 선택된 UI를 확인하는 방법
EventSystem.current.currentSelectedGameObject
선택된 object가 없는 경우는 null이므로 예외처리를 해줘야 한다.
 */

public enum whatUI
{
    button,
    inputField,
    image
}

public class UITester : MonoBehaviour, IEventSystemHandler
{
    public whatUI ui = whatUI.button;
    private Button button;
    private InputField inputField;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        if(ui == whatUI.button)
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClickButton);
        }
        if (ui == whatUI.inputField)
        {
            inputField = GetComponent<InputField>();
            inputField.onValueChanged.AddListener(val => OnValueChanged());
        }
        if(ui == whatUI.image)
        {
            image = GetComponent<Image>();
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    if(EventSystem.current.currentSelectedGameObject == null)
        //    {
        //        Debug.Log(gameObject.name + " current selected is null");
        //        return;
        //    }
        //    Debug.Log("("+gameObject.name + ") current selected : " + EventSystem.current.currentSelectedGameObject.name);
        //    if(inputField != null)
        //        Debug.Log("(" + gameObject.name + ") input field is Focused? " + inputField.isFocused);

        //    if (EventSystem.current.currentSelectedGameObject != null)
        //    {
        //        Debug.Log("before set");
        //        EventSystem.current.SetSelectedGameObject(null);
        //        Debug.Log("after set");
        //    }
        //}
        //Vector3 mousepos = Input.mousePosition;
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("image size delta : " + image.rectTransform.sizeDelta);
        //    Debug.Log("image rect size : " + image.rectTransform.rect.size);
            
        //    Debug.Log("mouse pos : " + mousepos);
        //    Debug.Log("viewport cor : " + Camera.main.ScreenToViewportPoint(mousepos));
        //}

        //if (Input.GetMouseButton(0))
        //{
        //    Debug.Log("mouse pos : " + mousepos);
        //}
    }

    public void OnClickButton()
    {
        //Debug.Log(button.gameObject.name + " is Clicked");
        //if (EventSystem.current.currentSelectedGameObject == null)
        //{
        //    Debug.Log("current selected is null");
        //    return;
        //}
        //Debug.Log("current selected : " + EventSystem.current.currentSelectedGameObject.name);
    }

    public void OnValueChanged()
    {
        //Debug.Log(inputField.gameObject.name + " is changed");
    }

}
