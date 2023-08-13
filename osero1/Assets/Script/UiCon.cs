using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCon : MonoBehaviour
{
    //��̐�UI
    [Header("��̐�")]
    [SerializeField] private Text countBText;
    [SerializeField] private Text countWText;

    //�^�[���̐FUI
    [Header("�^�[���̐F")]
    [SerializeField] private Image turnC;
    [SerializeField] private Image turnC2;
    [SerializeField] private Image circleB;
    [SerializeField] private Image circleW;
    private Animator circleBAnim;
    private Animator circleWAnim;

    //���U���g
    [Header("���U���g")]
    [SerializeField] private GameObject resultCanvas;
    private Text resultText;

    //�C�x���g
    [Header("�C�x���g")]
    [SerializeField] private GameObject eventCanvas;
    private Text calloutBText;
    private Text calloutWText;

    // Start is called before the first frame update
    void Start()
    {

        circleB.gameObject.SetActive(false);//���A�j���[�V����
        circleBAnim = circleB.GetComponent<Animator>();

        circleW.gameObject.SetActive(false);//���A�j���[�V����
        circleWAnim = circleW.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    //�^�[�������ɕς��������ui
    public void TurnBUi()
    {
        circleB.gameObject.SetActive(true);
        circleBAnim.SetTrigger("circleBAnimStart");

        turnC.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 255.0f);
        turnC2.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 255.0f);
    }
    //�^�[�������ɕς��������ui
    public void TurnWUi()
    {
        circleW.gameObject.SetActive(true);
        circleWAnim.SetTrigger("circleWAnimStart");

        turnC.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
        turnC2.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
    }

    //��̐�ui
    public void CountPieceUi(int countB,int countW) {
        countBText.text = string.Format("{00:D2}", countB);
        countWText.text = string.Format("{00:D2}", countW);
    }

    
    //���U���g
    public void ResultUi(int countB, int countW)
    {
        GameObject resultCan = Instantiate(resultCanvas);

        GameObject reT = resultCan.transform.Find("Result/ResultText").gameObject;
        resultText = reT.GetComponent<Text>();

        if (countB > countW)
        {
            resultText.text = string.Format("BlackWIN");
            circleB.gameObject.SetActive(true);
            circleBAnim.SetTrigger("circleBAnimStart");
        }
        else if (countB < countW)
        {
            resultText.text = string.Format("WhiteWIN");
            circleW.gameObject.SetActive(true);
            circleWAnim.SetTrigger("circleWAnimStart");
        }
        else
        {
            resultText.text = string.Format("DRAW");
            circleB.gameObject.SetActive(true);
            circleBAnim.SetTrigger("circleBAnimStart");
            circleW.gameObject.SetActive(true);
            circleWAnim.SetTrigger("circleWAnimStart");

        }

        
    }

    //�C�x���g
    public void EventUi(MainCon.eventName eventRan)
    {
        GameObject eventCan = Instantiate(eventCanvas); 

        GameObject evBT = eventCan.transform.Find("CalloutB/CalloutBText").gameObject;
        calloutBText = evBT.GetComponent<Text>();
        GameObject evWT = eventCan.transform.Find("CalloutW/CalloutWText").gameObject;
        calloutWText = evWT.GetComponent<Text>();


        string evB = "";
        string evW = "";
        switch (eventRan)
        {
            case MainCon.eventName.Corner:
                evB = "�l���̋��\n�r���`";
                evW = "�p�̋��\n�v���I";
                break;
            case MainCon.eventName.Change:
                evB = "�u�����\n�t�ɂȂ�`";
                evW = "�t�]�I";
                break;
            case MainCon.eventName.Site:
                evB = "�L���L����\n�Ƃ��ɒu����\n���_�{�R�`";
                evW = "�L���L����\n�Ƃ��ɒu����\n�|�C���g�{�R�I";
                break;
            default:
                break;
        }
        calloutBText.text = string.Format(evB);
        calloutWText.text = string.Format(evW);

    }


}
