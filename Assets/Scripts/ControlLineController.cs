using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 ContorlLineController 클래스
- Control Line의 움직임을 제어하는 클래스이다.
- Control Line
    - 플레이어가 Palette에 그려진 Palette Item을 편집할 때, Palette Item 위에 나타날 제어표시이다.
    - 평행이동, 회전, 스케일 정보를 입력받고 각 정보가 Control Line에 반영된다.
- Control Line의 구조
    - Rotate Line : 버튼
    - Neck : Rotate Line과 Box사이의 연결 부분
    - Box : 버튼, Palette Item을 감싸는 부분
        - Right Up Scale Point      : 오른쪽 상단 스케일 조정 버튼
        - Left Up Scale Point       : 왼쪽 상단 스케일 조정 버튼
        - Right Down Scale Point    : 오른쪽 하단 스케일 조정 버튼
        - Left Down Scale Point     : 왼쪽 하단 스케일 조정 버튼

주요기능
- 평행이동, 회전, 확대/축소 변경 정보는 마우스 드래그를 통해 입력받는다.
- 마우스가 클릭한 시점을 기준으로 도형변환이 이루어진다.
- Control Line을 통해 입력받은 정보는 Palette에 전달되어 Palette Item에 반영된다.
    - Palette에 변환정보를 전달할 때는 스크린 좌표에서 월드좌표로 값을 변경해주어야 한다.
- Box를 클릭하면 평행이동 정보를 입력받는다.
    - 평행이동 정보는 Box의 중심을 기준으로 한다.
    - 평행이동 변위는 마우스 클릭 위치 -> 마우스 드래그 위치이다.
- Rotate Line을 클릭하면 회전 정보를 입력받는다.
    - 회전의 기준은 Box의 중심이다.
    - (Box의 중심->마우스 클릭 위치)의 선분을 기준으로 (Box의 중심->드래그 위치)의 선분과의 각을 구해 회전각도를 설정한다.
- Scale Point를 클릭하면 스케일(확대/축소) 정보를 입력받는다.
    - 스케일 정보는 Box의 크기를 기준으로 한다.
    - Scale Point들은 Box의 각 꼭지점에 위치한다.
    - 스케일의 변화된 정도는 Scale Point의 위치변화정도를 통해 설정된다.
    - 마우스 클릭시 위치 -> 드래그 위치 사이의 각각 x좌표, y좌표 차이를 구해 스케일 정보를 변환시킨다.
    - 스케일 중심은 Box의 중심으로 한다.
- Control Line을 활성화하거나 비활성화할 수 있어야 한다.
 */

//Drawing 상태를 저장한다.
public enum DrawingState { 
    None, //조작하지 않을 때
    isTranslating, //평행이동
    isRotating, //회전
    isScaling//확대/축소
}

public class ControlLineController : MonoBehaviour
{
    //Scale Point들의 위치를 설정하고, 스케일 방향을 조정하기 위한 객체
    public ScalePointsController scalePoints;

    //Control Line의 구성요소들의 Rect Transform정보를 저장한다.
    public RectTransform box;
    public RectTransform neck;
    public RectTransform rotateLine;
    private RectTransform controlLine;//Control Line의 Rect Transform

    //Control Line이 표시될 공간 -> Drawn Item의 Palette Item가 표시되는 공간이기도 하다.
    public RectTransform palette;

    //Control Line의 모양을 변형시킬 때 사용되는 변수들이다.
    private Vector3 startPos;//처음 Control Line의 위치, 스크린 좌표
    private Vector3 clickPos;//처음 마우스를 클릭했을 때 얻어지는 마우스 위치, 스크린 좌표
    private Vector3 dragPos;//드래그 하는 동안 얻어지는 마우스 위치, 스크린 좌표

    //스케일 관련 변수들, 스케일시 각 Object의 초기값을 저장하기 위해 사용된다.
    //마우스를 클릭했을때 초기화될 변수들
    private Vector2 startSize;//클릭했을 때, Box의 크기를 저장할 변수    
    private Vector2 startNeckPos;//neck 위치, UI좌표
    private Vector2 startRotatelinePos;//rotate line 위치, UI좌표

