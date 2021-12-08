using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using Photon.Pun;
using Photon.Realtime;

/*
 InputManagerŬ����
- ä�ù��� �����ϴ� Ŭ����
- �Է�â�� ���� ������� �Է��� �����ϰ� �Է°��� ä��â���� �����Ѵ�.

- Main Stage���� Collecting Time�� ���۵Ǳ� ��, 4���� ���� �� ���� - ���� �λ縦 �ְ�޴� �뵵
- Problem Stage���� Quiz Time �� ����. - ���� ��ȭ�ϰ�, ������ �Է��ϴµ� ����

ä�ð��� �������
- Chatting Log  : �����ڵ��� ������ �Է°����� ������, ��ȭ������ ��������.
- inputField    : ����ڰ� ä���� �ϱ� ���� �ؽ�Ʈ�� �Է��ϴ� ��
- button        : ����ڰ� �ؽ�Ʈ �Է��� �Ϸ��ϰ� Ŭ���ϸ� ���۵Ǵ� ��ư

�ֿ���
- ���� ��ư�� Ŭ���ϰų� ����Ű�� ������ �Էµ� �ؽ�Ʈ�� chatting log�� �̵��Ѵ�.
- ������ �ϰ� �ڿ��� �Է�â(inputField)�� ��� Ȱ��ȭ�Ǽ� �����ؼ� �Է��� �� �ֵ��� �ߴ�.
- InputManager�� �Է�â�� Ȱ��ȭ�ϰų� ��Ȱ��ȭ�� �� �־�� �Ѵ�.
 */

public class InputManager : MonoBehaviourPun
{
    public Button button;//���۹�ư
    public InputField inputField;//�ؽ�Ʈ�� �ԷµǴ� ��
    public Text chattingLog;//����ڵ��� ��ȭ������ ���̴� ��

    public string input;//�Է� �ؽ�Ʈ
    private string playerName;//�÷��̾��� �̸�

    //�� �������� ����ڰ� Text�� �Է��� �� ���� �ֱٿ� �߰��� �ؽ�Ʈ�� �����ְ� �ϱ� �����̴�.
    public RectTransform content;//Chat Text�� ����� �κ�

    void Awake()
    {
        chattingLog.text = "�ݰ����ϴ�.";
        //GameManager�κ��� ���� �÷��̾��� �̸��� �޾ƿ´�.
        Debug.Log("input manager");
        //playerName = GameManager.instance.localPlayer.name;
        playerName = PhotonNetwork.LocalPlayer.NickName;

        //���� ��ư�� �����ų� ����Ű�� ������ �� �̺�Ʈ ���
        //���� ��ư�� ������ ��
        //button click�̺�Ʈ���� pointer down �̺�Ʈ�� ����
        //���� ����
        //  ��ư�� ������ ��, ���� ���·� asdwŰ�� ������ �Ǹ�, �Է�â�� �ٽ� ���õǸ鼭, ������ �Է��ߴ� ���� ������� �ȴ�.
        //  (�Է�â�� �ٽ� ���õǰ� �� ������ ĳ���� �������� �����ϱ� ����)
        //  ������ �Է��ߴ� ���� ������� �Ǵ� ���� �����ϱ� ����, pointer down�̺�Ʈ�� �����ؼ�, �ٷ� ������Ʈ �ǰ� �ߴ�.
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entryPointerDown);

        //����Ű�� ������ ��
        inputField.onEndEdit.AddListener((val) =>
        {
            //�Է°��� �ƹ��͵� ���� ����Ǿ������� InputField�� ���û��¸� �����ؾ��Ѵ�.
            if (val.Length == 0)
            {
                //Attempting to select while already selecting an object����
                //EventSystem.current.SetSelectedGameObject(null);�� ������ �� ������ �߻���.
                //������ �𸣰�����, ������� �غ��� �� �κ��� �ι� ����ȴ�.
                //if (!eventSystem.alreadySelecting)���� ���ð˻縦 ���ָ� �ذ�ȴٰ� �Ѵ�.
                //https://stackoverflow.com/questions/56145437/how-to-make-textmesh-pro-input-field-deselect-on-enter-key
                var eventSystem = EventSystem.current;
                if (!eventSystem.alreadySelecting) eventSystem.SetSelectedGameObject(null);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                SendDelay();
        });
        // EndEdit�� inputField�ٱ��� Ŭ���ϰų�(text������ ��Ȱ��ȭ��) �Ǵ� ����Ű�� ������ �� ����ȴ�.
        // ����Ű�� ������ �� ����ǰ� �Ϸ���
        // if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) ���ǹ��� �߰��ؾ� �ȴ�.
        // val�� inputField�� text��.
    }

