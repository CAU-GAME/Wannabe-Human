using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
button�� inputField�� �����ذ��� �׽�Ʈ�غ� ��ũ��Ʈ 

������
 1. �Է»����� ��, awsdŰ�� ������ ĳ���͵� �����̰�, �Է�â�� ���ڵ� �Էµȴ�. -> �÷��̾� ���� �� �Է�â�� Ȱ��ȭ ���� ��, �� �����̵��� ��ġ�ؾ���
 2. InputField�� ���û����϶�, ȭ��ǥ Ű�� ��������, Game Start��ư�� Ŭ���ǰų�, ���� ��ư�� Ŭ���Ǵ� ��찡 �߻� -> Navigation�� None���� �����ؾ� �ȴ�.
 3. ��ư�� �ѹ� Ŭ���ϰ� ����, ���Ŀ� ����Ű�� ������ ���۹�ư�� �Ǵٽ� Ŭ���Ǵ� ����
 4. InputField�� �Է»��°� ������� ��(������ ���û����� ��), ����Ű�� ������ �ٽ� �Է»��·� ����ȴ�.
    -> 3,4���� UI�� ���û��¸� �����ؾ� �Ѵ�. 
    -> button�� onClick�̺�Ʈ �߻��� �Լ��� ������ �Ŀ�, inputField�� onEndEdit���� �Ŀ� ���õ� UI�� �������־�� �� �� ����.

 Button�̳� InputField���� UI ������Ʈ���� Ŭ���Ǹ� ���û��°� �ȴ�.
inspectorâ�� ���� Navigation�� �����ϴ� �κ��� �ִµ�, Automatic�� �⺻���̴�.

Navigation�� ���õ� ���¿��� ȭ��ǥŰ�� ������ ������ ���� ���û��°� ���̵ǵ��� �ϴ� Ư���� �� ����.
��ư�� �ѹ� Ŭ���ϸ� ���û��·� ���ϰ� �� ���¿��� ȭ��ǥŰ�� ������ �ٷ� ���� �ִ� inputField�� ���û��·� ���ϰ� �ȴ�.
ȭ��ǥ�� Navigation�� �κп� �ִ� Visualize�� ������ ������ �󿡼� ǥ�õȴ�.
Navigation�� None���� �����ϸ� ȭ��ǥ�� ������ UI ���� ���°� �ٸ� UI�� ���� ���� �ʴ´�.

��ư�� ���õ� ���¿��� ȭ��ǥŰ�� ���� ���¸� ���̽����� ��, InputField�� ���õǴ� ���
InputField�� ���õǸ� �Է»��·� ��ٷ� ���ϰ� �ȴ�. �� ���� Ű���带 ������ ������ �Է�â���� �Է°��� ���޵Ǵ� �� ����.
onEndEdit�̺�Ʈ�� ȣ��� �Ŀ��� �Է»��°� �����ȴ�. �Է»��°� �����ǵ� ������ ���õ� ���·� �����ִ�.
�Է»��°� �����Ǹ� ȭ��ǥ�� ������ ��(������ ���û����� ������), ���°� ���̵ȴ�.

��ư�� ���� ������ ���� ����Ű�� ������ ��, Ŭ���̺�Ʈ�� �߻��ϰ� �ȴ�.
InputField�� ���� ������ �� ����Ű�� ������, �Է»��·� ���̵ȴ�. -> Ŀ���� �����Ÿ��� �ȴ�.
�Է»������� �ƴ����� Ȯ���ϴ� ���� isFocused�� ����ȴ�.

���� ������ ��, ����Ű �Է��� �����ϱ� ���ؼ��� ������ �����ؾ� �Ѵ�.
EventSystem.current.SetSelectedGameObject(null);
��ư�� ��쿡�� Navigation�� None���θ� ���൵ ���û��·� ������ �ȵǴ� �� ����.

���� ���õ� UI�� Ȯ���ϴ� ���
EventSystem.current.currentSelectedGameObject
���õ� object�� ���� ���� null�̹Ƿ� ����ó���� ����� �Ѵ�.
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
