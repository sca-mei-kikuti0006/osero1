using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCon : MonoBehaviour
{
    //駒の数UI
    [Header("駒の数")]
    [SerializeField] private Text countBText;
    [SerializeField] private Text countWText;

    //ターンの色UI
    [Header("ターンの色")]
    [SerializeField] private Image turnC;
    [SerializeField] private Image turnC2;
    [SerializeField] private Image circleB;
    [SerializeField] private Image circleW;
    private Animator circleBAnim;
    private Animator circleWAnim;

    //リザルト
    [Header("リザルト")]
    [SerializeField] private GameObject resultCanvas;
    private Text resultText;

    //イベント
    [Header("イベント")]
    [SerializeField] private GameObject eventCanvas;
    private Text calloutBText;
    private Text calloutWText;

    // Start is called before the first frame update
    void Start()
    {

        circleB.gameObject.SetActive(false);//黒アニメーション
        circleBAnim = circleB.GetComponent<Animator>();

        circleW.gameObject.SetActive(false);//白アニメーション
        circleWAnim = circleW.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    //ターンが黒に変わった時のui
    public void TurnBUi()
    {
        circleB.gameObject.SetActive(true);
        circleBAnim.SetTrigger("circleBAnimStart");

        turnC.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 255.0f);
        turnC2.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 255.0f);
    }
    //ターンが白に変わった時のui
    public void TurnWUi()
    {
        circleW.gameObject.SetActive(true);
        circleWAnim.SetTrigger("circleWAnimStart");

        turnC.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
        turnC2.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
    }

    //駒の数ui
    public void CountPieceUi(int countB,int countW) {
        countBText.text = string.Format("{00:D2}", countB);
        countWText.text = string.Format("{00:D2}", countW);
    }

    
    //リザルト
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

    //イベント
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
                evB = "四隅の駒は\n排除～";
                evW = "角の駒は\n没収！";
                break;
            case MainCon.eventName.Change:
                evB = "置いた駒が\n逆になる～";
                evW = "逆転！";
                break;
            case MainCon.eventName.Site:
                evB = "キラキラな\nとこに置くと\n得点＋３～";
                evW = "キラキラな\nとこに置くと\nポイント＋３！";
                break;
            default:
                break;
        }
        calloutBText.text = string.Format(evB);
        calloutWText.text = string.Format(evW);

    }


}
