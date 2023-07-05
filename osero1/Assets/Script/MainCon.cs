using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//オセロのみの機能のスクリプト

public class MainCon : MonoBehaviour
{
    private enum turnBW
    {
        Black = 0,
        White = 180, //pieceをひっくり返すので180
        Not = 2
    }
    private turnBW turn = turnBW.Black;//どっちのターンか
    private turnBW notTurn = turnBW.White;//ターンじゃない方

    //駒データ
    [SerializeField] private GameObject piece;
    private turnBW[,] piseBoard = new turnBW[8, 8];
    private GameObject[,] pieceBox = new GameObject[8, 8];

    //置けるとこ
    [SerializeField] private GameObject show;//置ける場所
    private bool[,] overBoard = new bool[8, 8];
    private GameObject[,] overBox = new GameObject[8, 8];

    private bool canPut = false;//置ける所が一か所でもあるか
    private bool end = false;//お互い置けなければ終わる

    //ひっくり返す駒リスト
    List<int> overListX = new List<int>();
    List<int> overListZ = new List<int>();

    //駒の数UI
    [SerializeField] private Text BUi;
    [SerializeField] private Text WUi;
    private int countB = 0;
    private int countW = 0;

    //ui
    [SerializeField] private Image CUi;
    [SerializeField] private Image CUi2;

    //リザルト
    [SerializeField] private GameObject risu;
    [SerializeField] private Text risuT;

    //裏返るアニメーションの最中に次に進まないように
    enum step {
        free,//駒を置ける時
        put,//駒を置いた後
        search,//裏返す駒を調べた後
        reverse//駒を裏返した後
    }
    private step stopCount = step.free;

    // Start is called before the first frame update
    void Start()
    {
        //盤面データ初期
        for (int i = 0; i < 8; i++){
            for (int j = 0; j < 8; j++){

                if ((i == 3 && j == 4) || (i == 4 && j == 3)){ //初期黒
                    piseBoard[i, j] = turnBW.Black;
                    pieceBox[i, j] = Instantiate(piece, new Vector3(j, 0.07f, -i), Quaternion.Euler(0, 0, 0));

                }
                else if ((i == 3 && j == 3) || (i == 4 && j == 4)) {//初期白
                    piseBoard[i, j] = turnBW.White;
                    pieceBox[i, j] = Instantiate(piece, new Vector3(j, 0.07f, -i), Quaternion.Euler(0, 0, 180));

                }
                else{                      //初期配置以外は空
                    piseBoard[i, j] = turnBW.Not;
                }

            }
        }

        CanPut();

        risu.gameObject.SetActive(false);//リザルトを非表示
    }

    // Update is called once per frame
    void Update()
    {
        if(stopCount == step.free) {

            //お互い置ける場所がないときゲームが終わる
            if (!canPut)
            {
                if (end == false){
                    TurnChange();
                    end = true;
                }
                else{
                    GameEnd();
                }
            }

            //駒設置
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 13.0f))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    int z = (int)hit.transform.position.z * -1;
                    int x = (int)hit.transform.position.x;

