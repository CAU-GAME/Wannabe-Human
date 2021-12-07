using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 ContorlLineController Ŭ����
- Control Line�� �������� �����ϴ� Ŭ�����̴�.
- Control Line
    - �÷��̾ Palette�� �׷��� Palette Item�� ������ ��, Palette Item ���� ��Ÿ�� ����ǥ���̴�.
    - �����̵�, ȸ��, ������ ������ �Է¹ް� �� ������ Control Line�� �ݿ��ȴ�.
- Control Line�� ����
    - Rotate Line : ��ư
    - Neck : Rotate Line�� Box������ ���� �κ�
    - Box : ��ư, Palette Item�� ���δ� �κ�
        - Right Up Scale Point      : ������ ��� ������ ���� ��ư
        - Left Up Scale Point       : ���� ��� ������ ���� ��ư
        - Right Down Scale Point    : ������ �ϴ� ������ ���� ��ư
        - Left Down Scale Point     : ���� �ϴ� ������ ���� ��ư

�ֿ���
- �����̵�, ȸ��, Ȯ��/��� ���� ������ ���콺 �巡�׸� ���� �Է¹޴´�.
- ���콺�� Ŭ���� ������ �������� ������ȯ�� �̷������.
- Control Line�� ���� �Է¹��� ������ Palette�� ���޵Ǿ� Palette Item�� �ݿ��ȴ�.
    - Palette�� ��ȯ������ ������ ���� ��ũ�� ��ǥ���� ������ǥ�� ���� �������־�� �Ѵ�.
- Box�� Ŭ���ϸ� �����̵� ������ �Է¹޴´�.
    - �����̵� ������ Box�� �߽��� �������� �Ѵ�.
    - �����̵� ������ ���콺 Ŭ�� ��ġ -> ���콺 �巡�� ��ġ�̴�.
- Rotate Line�� Ŭ���ϸ� ȸ�� ������ �Է¹޴´�.
    - ȸ���� ������ Box�� �߽��̴�.
    - (Box�� �߽�->���콺 Ŭ�� ��ġ)�� ������ �������� (Box�� �߽�->�巡�� ��ġ)�� ���а��� ���� ���� ȸ�������� �����Ѵ�.
- Scale Point�� Ŭ���ϸ� ������(Ȯ��/���) ������ �Է¹޴´�.
    - ������ ������ Box�� ũ�⸦ �������� �Ѵ�.
    - Scale Point���� Box�� �� �������� ��ġ�Ѵ�.
    - �������� ��ȭ�� ������ Scale Point�� ��ġ��ȭ������ ���� �����ȴ�.
    - ���콺 Ŭ���� ��ġ -> �巡�� ��ġ ������ ���� x��ǥ, y��ǥ ���̸� ���� ������ ������ ��ȯ��Ų��.
    - ������ �߽��� Box�� �߽����� �Ѵ�.
- Control Line�� Ȱ��ȭ�ϰų� ��Ȱ��ȭ�� �� �־�� �Ѵ�.
 */

//Drawing ���¸� �����Ѵ�.
public enum DrawingState { 
    None, //�������� ���� ��
    isTranslating, //�����̵�
    isRotating, //ȸ��
    isScaling//Ȯ��/���
}

public class ControlLineController : MonoBehaviour
{
    //Scale Point���� ��ġ�� �����ϰ�, ������ ������ �����ϱ� ���� ��ü
    public ScalePointsController scalePoints;

    //Control Line�� ������ҵ��� Rect Transform������ �����Ѵ�.
    public RectTransform box;
    public RectTransform neck;
    public RectTransform rotateLine;
    private RectTransform controlLine;//Control Line�� Rect Transform

    //Control Line�� ǥ�õ� ���� -> Drawn Item�� Palette Item�� ǥ�õǴ� �����̱⵵ �ϴ�.
    public RectTransform palette;

    //Control Line�� ����� ������ų �� ���Ǵ� �������̴�.
    private Vector3 startPos;//ó�� Control Line�� ��ġ, ��ũ�� ��ǥ
    private Vector3 clickPos;//ó�� ���콺�� Ŭ������ �� ������� ���콺 ��ġ, ��ũ�� ��ǥ
    private Vector3 dragPos;//�巡�� �ϴ� ���� ������� ���콺 ��ġ, ��ũ�� ��ǥ

