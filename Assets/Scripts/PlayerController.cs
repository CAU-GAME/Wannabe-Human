using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * Test용으로 만든 클래스
 PlayerController 클래스
- 참가자의 입력을 받아서 캐릭터를 움직인다.
 */
public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    public float speed = 100f;
    //현재 게임 플레이 상태에 따라서 캐릭터의 움직임을 제한하기 위해 필요하다.
    public MainStageManager mainStageManager;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //사용자가 채팅창에 입력중일 땐(입력창이 활성화되어 있는 상태 - 선택되어 있는 상태), 캐릭터의 움직임을 제한한다.
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
