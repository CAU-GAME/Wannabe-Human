using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TempHand : MonoBehaviourPunCallbacks
{
    public bool isAttacking = false;
    private PhotonView pv;
    
    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void AttackRPC()
    {
        pv.RPC("StartAttackToOtherPlayer", RpcTarget.All);
    }

    [PunRPC]
    public void StartAttackToOtherPlayer()
    {
        StartCoroutine(PlayAttackAnim());
    }

    IEnumerator PlayAttackAnim()
    {
        isAttacking = true;
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }
}