    //������ ���� ������, �����Ͻ� �� Object�� �ʱⰪ�� �����ϱ� ���� ���ȴ�.
    //���콺�� Ŭ�������� �ʱ�ȭ�� ������
    private Vector2 startSize;//Ŭ������ ��, Box�� ũ�⸦ ������ ����    
    private Vector2 startNeckPos;//neck ��ġ, UI��ǥ
    private Vector2 startRotatelinePos;//rotate line ��ġ, UI��ǥ

    //Transform�� �ʱ�ȭ�� ���� ����
    //Control Line�� ������ �� ������ �ʱ�ȭ�� �ȴ�.
    private Vector2 initialBoxsize;
    private Vector2 initialNeckPos;
    private Vector2 initialRotatelinePos;
    private Vector3 initialPos;
    private Quaternion initialRot;

    //�� Transform�� ������ ��, � ������ ������ �� ������ ������ ����
    DrawingState curState = DrawingState.None;

    public RectTransform canvas;//ĵ������ RectTransform
    private Vector2 scaleScreenToUI;//UIȭ�� �ʺ�/��ũ��ȭ�� �ʺ� , UIȭ�� ����/��ũ��ȭ�� ����
    //position -> ���� ��ǥ�� ����, Input.mousePosition���� ���� �� �ִ� ��ũ�� ��ǥ�͵� ��ġ�� �����ϴ�.
    //UI���� ��ġ�� ��ũ�� ��ǥ���� ĵ������ �ʺ�� ���̸�ŭ �������� ���־�� �Ѵ�.

    //��ũ�� ��ǥ���� UI��ǥ�� ��ȯ�ϴ� �ٸ� ��� -> �� ����� �� �� �������� �� ����.
    /*
        CanvasScaler cs;
        RectTransform rt;

        float wRatio = Screen.width  / cs.referenceResolution.x;
        float hRatio = Screen.height / cs.referenceResolution.y;

        // ��� ������
        float ratio =
            wRatio * (1f - cs.matchWidthOrHeight) +
            hRatio * (cs.matchWidthOrHeight);

        // ���� ��ũ������ RectTransform�� ���� �ʺ�, ����
        float pixelWidth  = rt.rect.width  * ratio;
        float pixelHeight = rt.rect.height * ratio;
     *///�˻��ϴٰ� �쿬�� �߰��� �ڵ�, Scale����� �� ��ũ�� ��ǥ���� ��� �������� �������� �� �ñ��ߴµ� ���� �����־���.
    
    //https://rito15.github.io/posts/unity-punch-ui-image/
    //�̰� UI�� ���۶մ� �ǵ�, �״�� �����ߴµ� �� �ȵǴ� �� ����..
    

    void Awake()
    {
        //Palette�� �θ� ������Ʈ�� ����
        gameObject.transform.SetParent(palette);
        //RectTransform�� �ʱ�ȭ�Ѵ�.
        controlLine = GetComponent<RectTransform>();
        controlLine.localScale = Vector3.one;

        //�ʱ� ���콺�� ��ġ�� �ʱ�ȭ�Ѵ�.
        startPos = controlLine.anchoredPosition;
        dragPos = controlLine.anchoredPosition;

        //ĵ���� ũ��� Screenũ�⸦ �̿��� UI��ǥ�� ��ũ����ǥ ������ ������ ���Ѵ�.
        scaleScreenToUI = new Vector2(canvas.rect.width / Screen.width, canvas.rect.height / Screen.height);

        //�ʱⰪ ����
        initialPos = palette.position;
        initialRot = palette.rotation;
        initialBoxsize = box.sizeDelta;
        initialNeckPos = neck.anchoredPosition;
        initialRotatelinePos = rotateLine.anchoredPosition;
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//���콺�� Ŭ������ �� ������ - ��ȯ�� ���ذ��� �����Ѵ�.
        {
            //���� ��ġ ����
            startPos = controlLine.position;
            clickPos = Input.mousePosition;

            //���� Scale ����
            startSize = box.sizeDelta;
            startNeckPos = neck.anchoredPosition;
            startRotatelinePos = rotateLine.anchoredPosition;
        }
        else if (Input.GetMouseButton(0))//���콺�� Drag�� �� ������
        {
            dragPos = Input.mousePosition;//�巡���ϴ� ��ġ�� ���Ѵ�. ��ȯ�� ����� ������ ��

            switch (curState)//���� Drawing ���¸� Ȯ��
            {
                case DrawingState.None://����
                    break;

                case DrawingState.isTranslating://�����̵�
                    Translate(startPos, clickPos, dragPos);
                    break;

                case DrawingState.isRotating://ȸ��
                    Rotate(dragPos);
                    break;

                case DrawingState.isScaling://Ȯ��, ���
                    Vector3 temp = dragPos - clickPos;//��ŭ ���������� ���

                    //������ ������ �����ϱ� ���� ȸ������ ���Ѵ�.
                    float angle = controlLine.rotation.eulerAngles.z * Mathf.Deg2Rad;
                    
                    //ȸ���� �������� ���̺��͸� ��ȯ
                    Vector2 factor = new Vector2(Mathf.Cos(angle) * temp.x + Mathf.Sin(angle) * temp.y, -Mathf.Sin(angle) * temp.x + Mathf.Cos(angle) * temp.y);
                    
                    //���̺��͸� +,+�� ��ȯ
                    scalePoints.SetFactor(ref factor);

                    //UIȭ�鿡 �°� �� ����, factor�� Screen��ǥ�踦 �������� �� ����
                    //UIȭ�鿡���� ������ �ٲ���� �Ѵ�.
                    factor.x *= scaleScreenToUI.x;
                    factor.y *= scaleScreenToUI.y;

                    //���������� ������ ����
                    Scale(startSize, startNeckPos, startRotatelinePos, factor);
                    break;
            }
        }
        else if (Input.GetMouseButtonUp(0))//���콺 Drag�� ������ �� ������. Ŭ���� �������� ��
        {
            //����ؼ� Control Line�� �����Ǹ� �ȵǹǷ� �� �����ڸ� �ٽ� None���� �ٲ��ش�.
            curState = DrawingState.None;
        }
    }

