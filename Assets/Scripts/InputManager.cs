using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using Photon.Pun;
using Photon.Realtime;

/*
 InputManager클래스
- 채팅방을 관리하는 클래스
- 입력창을 통해 사용자의 입력을 감지하고 입력값을 채팅창으로 전송한다.

- Main Stage에서 Collecting Time이 시작되기 전, 4명이 모일 때 사용됨 - 서로 인사를 주고받는 용도
- Problem Stage에서 Quiz Time 때 사용됨. - 서로 대화하고, 정답을 입력하는데 사용됨

채팅공간 구성요소
- Chatting Log  : 참가자들이 전송한 입력값들이 보여짐, 대화내역이 보여진다.
- inputField    : 사용자가 채팅을 하기 위해 텍스트를 입력하는 곳
- button        : 사용자가 텍스트 입력을 완료하고 클릭하면 전송되는 버튼

주요기능
- 전송 버튼을 클릭하거나 엔터키를 누르면 입력된 텍스트가 chatting log로 이동한다.
- 전송을 하고난 뒤에도 입력창(inputField)가 계속 활성화되서 연속해서 입력할 수 있도록 했다.
- InputManager는 입력창을 활성화하거나 비활성화할 수 있어야 한다.
 */

public class InputManager : MonoBehaviourPun
{
    public Button button;//전송버튼
    public InputField inputField;//텍스트가 입력되는 곳
    public Text chattingLog;//사용자들의 대화내역이 보이는 곳

    public string input;//입력 텍스트
    private string playerName;//플레이어의 이름

    //이 변수들은 사용자가 Text를 입력할 때 가장 최근에 추가된 텍스트를 보여주게 하기 위함이다.
    public RectTransform content;//Chat Text가 담겨질 부분

    void Awake()
    {
        chattingLog.text = "반갑습니다.";
        //GameManager로부터 로컬 플레이어의 이름을 받아온다.
        Debug.Log("input manager");
        //playerName = GameManager.instance.localPlayer.name;
        playerName = PhotonNetwork.LocalPlayer.NickName;

        //전송 버튼을 누르거나 엔터키를 눌렀을 때 이벤트 등록
        //전송 버튼을 눌렀을 때
        //button click이벤트에서 pointer down 이벤트로 변경
        //변경 이유
        //  버튼이 눌렸을 때, 누른 상태로 asdw키를 누르게 되면, 입력창이 다시 선택되면서, 기존에 입력했던 값이 사라지게 된다.
        //  (입력창이 다시 선택되게 한 이유는 캐릭터 움직임을 제한하기 위해)
        //  기존에 입력했던 값이 사라지게 되는 것을 방지하기 위해, pointer down이벤트로 변경해서, 바로 업데이트 되게 했다.
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entryPointerDown);

        //엔터키를 눌렀을 때
        inputField.onEndEdit.AddListener((val) =>
        {
            //입력값이 아무것도 없이 종료되었을때는 InputField의 선택상태를 해제해야한다.
            if (val.Length == 0)
            {
                //Attempting to select while already selecting an object에러
                //EventSystem.current.SetSelectedGameObject(null);를 실행할 때 에러가 발생함.
                //이유는 모르겠지만, 디버깅을 해보면 이 부분이 두번 실행된다.
                //if (!eventSystem.alreadySelecting)으로 선택검사를 해주면 해결된다고 한다.
                //https://stackoverflow.com/questions/56145437/how-to-make-textmesh-pro-input-field-deselect-on-enter-key
                var eventSystem = EventSystem.current;
                if (!eventSystem.alreadySelecting) eventSystem.SetSelectedGameObject(null);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                SendDelay();
        });
        // EndEdit은 inputField바깥을 클릭하거나(text선택이 비활성화됨) 또는 엔터키를 눌렀을 때 실행된다.
        // 엔터키만 눌렀을 때 실행되게 하려면
        // if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) 조건문을 추가해야 된다.
        // val은 inputField의 text다.
    }