                    if (overBoard[z, x] == true){//駒が置ける場所だったら
                        stopCount = step.put;//ステップを進める
                        pieceBox[z, x] = Instantiate(piece, new Vector3(hit.transform.position.x, 0.07f, hit.transform.position.z), Quaternion.Euler(0, 0, (int)turn));
                        piseBoard[z, x] = turn;
                        SearchReverse(z, x);
                        end = false;
                    }
                }
            }
        }
        else if(stopCount == step.reverse) {//裏返るアニメーションが終わったら
            TurnChange();//ターン交代
            stopCount = step.free;
        }
    }

    //ターン交代
    private void TurnChange()
    {
        if (turn == turnBW.Black)//黒から白に
        {
            turn = turnBW.White;
            notTurn = turnBW.Black;

            CUi.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
            CUi2.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);

        }
        else if (turn == turnBW.White)//白から黒に
        {
            turn = turnBW.Black;
            notTurn = turnBW.White;

            CUi.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 255.0f);
            CUi2.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 255.0f);
        }

        CountPiece();
        CanPut();
    }

    //今の駒の数
    private void CountPiece()
    {
        countB = 0;
        countW = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (piseBoard[i, j] == turnBW.Black)
                {
                    countB++;
                }
                else if (piseBoard[i, j] == turnBW.White)
                {
                    countW++;
                }
            }
        }
        BUi.text = string.Format("{00:D2}", countB);
        WUi.text = string.Format("{00:D2}", countW);

    }

    //駒の設置が可能か
    private void CanPut()
    {
        overBoard = new bool[8, 8];
        canPut = false;

        foreach (GameObject show in overBox)
        {
            Destroy(show);
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (piseBoard[i, j] == turnBW.Not)//何も置かれてないなら設置可能か調べる
                {
                    SearchReverse(i, j);
                }
                else
                {
                    overBoard[i, j] = false;
                }

                if (overBoard[i, j])//設置可能なら表示する
                {
                    overBox[i, j] = Instantiate(show, new Vector3(j, 0.07f, -i), Quaternion.Euler(90.0f, 0, 0));
                    canPut = true;
                }
            }
        }
    }

    //置けるか調べるまたは、実際裏返す場所をlistに入れる
    private void SearchReverse(int Z, int X)
    {
        int[] _X = new int[] { -1, -1, 0, 1, 1,  1,  0, -1 };
        int[] _Z = new int[] {  0,  1, 1, 1, 0, -1, -1, -1 };//石の隣8方向


        for (int i = 0; i < _X.Length; i++)
        {
            int x = X;
            int z = Z;
            int _x = _X[i];
            int _z = _Z[i];

            bool firstF = true;
            bool isOver = false;

            overListX = new List<int>();
            overListZ = new List<int>();

            while (!isOver)
            {
                x += _x;
                z += _z;

                if (x < 0 || 7 < x || z < 0 || 7 < z)//ボードを超えたらbreak
                {
                    break;
                }

                if (firstF)//最初の隣が相手の色か（一度だけ実行）
                {
                    if (piseBoard[z, x] != notTurn)//違うならbreak
                    {
                        break;
                    }
                    firstF = false;
                }

                if (piseBoard[z, x] == notTurn)//相手の色ならlistに入れる
                {
                    overListX.Add(x);
                    overListZ.Add(z);

                }
                else if (piseBoard[z, x] == turn)//自分の色ならここまで裏返る
                {
                    overBoard[Z, X] = true;
                    if (stopCount == step.put)//裏返す
                    {
                        StartCoroutine(Reverse(overListX, overListZ));
                    }
                    isOver = true;//while抜ける

                }
                else
                {
                    break;
                }
            }
        }
        if(stopCount == step.put) {//ステップを進める
            stopCount = step.search;
        }

    }

    //裏返す
    private IEnumerator Reverse(List<int> listX, List<int> listZ)
    {
        int x, z;
        for (int i = 0; i < listX.Count; i++)
        {
            x = listX[i];
            z = listZ[i];
            piseBoard[z, x] = turn;
            StartCoroutine(ReverseAnimation(z,x));
            yield return new WaitForSeconds(0.1f);
        }
    }

    //裏返すアニメーション
    private IEnumerator ReverseAnimation(int z,int x) {
        while(pieceBox[z, x].transform.position.y < 0.5f) {
            pieceBox[z, x].transform.position += new Vector3(0,Time.deltaTime*4.0f,0);
            yield return null;
        }
        float ro = 0;
        while (ro < 180)
        {
            pieceBox[z, x].transform.Rotate(new Vector3(0, 0, Time.deltaTime * 500.0f));
            ro += Time.deltaTime * 500.0f;
            yield return null;
        }
        while (pieceBox[z, x].transform.position.y > 0.07f)
        {
            pieceBox[z, x].transform.position -= new Vector3(0, Time.deltaTime * 4.5f, 0);
            yield return null;
        }

        if(stopCount == step.search){//ステップを進める
            yield return 3;
            stopCount = step.reverse;
        }
    }

    //ゲーム終了、リザルト
    private void GameEnd()
    {
        if (countB > countW) {
            risuT.text = string.Format("BlackWIN");
        }
        else if (countB < countW)
        {
            risuT.text = string.Format("WhiteWIN");
        }
        else {
            risuT.text = string.Format("DRAW");
        }

        risu.gameObject.SetActive(true);
    }

}