    //�� ��ư���� Event�� �߻����� �� Drawing ���¸� �����ϱ� ���� �Լ�
    //�� ��������� Event Trigger - Pointer Down���� �̺�Ʈ�� �����Ѵ�.
    //Rotate Line�� Ŭ���ϸ� ȸ������ ���°� �ٲ��.
    public void OnPointerDownRotateLine()
    {
        curState = DrawingState.isRotating;
    }

    //Box�� ���õǸ� �����̵� ���·� �ٲ��.
    public void OnPointerDownBox()
    {
        curState = DrawingState.isTranslating;
    }

    //Scale Point�� ���õǸ� �����ϸ� ���·� �ٲ��.
    public void OnPointerDownScalePoints()
    {
        curState = DrawingState.isScaling;
    }

    //Control Line�� �����̵��� �����ϴ� �Լ�
    //startpos�� control line�� ���� ��ġ
    //clickpos�� ��ȭ���� ������ �Ǵ� ��ġ - ���콺 Ŭ�� ��ġ
    //dragpos�� ��ȭ���� ũ�Ⱑ �����Ǵ� ��ġ - ���콺 �巡�� ��ġ
    public void Translate(Vector3 startpos, Vector3 clickpos, Vector3 dragpos)
    {
        Vector3 dir = dragpos - clickpos;//���⺤�� - control line�� �̵��ϴ� ����
        controlLine.position = new Vector2(startpos.x + dir.x, startpos.y + dir.y);//���������� ������ŭ �̵�
    }

    //Control Line�� ȸ���� �����ϴ� �Լ�
    //target��ġ�� ���� �ٶ󺸰� �ȴ�. world position(screen position�� ��ǥ�� ����) UI��ǥ������ �ٸ�
    //ȸ�� �߽��� control line�� ��ġ
    public void Rotate(Vector3 target)
    {
        Vector3 vectorToTarget = target - new Vector3(controlLine.position.x, controlLine.position.y, 0f);//target - ȸ�� �߽�(control line) => ȸ�� ��� ���� ����
        controlLine.rotation = Quaternion.LookRotation(Vector3.forward, vectorToTarget);//�Ӹ��� �����̰� �Ѵ�. 2D�̱� ������
    }