    public IEnumerator Send()
    { 
        input = string.Copy(inputField.text);//입력값 세팅
        if (input.Length == 0) {
            yield break;
        }//길이가 0인 경우는 종료
        yield return null;//잠시 대기 -> inputField와 button이 동시에 활성화되면 에러가 난다고 한다.
                          //그래서 inputField가 비활성화될 때까지 대기

        Debug.Log("input log : " + input + " length : " + input.Length);//디버깅용
        inputField.text = "";//입력창에 있는 글자는 없앤다.

        inputField.ActivateInputField();//계속해서 입력할 수 있도록 활성화시킨다.
                                        //inputField.Select();
                                        //이 함수는 활성화 상태면 비활성화로
                                        //비활성화 상태면 활성화로 바꾼다. 

        string log = "\n[" + playerName + "] " + input;
        //Player의 이름와 입력값을 이어붙여서 출력값을 만든다.

        photonView.RPC("ReceiveMsg", RpcTarget.Others, log);
        ReceiveMsg(log);
        //chattingLog.text += log;//최종적으로 대화내역에 내용을 추가한다.
        //PlaceContentBottom();
    }

    [PunRPC]
    public void ReceiveMsg(string log)
    {
        chattingLog.text += log;
        PlaceContentBottom();
    }

    public void SendDelay()
    {
        StartCoroutine(Send());
    }

    //코루틴을 이용한 이유
    //엔터키를 눌러서 Send를 실행하는 것은 에러없이 실행이 된다. 그런데 버튼을 클릭했을 때
    //Attempting to select while already selecting an object에러가 발생했다.
    //에러 원인은 inputField가 선택된 상태에서 Button이 또 선택됐기 때문인 것 같다. 한프레임 내에서?
    // 두개의 UI가 동시에 선택되면 안되는 것 같다.
    //https://answers.unity.com/questions/1315276/event-system-setselectedgameobject-error-but-code.html
    //여기를 참고해보니 두번째 버튼이 실행될 때 delay를 주면 된다고 해서 일단 그렇게 오류를 해결했다.
    //에러가 있어도 게임진행에 큰 문제는 없었지만 혹시나 해서 해결했다.

    //일단 onEndEdit이벤트 발생시에는 문제가 없었지만
    //Button이벤트와 같은 시점에 실행되도록 똑같이 SendDelay함수를 등록했다.

    public void DisableInput()
    {
        inputField.interactable = false;
        button.interactable = false;
        var eventSystem = EventSystem.current;
        if (!eventSystem.alreadySelecting) eventSystem.SetSelectedGameObject(null);
        inputField.DeactivateInputField();
        inputField.text = "";
    }

    public void EnableInput()
    {
        inputField.interactable = true;
        inputField.ActivateInputField();
        button.interactable = true;
    }

    public void PlaceContentBottom()
    {
        float contentPos = content.anchoredPosition.y;

        //content가 밑에 있는 경우는 종료한다.
        //float는 정확한 값으로 비교할 수 없기 때문에 범위로 판단했다.
        if (contentPos < 0.1f && contentPos > -0.1f) return;
        content.anchoredPosition = new Vector2(0f, 0f);
    }

    //Send Button에 EventTrigger - Pointer Down이벤트 함수로 등록
    //버튼이 눌렸을 때, 잠시 inputField가 비활성화 된다.
    //그래서 버튼이 눌린 상태로 asdw키를 누르게 되면 플레이어 캐릭터가 움직이게 된다.
    //-> 왜냐하면 inputField의 isFocused여부로 플레이어 움직임을 제한했기 때문
    //그래서 버튼이 눌려도 inputField가 계속 활성화 상태로 있을 수 있게 만들었다.
    public void OnPointerDown(PointerEventData data)
    {
        SendDelay();
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }
}