    //Transform의 초기화를 위한 변수
    //Control Line이 생성될 때 값으로 초기화가 된다.
    private Vector2 initialBoxsize;
    private Vector2 initialNeckPos;
    private Vector2 initialRotatelinePos;
    private Vector3 initialPos;
    private Quaternion initialRot;

    //각 Transform을 적용할 때, 어떤 종류의 변형을 할 것인지 구분할 변수
    DrawingState curState = DrawingState.None;

    public RectTransform canvas;//캔버스의 RectTransform
    private Vector2 scaleScreenToUI;//UI화면 너비/스크린화면 너비 , UI화면 높이/스크린화면 높이
    //position -> 월드 좌표와 동일, Input.mousePosition으로 얻을 수 있는 스크린 좌표와도 위치가 동일하다.
    //UI내의 위치는 스크린 좌표에서 캔버스의 너비와 높이만큼 스케일을 해주어야 한다.

    //스크린 좌표에서 UI좌표를 변환하는 다른 방법 -> 이 방법이 좀 더 원리적인 것 같다.
    /*
        CanvasScaler cs;
        RectTransform rt;

        float wRatio = Screen.width  / cs.referenceResolution.x;
        float hRatio = Screen.height / cs.referenceResolution.y;

        // 결과 비율값
        float ratio =
            wRatio * (1f - cs.matchWidthOrHeight) +
            hRatio * (cs.matchWidthOrHeight);

        // 현재 스크린에서 RectTransform의 실제 너비, 높이
        float pixelWidth  = rt.rect.width  * ratio;
        float pixelHeight = rt.rect.height * ratio;
     *///검색하다가 우연히 발견한 코드, Scale모드일 때 스크린 좌표에서 어떻게 비율값이 정해지는 지 궁금했는데 여기 나와있었다.
    
    //https://rito15.github.io/posts/unity-punch-ui-image/
    //이건 UI에 구멍뚫는 건데, 그대로 따라했는데 잘 안되는 것 같다..
    

    void Awake()
    {
        //Palette를 부모 오브젝트로 설정
        gameObject.transform.SetParent(palette);
        //RectTransform을 초기화한다.
        controlLine = GetComponent<RectTransform>();
        controlLine.localScale = Vector3.one;

        //초기 마우스의 위치를 초기화한다.
        startPos = controlLine.anchoredPosition;
        dragPos = controlLine.anchoredPosition;

        //캔버스 크기와 Screen크기를 이용해 UI좌표와 스크린좌표 사이의 비율을 구한다.
        scaleScreenToUI = new Vector2(canvas.rect.width / Screen.width, canvas.rect.height / Screen.height);

        //초기값 설정
        initialPos = palette.position;
        initialRot = palette.rotation;
        initialBoxsize = box.sizeDelta;
        initialNeckPos = neck.anchoredPosition;
        initialRotatelinePos = rotateLine.anchoredPosition;
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//마우스를 클릭했을 때 동작함 - 변환의 기준값을 설정한다.
        {
            //시작 위치 세팅
            startPos = controlLine.position;
            clickPos = Input.mousePosition;

            //시작 Scale 세팅
            startSize = box.sizeDelta;
            startNeckPos = neck.anchoredPosition;
            startRotatelinePos = rotateLine.anchoredPosition;
        }
        else if (Input.GetMouseButton(0))//마우스를 Drag할 때 동작함
        {
            dragPos = Input.mousePosition;//드래그하는 위치를 구한다. 변환의 결과를 결정할 값

            switch (curState)//현재 Drawing 상태를 확인
            {
                case DrawingState.None://평상시
                    break;

                case DrawingState.isTranslating://평행이동
                    Translate(startPos, clickPos, dragPos);
                    break;

                case DrawingState.isRotating://회전
                    Rotate(dragPos);
                    break;

                case DrawingState.isScaling://확대, 축소
                    Vector3 temp = dragPos - clickPos;//얼만큼 움직였는지 계산

                    //스케일 방향을 조정하기 위해 회전각을 구한다.
                    float angle = controlLine.rotation.eulerAngles.z * Mathf.Deg2Rad;
                    
                    //회전된 방향으로 차이벡터를 변환
                    Vector2 factor = new Vector2(Mathf.Cos(angle) * temp.x + Mathf.Sin(angle) * temp.y, -Mathf.Sin(angle) * temp.x + Mathf.Cos(angle) * temp.y);
                    
                    //차이벡터를 +,+로 변환
                    scalePoints.SetFactor(ref factor);

                    //UI화면에 맞게 값 조정, factor는 Screen좌표계를 바탕으로 한 변위
                    //UI화면에서의 변위로 바꿔줘야 한다.
                    factor.x *= scaleScreenToUI.x;
                    factor.y *= scaleScreenToUI.y;

                    //최종적으로 스케일 진행
                    Scale(startSize, startNeckPos, startRotatelinePos, factor);
                    break;
            }
        }
        else if (Input.GetMouseButtonUp(0))//마우스 Drag를 끝냈을 때 동작함. 클릭이 떨어졌을 때
        {
            //계속해서 Control Line이 변형되면 안되므로 각 구분자를 다시 None으로 바꿔준다.
            curState = DrawingState.None;
        }
    }

