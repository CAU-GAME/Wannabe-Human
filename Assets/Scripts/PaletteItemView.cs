using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/*
 * PaletteItemView 클래스
- 네트워크용 Palette Item이 다른 플레이어들 컴퓨터에서 생성될 때,
로컬Palette Item들이 겹쳐지는 순서와 리모트Palette Item들이 겹치는 순서를 동일하게 맞추기 위해
- 리모트 Palette Item의 이미지를 설정
 */
public class PaletteItemView : MonoBehaviourPun
{
    private static int orderInLayer = 0;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
        orderInLayer += 1;
    }

    public void SetImageOnClient(string name)
    {
        photonView.RPC("ApplyImage", RpcTarget.Others, name);
    }

    [PunRPC]
    public void ApplyImage(string name)
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(GameManager.instance.itemImagePath + name);
    }
}
