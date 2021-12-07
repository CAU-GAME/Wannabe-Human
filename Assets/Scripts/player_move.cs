using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;


public class player_move : MonoBehaviourPunCallbacks
{
    public static player_move Inst {get; private set;}
    float hAxis;
    float vAxis;
    bool isSpace;
    GameObject canGetItem;

    Vector3 moveVec;
    public float speed = 5f;    //속도
    private Animator animator;
    public static bool drop = false;
    Rigidbody rigid;

    public MainStageManager mainStageManager;
    private PlayerPickupItem pickup;
    private PhotonView pv;
    private GameObject destroyingTarget;
   
    void Start()
    {
        rigid = GetComponent<Rigidbody>(); 
        animator = GetComponentInChildren<Animator>();
        pickup = GetComponent<PlayerPickupItem>();

        mainStageManager = GameObject.Find("Main Stage Manager").GetComponent<MainStageManager>();

        if (!GetComponent<PhotonView>().IsMine)
        {
            Destroy(transform.GetChild(8).gameObject);
            Destroy(GetComponent<PlayerPickupItem>());
            Destroy(GetComponent<player_move>());
        }
        else
        {
            if (Inst == null)
                Inst = this;
            pv = GetComponent<PhotonView>();
        }
    }

    void Update()
    {
        //사용자가 채팅창에 입력중일 땐(입력창이 활성화되어 있는 상태 - 선택되어 있는 상태), 캐릭터의 움직임을 제한한다.
        if (EventSystem.current.currentSelectedGameObject != null)
            if (EventSystem.current.currentSelectedGameObject == mainStageManager.chatting.inputField.gameObject)
                return;

        GetInput();
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed*Time.deltaTime;

        transform.LookAt(transform.position + moveVec);
    }

    void GetInput()
    {
        isSpace = Input.GetKeyDown(KeyCode.Space);
       if(!Input.anyKey)
        {
            animator.SetBool("IsWalk", false);
        }
         
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("IsWalk", true);
        }
       
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetBool("IsWalk", true);
        }
       
       
        if (Input.GetKeyDown(KeyCode.S))
        {
            animator.SetBool("IsWalk", true);
        }
       
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.SetBool("IsWalk", true);
        }

        //게임 상태가 Gathering일때만 스킬을 사용할 수 있도록 한다.
        if (mainStageManager.GetCurState() != MainStageState.Gathering) return;
        
        if (Input.GetMouseButtonDown(0))//마우스 좌클릭시
        {
            
            if (crush.cancrush == true)
            {
                crush.Is_crush = true;
                drop = true;
                animator.SetBool("Crush", true);

            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            drop = false;
            animator.SetBool("Crush", false);

        }

        if (Input.GetMouseButtonDown(1))//마우스 우클릭시
        {
            if (punch.canpunch == true)
            {
                punch.Is_Punch = true;

                animator.SetBool("Punch", true);
                GetComponent<TempHand>().AttackRPC();

            }

        }
        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("Punch", false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "item" && Input.GetKeyDown(KeyCode.Space))
        {
            canGetItem = other.gameObject;
            pickup.PickupItem(other.gameObject.GetComponent<ItemInfo>());
            Destroy(canGetItem);
            //canGetItem.SetActive(false);
        }
        if (other.tag == "Hand")
        {
            if (other.GetComponentInParent<TempHand>().isAttacking && other.GetComponentInParent<TempHand>().gameObject != gameObject)
            {
                animator.SetTrigger("Die");
                other.GetComponentInParent<TempHand>().isAttacking = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "item")
            canGetItem = null;
        if (other.tag == "Hand")
        {
            
        }
    }
}