    //Control Line�� Scale�� �����ϴ� �Լ�
    //������ : boxsize, neckpos, rotatelinepos - UI��ǥ �������� ũ��� ��ġ
    //��ȭ�� : uiFactor - UI��ǥ �������� ��ȭ��
    public void Scale(Vector2 boxsize, Vector2 neckpos, Vector2 rotatelinepos, Vector2 uiFactor)
    {
        if (boxsize.x + 2 * uiFactor.x < 0) return;
        if (boxsize.y + 2 * uiFactor.y < 0) return;
        box.sizeDelta = new Vector2(boxsize.x + 2 * uiFactor.x, boxsize.y + 2 * uiFactor.y);
        //�������� box�� ũ�⸦ �����ϴ� ������ ǥ���ߴ�. �ֳ��ϸ� box�׵θ��� ���� ���� �����ؾ� �Ǳ� ����
        //uiFactor�� scale point�� ������ ��Ÿ����. box�� �߽��� �������� �������� �Ǳ� ������ �¿찡 ���ÿ� ���ؾߵǼ� 2�踦 ���־���.

        scalePoints.PlacePoints(box.sizeDelta.x * 0.5f, box.sizeDelta.y * 0.5f);//boxũ�⿡ ���缭 scale point�� ���ġ�Ѵ�.
        rotateLine.anchoredPosition = new Vector2(0f, rotatelinepos.y + uiFactor.y);//rotate line�� ��ġ�� �ٲ� ��ġ�� ����
        neck.anchoredPosition = new Vector2(0f, neckpos.y + uiFactor.y);//neck�� ��ġ�� �ٲ� ��ġ�� ����
    }

    //Control Line�� �ʱ� ũ�⸦ �����ϴ� �Լ�
    //factor = end point - start point
    public void SetInitialSize(Vector2 point)
    {
        //Control Line�� ������ �� point�� ���Ѵ�. (��ũ�� ��ǥ��)
        Vector2 max = new Vector2(
            controlLine.position.x + box.rect.size.x / 2,
            controlLine.position.y + box.rect.size.y / 2);
        Vector2 factor = point - max;//������ ��ȭ���� ���Ѵ�.

        //UI ũ�⸦ �����ش�.
        //-> ������ UI��ǥ���̹Ƿ� ���⼭�� factor�� scaleScreenToUI�� ������ �ʿ䰡 ����. 

        //������ �ǽ�
        Scale(initialBoxsize, initialNeckPos, initialRotatelinePos, factor);
        
        //�ʱⰪ ����
        initialBoxsize = box.sizeDelta;
        initialNeckPos = neck.anchoredPosition;
        initialRotatelinePos = rotateLine.anchoredPosition;
    }

    //Control Line�� ��ġ�� �ʱ�ȭ�Ѵ�.
    public void InitializePosition()
    {
        controlLine.position = initialPos;
    }

    //Control Line�� ȸ������ �ʱ�ȭ�Ѵ�.
    public void InitializeRotation()
    {
        controlLine.rotation = initialRot;
    }

    //Control Line�� ������ ���� �ʱ�ȭ�Ѵ�.
    public void InitializeScale()
    {
        //controlLine.localScale = Vector3.one;
        Scale(initialBoxsize, initialNeckPos, initialRotatelinePos, Vector2.zero);
    }

    //Control Line�� ��ġ, ȸ����, �����ϰ��� ��� �ʱ�ȭ�Ѵ�.
    public void InitializeTransform()
    {
        InitializePosition();
        InitializeRotation();
        InitializeScale();
    }

    //Control Line�� ��Ȱ��ȭ�Ѵ�. -> Control Line�� �����.
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    //Control Line�� Ȱ��ȭ�Ѵ�. -> Control Line�� ǥ���Ѵ�.
    public void Draw()
    {
        gameObject.SetActive(true);
    }

    //Control Line�� ��ġ�� ��ȯ
    //Screen ��ǥ��
    public Vector2 GetPosition()
    {
        return controlLine.position;
    }

    //Control Line�� ��ġ�� World��ǥ��� ��ȯ�����ش�.
    public Vector3 GetWorldPosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(controlLine.position);
        worldPos = new Vector3(worldPos.x, worldPos.y, 0f);
        return worldPos;
    }

    //Control Line�� �����ϰ��� World��ǥ�迡�� ����� �� �ֵ��� ��ȯ��Ų��.
    public Vector3 GetScale()
    {
        Vector2 curSize = box.sizeDelta;//ũ�Ⱑ �󸶳� ���ߴ����� �˾ƾ� �Ѵ�. -> ���簪/�ʱⰪ
        Vector3 scale = new Vector3(curSize.x / initialBoxsize.x, curSize.y / initialBoxsize.y, 1f);
        return scale;
    }

    //Control Line�� ȸ������ World��ǥ�迡�� ����� �� �ֵ��� �ٲ��ش�.
    public Quaternion GetRotation()
    {
        Quaternion curRotation = controlLine.rotation;//ȸ������ �״�� ��밡��
        return curRotation;
    }

    //Control Line�� ���� ���¸� ��ȯ�Ѵ�.
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