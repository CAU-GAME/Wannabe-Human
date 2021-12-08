using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Button loginButton;//�α��� ��ư, Ŭ���ϸ� �� ������ �õ��Ѵ�.
    public Text IDtext;//�Է¹��� ���̵�
    public Text ConnectionStatus;//���ӻ��¸� ��Ÿ���� ����

    // Start is called before the first frame update
    void Start()
    {
        //������ ������ ������ ������ ���� �õ�
        PhotonNetwork.ConnectUsingSettings();
        loginButton.interactable = false;//������ ������ �����ϱ� ��, �α��� ��ư�� ��Ȱ��ȭ�Ѵ�. Ŭ�� �̺�Ʈ �߻� ����.
        ConnectionStatus.text = "Connecting to Master Server...";//���� �õ� ������ ǥ���Ѵ�.

        // ������ ������ ���� ������ �� �뿡 �����Ѵ�.
        // ������ ���� -> ��
    }

    // Update is called once per frame
    void Update()
    {

    }

    //�α��� ��ư�� Ŭ������ �� ������ �Լ�
    //Room������ �õ��Ѵ�.
    public void Connect()
    {
        if (IDtext.text.Equals(""))//���̵�� �ƹ��͵� �Է����� �ʾ��� ���
        {
            return;
        }
        else
        {
            //�г��� �߰�
            PhotonNetwork.LocalPlayer.NickName = IDtext.text;//���� �÷��̾��� �г����� �Է¹��� �ؽ�Ʈ�� �����Ѵ�.
            loginButton.interactable = false;
            if (PhotonNetwork.IsConnected)//������ ������ ���ӵ� ����
            {
                ConnectionStatus.text = "connecting to room...";
                PhotonNetwork.JoinRandomRoom();//�뿡 �����Ѵ�.
            }
            else//������ ������ �������� ���ߴٸ� �ٽ� ������ �õ��Ѵ�.
            {
                ConnectionStatus.text = "Offline : failed to connect.\nreconnecting...";
                PhotonNetwork.ConnectUsingSettings();
            }
        }

    }

    public override void OnConnectedToMaster()//������ ������ ���ӵǸ� ����Ǵ� �޼���
    {
        //������ ������ ������ �Ǹ� ��ư�� �ٽ� Ȱ��ȭ�Ѵ�.
        loginButton.interactable = true;
        ConnectionStatus.text = "Online : connected to master server";//������ ������ ����Ǿ����� �˷��ش�.
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //������ ���� ���ӿ� �����ϸ� �����
        // ������ ������ ���ӵ� ���¿��� ������ ���� ��� �����
        loginButton.interactable = false;//������ �������Ƿ� ��ư�� ��Ȱ��ȭ
        ConnectionStatus.text = "Offline : failed to connect.\nreconnecting...";//���� �������� ���¶�� �˷���
        PhotonNetwork.ConnectUsingSettings();//�ٽ� ������ ������ ���� �õ�
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //���� �� ���ӿ� ������ ��� ����� - ���� ������ (�� �ڸ��� �ִ�) ���� ���� ���� ���
        ConnectionStatus.text = "No empty room. creating new room...";// ����� ����, ���ο� �� ����
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });//�ִ� ���밡���ο� 4������ ����
        //null�κ� ������ ���� �̸�

        //������ ���� ���� ���� ������� �����Ѵ�.
        // ���� ������ Ŭ���̾�Ʈ�� ȣ��Ʈ ������ �ô´�.
    }

    public override void OnJoinedRoom()
    {
        //�� ������ ������ ��� �ڵ����� ����
        //������ �ִ� �濡 �����ϰų� PhotonNetwork.CreateRoom�� �̿��� ���ο� ���� ���� ���
        ConnectionStatus.text = "Success to join room";// ���� ����!

        PhotonNetwork.LoadLevel("Main");//������ �÷����� ���� �ε��Ѵ�.
        //LoadManager.LoadScene()�� �ƴ�.
        Debug.Log("my nick name : " + PhotonNetwork.LocalPlayer.NickName);
    }
}