    public IEnumerator Send()
    { 
        input = string.Copy(inputField.text);//�Է°� ����
        if (input.Length == 0) {
            yield break;
        }//���̰� 0�� ���� ����
        yield return null;//��� ��� -> inputField�� button�� ���ÿ� Ȱ��ȭ�Ǹ� ������ ���ٰ� �Ѵ�.
                          //�׷��� inputField�� ��Ȱ��ȭ�� ������ ���

        Debug.Log("input log : " + input + " length : " + input.Length);//������
        inputField.text = "";//�Է�â�� �ִ� ���ڴ� ���ش�.

        inputField.ActivateInputField();//����ؼ� �Է��� �� �ֵ��� Ȱ��ȭ��Ų��.
                                        //inputField.Select();
                                        //�� �Լ��� Ȱ��ȭ ���¸� ��Ȱ��ȭ��
                                        //��Ȱ��ȭ ���¸� Ȱ��ȭ�� �ٲ۴�. 

        string log = "\n[" + playerName + "] " + input;
        //Player�� �̸��� �Է°��� �̾�ٿ��� ��°��� �����.

        photonView.RPC("ReceiveMsg", RpcTarget.Others, log);
        ReceiveMsg(log);
        //chattingLog.text += log;//���������� ��ȭ������ ������ �߰��Ѵ�.
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

    //�ڷ�ƾ�� �̿��� ����
    //����Ű�� ������ Send�� �����ϴ� ���� �������� ������ �ȴ�. �׷��� ��ư�� Ŭ������ ��
    //Attempting to select while already selecting an object������ �߻��ߴ�.
    //���� ������ inputField�� ���õ� ���¿��� Button�� �� ���õƱ� ������ �� ����. �������� ������?
    // �ΰ��� UI�� ���ÿ� ���õǸ� �ȵǴ� �� ����.
    //https://answers.unity.com/questions/1315276/event-system-setselectedgameobject-error-but-code.html
    //���⸦ �����غ��� �ι�° ��ư�� ����� �� delay�� �ָ� �ȴٰ� �ؼ� �ϴ� �׷��� ������ �ذ��ߴ�.
    //������ �־ �������࿡ ū ������ �������� Ȥ�ó� �ؼ� �ذ��ߴ�.

    //�ϴ� onEndEdit�̺�Ʈ �߻��ÿ��� ������ ��������
    //Button�̺�Ʈ�� ���� ������ ����ǵ��� �Ȱ��� SendDelay�Լ��� ����ߴ�.

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

        //content�� �ؿ� �ִ� ���� �����Ѵ�.
        //float�� ��Ȯ�� ������ ���� �� ���� ������ ������ �Ǵ��ߴ�.
        if (contentPos < 0.1f && contentPos > -0.1f) return;
        content.anchoredPosition = new Vector2(0f, 0f);
    }

    //Send Button�� EventTrigger - Pointer Down�̺�Ʈ �Լ��� ���
    //��ư�� ������ ��, ��� inputField�� ��Ȱ��ȭ �ȴ�.
    //�׷��� ��ư�� ���� ���·� asdwŰ�� ������ �Ǹ� �÷��̾� ĳ���Ͱ� �����̰� �ȴ�.
    //-> �ֳ��ϸ� inputField�� isFocused���η� �÷��̾� �������� �����߱� ����
    //�׷��� ��ư�� ������ inputField�� ��� Ȱ��ȭ ���·� ���� �� �ְ� �������.
    public void OnPointerDown(PointerEventData data)
    {
        SendDelay();
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
    }
}
