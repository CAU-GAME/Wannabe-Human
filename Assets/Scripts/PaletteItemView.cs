using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/*
 * PaletteItemView Ŭ����
- ��Ʈ��ũ�� Palette Item�� �ٸ� �÷��̾�� ��ǻ�Ϳ��� ������ ��,
����Palette Item���� �������� ������ ����ƮPalette Item���� ��ġ�� ������ �����ϰ� ���߱� ����
- ����Ʈ Palette Item�� �̹����� ����
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