    //각 버튼에서 Event가 발생했을 때 Drawing 상태를 세팅하기 위한 함수
    //각 구성요소의 Event Trigger - Pointer Down으로 이벤트를 감지한다.
    //Rotate Line을 클릭하면 회전으로 상태가 바뀐다.
    public void OnPointerDownRotateLine()
    {
        curState = DrawingState.isRotating;
    }

    //Box가 선택되면 평행이동 상태로 바뀐다.
    public void OnPointerDownBox()
    {
        curState = DrawingState.isTranslating;
    }

    //Scale Point가 선택되면 스케일링 상태로 바뀐다.
    public void OnPointerDownScalePoints()
    {
        curState = DrawingState.isScaling;
    }

    //Control Line의 평행이동을 진행하는 함수
    //startpos는 control line의 시작 위치
    //clickpos는 변화량의 기준이 되는 위치 - 마우스 클릭 위치
    //dragpos는 변화량의 크기가 결정되는 위치 - 마우스 드래그 위치
    public void Translate(Vector3 startpos, Vector3 clickpos, Vector3 dragpos)
    {
        Vector3 dir = dragpos - clickpos;//방향벡터 - control line이 이동하는 변위
        controlLine.position = new Vector2(startpos.x + dir.x, startpos.y + dir.y);//시작점에서 변위만큼 이동
    }

    //Control Line의 회전을 진행하는 함수
    //target위치를 향해 바라보게 된다. world position(screen position과 좌표값 동일) UI좌표값과는 다름
    //회전 중심은 control line의 위치
    public void Rotate(Vector3 target)
    {
        Vector3 vectorToTarget = target - new Vector3(controlLine.position.x, controlLine.position.y, 0f);//target - 회전 중심(control line) => 회전 결과 벡터 생성
        controlLine.rotation = Quaternion.LookRotation(Vector3.forward, vectorToTarget);//머리만 움직이게 한다. 2D이기 때문에
    }

