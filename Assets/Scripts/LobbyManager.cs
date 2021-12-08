using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Button loginButton;//로그인 버튼, 클릭하면 룸 접속을 시도한다.
    public Text IDtext;//입력받은 아이디
    public Text ConnectionStatus;//접속상태를 나타내는 변수

    // Start is called before the first frame update
    void Start()
    {
        //설정한 정보로 마스터 서버에 접속 시도
        PhotonNetwork.ConnectUsingSettings();
        loginButton.interactable = false;//마스터 서버에 접속하기 전, 로그인 버튼을 비활성화한다. 클릭 이벤트 발생 못함.
        ConnectionStatus.text = "Connecting to Master Server...";//접속 시도 중임을 표시한다.

        // 마스터 서버에 먼저 접속한 후 룸에 접속한다.
        // 마스터 서버 -> 룸
    }

    // Update is called once per frame
    void Update()
    {

    }

    //로그인 버튼을 클릭했을 때 실행할 함수
    //Room접속을 시도한다.
    public void Connect()
    {
        if (IDtext.text.Equals(""))//아이디로 아무것도 입력하지 않았을 경우
        {
            return;
        }
        else
        {
            //닉네임 추가
            PhotonNetwork.LocalPlayer.NickName = IDtext.text;//로컬 플레이어의 닉네임을 입력받은 텍스트로 설정한다.
            loginButton.interactable = false;
            if (PhotonNetwork.IsConnected)//마스터 서버에 접속된 상태
            {
                ConnectionStatus.text = "connecting to room...";
                PhotonNetwork.JoinRandomRoom();//룸에 접속한다.
            }
            else//마스터 서버에 접속하지 못했다면 다시 접속을 시도한다.
            {
                ConnectionStatus.text = "Offline : failed to connect.\nreconnecting...";
                PhotonNetwork.ConnectUsingSettings();
            }
        }

    }

    public override void OnConnectedToMaster()//마스터 서버에 접속되면 실행되는 메서드
    {
        //마스터 서버에 접속이 되면 버튼을 다시 활성화한다.
        loginButton.interactable = true;
        ConnectionStatus.text = "Online : connected to master server";//마스터 서버에 연결되었음을 알려준다.
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //마스터 서버 접속에 실패하면 실행됨
        // 마스터 서버에 접속된 상태에서 접속이 끊긴 경우 실행됨
        loginButton.interactable = false;//접속이 끊겼으므로 버튼을 비활성화
        ConnectionStatus.text = "Offline : failed to connect.\nreconnecting...";//아직 오프라인 상태라고 알려줌
        PhotonNetwork.ConnectUsingSettings();//다시 마스터 서버로 접속 시도
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //랜덤 룸 접속에 실패한 경우 실행됨 - 참가 가능한 (빈 자리가 있는) 랜덤 룸이 없는 경우
        ConnectionStatus.text = "No empty room. creating new room...";// 빈방이 없음, 새로운 방 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });//최대 수용가능인원 4명으로 설정
        //null부분 생성할 룸의 이름

        //생성된 룸은 리슨 서버 방식으로 동작한다.
        // 룸을 생성한 클라이언트가 호스트 역할을 맡는다.
    }

    public override void OnJoinedRoom()
    {
        //룸 참가에 성공한 경우 자동으로 실행
        //기존에 있던 방에 참가하거나 PhotonNetwork.CreateRoom를 이용해 새로운 방을 만든 경우
        ConnectionStatus.text = "Success to join room";// 참가 성공!

        PhotonNetwork.LoadLevel("Main");//게임을 플레이할 씬을 로드한다.
        //LoadManager.LoadScene()이 아님.
        Debug.Log("my nick name : " + PhotonNetwork.LocalPlayer.NickName);
    }
}
