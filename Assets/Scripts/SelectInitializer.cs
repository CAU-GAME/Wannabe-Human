using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 SelectInitializer 클래스
- 사용자가 컨트롤 라인을 클릭하지 않고 다른 곳을 클릭했을때 Drawn Item의 선택을 해제하기위한 클래스이다.
- 예를 들어 제시어/제시이미지를 클릭하거나, 채팅창이나 참가자들 정보가 나오는 부분을 클릭하면
  현재 선택된 Drawn Item의 선택을 해제시킨다.

- Canvas Object에 부착되어 있다.
 */
public class SelectInitializer : MonoBehaviour, IPointerDownHandler
{
    public DrawnItemsManager drawnItems;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        drawnItems.palette.InitializeSelection();
        drawnItems.InitializeSelection();
    }
}