    //Control Line의 Scale을 진행하는 함수
    //기준점 : boxsize, neckpos, rotatelinepos - UI좌표 내에서의 크기와 위치
    //변화량 : uiFactor - UI좌표 내에서의 변화량
    public void Scale(Vector2 boxsize, Vector2 neckpos, Vector2 rotatelinepos, Vector2 uiFactor)
    {
        if (boxsize.x + 2 * uiFactor.x < 0) return;
        if (boxsize.y + 2 * uiFactor.y < 0) return;
        box.sizeDelta = new Vector2(boxsize.x + 2 * uiFactor.x, boxsize.y + 2 * uiFactor.y);
        //스케일은 box의 크기를 조절하는 것으로 표현했다. 왜냐하면 box테두리의 변의 폭이 일정해야 되기 때문
        //uiFactor는 scale point의 변위를 나타낸다. box의 중심을 기준으로 스케일이 되기 때문에 좌우가 동시에 변해야되서 2배를 해주었다.

        scalePoints.PlacePoints(box.sizeDelta.x * 0.5f, box.sizeDelta.y * 0.5f);//box크기에 맞춰서 scale point를 재배치한다.
        rotateLine.anchoredPosition = new Vector2(0f, rotatelinepos.y + uiFactor.y);//rotate line의 위치도 바뀐 위치로 조정
        neck.anchoredPosition = new Vector2(0f, neckpos.y + uiFactor.y);//neck의 위치도 바뀐 위치로 조정
    }

    //Control Line의 초기 크기를 설정하는 함수
    //factor = end point - start point
    public void SetInitialSize(Vector2 point)
    {
        //Control Line의 오른쪽 위 point를 구한다. (스크린 좌표계)
        Vector2 max = new Vector2(
            controlLine.position.x + box.rect.size.x / 2,
            controlLine.position.y + box.rect.size.y / 2);
        Vector2 factor = point - max;//스케일 변화량을 구한다.

        //UI 크기를 더해준다.
        //-> 변위가 UI좌표값이므로 여기서는 factor에 scaleScreenToUI를 곱해줄 필요가 없다. 

        //스케일 실시
        Scale(initialBoxsize, initialNeckPos, initialRotatelinePos, factor);
        
        //초기값 설정
        initialBoxsize = box.sizeDelta;
        initialNeckPos = neck.anchoredPosition;
        initialRotatelinePos = rotateLine.anchoredPosition;
    }

    //Control Line의 위치를 초기화한다.
    public void InitializePosition()
    {
        controlLine.position = initialPos;
    }

    //Control Line의 회전값을 초기화한다.
    public void InitializeRotation()
    {
        controlLine.rotation = initialRot;
    }

    //Control Line의 스케일 값을 초기화한다.
    public void InitializeScale()
    {
        //controlLine.localScale = Vector3.one;
        Scale(initialBoxsize, initialNeckPos, initialRotatelinePos, Vector2.zero);
    }

    //Control Line의 위치, 회전값, 스케일값을 모두 초기화한다.
    public void InitializeTransform()
    {
        InitializePosition();
        InitializeRotation();
        InitializeScale();
    }

    //Control Line을 비활성화한다. -> Control Line을 감춘다.
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    //Control Line을 활성화한다. -> Control Line을 표시한다.
    public void Draw()
    {
        gameObject.SetActive(true);
    }

    //Control Line의 위치를 반환
    //Screen 좌표값
    public Vector2 GetPosition()
    {
        return controlLine.position;
    }

    //Control Line의 위치를 World좌표계로 변환시켜준다.
    public Vector3 GetWorldPosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(controlLine.position);
        worldPos = new Vector3(worldPos.x, worldPos.y, 0f);
        return worldPos;
    }

    //Control Line의 스케일값을 World좌표계에서 사용할 수 있도록 변환시킨다.
    public Vector3 GetScale()
    {
        Vector2 curSize = box.sizeDelta;//크기가 얼마나 변했는지를 알아야 한다. -> 현재값/초기값
        Vector3 scale = new Vector3(curSize.x / initialBoxsize.x, curSize.y / initialBoxsize.y, 1f);
        return scale;
    }

    //Control Line의 회전값을 World좌표계에서 사용할 수 있도록 바꿔준다.
    public Quaternion GetRotation()
    {
        Quaternion curRotation = controlLine.rotation;//회전값은 그대로 사용가능
        return curRotation;
    }

    //Control Line의 현재 상태를 반환한다.
    public DrawingState GetDrawingState()
    {
        return curState;
    }

    public Vector2 GetScaleScreenToUI(Vector2 screen)
    {
        Vector2 ui = new Vector2(screen.x * scaleScreenToUI.x, screen.y * scaleScreenToUI.y);
        return ui;
    }
}