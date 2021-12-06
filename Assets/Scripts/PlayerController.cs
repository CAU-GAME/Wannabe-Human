using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * Test������ ���� Ŭ����
 PlayerController Ŭ����
- �������� �Է��� �޾Ƽ� ĳ���͸� �����δ�.
 */
public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    public float speed = 100f;
    //���� ���� �÷��� ���¿� ���� ĳ������ �������� �����ϱ� ���� �ʿ��ϴ�.
    public MainStageManager mainStageManager;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //����ڰ� ä��â�� �Է����� ��(�Է�â�� Ȱ��ȭ�Ǿ� �ִ� ���� - ���õǾ� �ִ� ����), ĳ������ �������� �����Ѵ�.
        if (EventSystem.current.currentSelectedGameObject != null)
            if (EventSystem.current.currentSelectedGameObject == mainStageManager.chatting.inputField.gameObject)
                return;

        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        float xSpeed = xInput * speed;
        float zSpeed = zInput * speed;

        Vector3 newVelocity = new Vector3(xSpeed, 0f, zSpeed);
        playerRigidbody.velocity = newVelocity;
    }
}
