using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainCon2P : MonoBehaviour
{
    //ーーーーーーーーーー定数ーーーーーーーーーーーーーーーーーーーー
    //盤の縦（横）の数　8×8
    private const int boardMax = 8;

    //ターンと駒設置に使う度数
    private enum turnBW
    {
        Black = 0,
        White = 180,//駒の裏表の角度
        Not = 1
    }

    //盤の中央のマス目(最初に置いてある駒の設置場所)
    //黒
    private static readonly int[,] boardCenterB = {{ 3, 4 },
                                                   { 4, 3 } };
    //白
    private static readonly int[,] boardCenterW = {{ 3, 3 },
                                                   { 4, 4 }};

    //設置するオブジェクトのｙ座標
    private const float putY = 0.07f;

    //イベントが発動する条件の設置駒数の制限（min～max個）
    private const int eventTimingMin = 22;
    private const int eventTimingMax = 42;

    //駒を裏返す時のｙ座標の最高到達点
    private const float moveY = 0.5f;
    //駒を裏返す時のｙ座標移動の速度(上げるとき)
    private const float moveYSpeedUp = 4.0f;
    //駒を裏返す時のｙ座標移動の速度(下げるとき)
    private const float moveYSpeedDo = 4.5f;

    //駒を裏返す角度
    private const float moveRo = 180.0f;
    //駒を裏返す時の回転の速度
    private const float moveRoSpeed = 500.0f;

    //イベントの種類
    public enum eventName
    {
        Corner = 0,
        Change,
        Site,
        Not
    }
    //イベントの種類の数
    private const int eventQty = 3;
    //イベントの終了までの待機時間
    private const float eventStop = 4.0f;

    //エフェクトのｙ座標
    private const float effectY = 0.15f;
    //ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー


    private turnBW turn = turnBW.Black;//どっちのターンか
    private turnBW notTurn = turnBW.White;//ターンじゃない方

    //駒データ
    [SerializeField] private GameObject piece;
    private turnBW[,] pieceBoard = new turnBW[boardMax, boardMax];
    private GameObject[,] pieceBox = new GameObject[boardMax, boardMax];

    //置けるとこ
    [SerializeField] private GameObject show;//置ける場所
    private bool[,] overBoard = new bool[8, 8];
    private GameObject[,] overBox = new GameObject[8, 8];

    //駒の数
    private int countB = 0;
    private int countW = 0;
    //ゲームが終わるか
    private bool end = false;

    //処理中は駒を置けないようにする
    private bool canPut = true;

    //イベント
    private bool eventUse = false;
    private int eventTime = 0;
    private int[] site = { 10,10 };

    //エフェクト
    [Header("エフェクト")]
    [SerializeField] private GameObject cornerEffect;
    [SerializeField] private GameObject changeEffect;
    [SerializeField] private GameObject siteEffect;

    //UiCon
    UiCon uiCon;



    // Start is called before the first frame update
    void Start()
    {
        uiCon = GetComponent<UiCon>();

        //盤面データ初期
        for (int z = 0; z < boardMax; z++)
        {
            for (int x = 0; x < boardMax; x++)
            {

                if ((z == boardCenterB[0,0] && x == boardCenterB[0, 1]) || (z == boardCenterB[1, 0] && x == boardCenterB[1, 1]))
                { //初期黒
                    pieceBoard[z, x] = turnBW.Black;
                    pieceBox[z, x] = Instantiate(piece, new Vector3(x, putY, -z), Quaternion.Euler(0, 0, (int)turnBW.Black));

                }
                else if ((z == boardCenterW[0, 0] && x == boardCenterW[0, 1]) || (z == boardCenterW[1, 0] && x == boardCenterW[1, 1]))
                {//初期白
                    pieceBoard[z, x] = turnBW.White;
                    pieceBox[z, x] = Instantiate(piece, new Vector3(x, putY, -z), Quaternion.Euler(0, 0, (int)turnBW.White));

                }
                else
                {                      //初期配置以外は空
                    pieceBoard[z, x] = turnBW.Not;
                }


            }
        }

        eventTime = UnityEngine.Random.Range(eventTimingMin, eventTimingMax);//駒が(min～maxのランダム)個置かれた時にイベントを発動させる
        AccompanyTurn();
    }

    // Update is called once per frame
    void Update()
    {
        //設置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 13.0f))
        {
            if (Input.GetMouseButtonDown(0))
            {
                int z = (int)hit.transform.position.z * -1;
                int x = (int)hit.transform.position.x;

                PutPiece(z,x);
            }
        }

    }

    //駒を置く
    private void PutPiece(int z,int x) {
        if (overBoard[z, x] == true&&canPut)
        {//駒が置ける場所だったら
            pieceBox[z, x] = Instantiate(piece, new Vector3(x, 0.07f, -z), Quaternion.Euler(0, 0, (int)turn));
            pieceBoard[z, x] = turn;

            SearchReverse(z, x,true);//裏返す
            canPut = false;
        }
    }

    //ターン交代
    private void TurnChange()
    {
        if (turn == turnBW.Black)//黒から白に
        {
            turn = turnBW.White;
            notTurn = turnBW.Black;
        }
        else if (turn == turnBW.White)//白から黒に
        {
            turn = turnBW.Black;
            notTurn = turnBW.White;
        }

        SearchTurn();
    }

    //置ける場所がないときターンをもう一度交代する
    private void SearchTurn() {
        if (!CanPut(false))
        {
            if (!end)
            {
                end = true;
                TurnChange();
            }
            else
            {
                GameEnd();
            }
        }
        else//ターン確定後の処理
        {
            if (eventTime == countB + countW && !eventUse)
            {
                SearchEvent();
            }
            else {
                AccompanyTurn();
            }
            end = false;
        }
    }

    //ターン確定後の処理
    private void AccompanyTurn() {
        TurnUi();
        CountPiece();
        CanPut(true);
        canPut = true;
    }

    //ターンの色のui
    private void TurnUi()
    {
        if (turn == turnBW.Black)
        {
            uiCon.TurnBUi();

        }
        else if (turn == turnBW.White)
        {
            uiCon.TurnWUi();
        }

    }

    //今の駒の数
    private void CountPiece()
    {
        countB = 0;
        countW = 0;

        for (int z = 0; z < 8; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (pieceBoard[z, x] == turnBW.Black)
                {
                    countB++;

                    if (site[0] == x&&site[1] == z) {//イベント
                        countB += 3;
                    }
                }
                else if (pieceBoard[z, x] == turnBW.White)
                {
                    countW++;

                    if (site[0] == x && site[1] == z)//イベント
                    {
                        countW += 3;
                    }
                }
            }
        }

        uiCon.CountPieceUi(countB, countW);
    }

    //駒の設置が可能なマスか
    //overPutがfalseの時は置く場所があるか調べるだけの時、trueの時は置ける場所の表示も
    private bool CanPut(bool overPut)
    {
        overBoard = new bool[boardMax, boardMax];
        bool canPut = false;

        foreach (GameObject show in overBox)//全て消す
        {
            Destroy(show);
        }

        for (int z = 0; z < boardMax; z++)
        {
            for (int x = 0; x < boardMax; x++)
            {
                if (pieceBoard[z, x] == turnBW.Not)//何も置かれてないなら設置可能か調べる
                {
                    if (SearchReverse(z, x,false)) {//設置可能なら表示する
                        if (overPut)
                        {
                            overBoard[z, x] = true;
                            overBox[z, x] = Instantiate(show, new Vector3(x, putY, -z), Quaternion.Euler(90.0f, 0, 0));
                        }
                        canPut = true;
                    }
                }
                else
                {
                    overBoard[z, x] = false;
                }
            }
        }

        return canPut;
    }

    //置けるか調べる(reverse=false)または、実際裏返す場所をlistに入れる(reverse=true)
    private bool SearchReverse(int z, int x,bool reverse)
    {
        int[,] dir = new int[,] { { -1, 1 }, {  0, 1 }, {  1, 1 },
                                  { -1, 0 },            {  1, 0 },
                                  { -1,-1 }, {  0,-1 }, {  1,-1 }};//石の隣8方向{x,z}

        bool re = false;//return
        List<List<List<int>>> lists = new List<List<List<int>>>();//方向事のリスト

        for (int i = 0; i < dir.GetLength(0); i++)
        {
            int X = x;
            int Z = z;
            int xDir = dir[i,0];
            int zDir = dir[i,1];

            bool firstF = true;

            List<List<int>> overLists = new List<List<int>>();//一方向のリスト

            while (true)
            {
                X += xDir;
                Z += zDir;

                List<int> overList = new List<int>();//一マスのリスト

                if (X < 0 || 7 < X || Z < 0 || 7 < Z)//ボードを超えたらbreak
                {
                    break;
                }

                if (firstF)//最初の隣が相手の色か（一度だけ実行）
                {
                    if (pieceBoard[Z, X] != notTurn)//違うならbreak
                    {
                        break;
                    }
                    firstF = false;
                }

                if (pieceBoard[Z, X] == notTurn)//相手の色ならlistに入れる
                {
                    overList.Add(X);
                    overList.Add(Z);
                    overLists.Add(overList);

                }
                else if (pieceBoard[Z, X] == turn)//自分の色ならここまで裏返る
                {
                    if (reverse)//裏返す
                    {
                        lists.Add(overLists);
                    }
                    re = true;
                    break;
                }
                else
                {
                    break;
                }
            }
        }

        SearchReverse(lists);
        return re;
    }

    //裏返している間に駒を置けないように最後の駒が裏返ってからターン交代に進む
    //裏返す準備
    private void SearchReverse(List<List<List<int>>> lists) {
        for (int i = 0; i < lists.Count; i++)
        {
            if (i == lists.Count - 1)
            {
                StartCoroutine(Reverse(lists[i], true));
            }
            else
            {
                StartCoroutine(Reverse(lists[i], false));
            }
        }
    }

    //裏返す
    private IEnumerator Reverse(List<List<int>> overLists,bool last)
    {
        int x, z;
        for (int i = 0; i < overLists.Count; i++)
        {
            x = overLists[i][0];
            z = overLists[i][1];
            pieceBoard[z, x] = turn;

            if (last && i == overLists.Count - 1)
            {
                StartCoroutine(ReverseAnimation(z, x, true));
            }
            else
            {
                StartCoroutine(ReverseAnimation(z, x, false));
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    //裏返すアニメーション
    private IEnumerator ReverseAnimation(int z, int x,bool last)
    {
        float move = pieceBox[z, x].transform.position.y;
        while (move < moveY)
        {
            move += Time.deltaTime * moveYSpeedUp;
            if (move > moveY)
            {
                move = moveY;
            }
            pieceBox[z, x].transform.position = new Vector3(pieceBox[z, x].transform.position.x, move, pieceBox[z, x].transform.position.z);
            yield return null;
        }

        move = 0;
        while (move < 180)
        {
            float mo = Time.deltaTime * moveRoSpeed;
            move += mo;
            if (move > moveRo)
            {
                mo -= move - moveRo;
                move = moveRo;
            }
            pieceBox[z, x].transform.Rotate(new Vector3(0, 0,mo));
            yield return null;
        }

        move = pieceBox[z, x].transform.position.y;
        while (move > putY)
        {
            move -= Time.deltaTime * moveYSpeedDo;
            if (move < putY)
            {
                move = putY;
            }
            pieceBox[z, x].transform.position = new Vector3(pieceBox[z, x].transform.position.x, move, pieceBox[z, x].transform.position.z);
            yield return null;
        }

        //裏返す駒の中で最後の駒
        if (last) {
            TurnChange();
        }
    }

    //ゲーム終了、リザルト
    private void GameEnd()
    {
       uiCon.ResultUi(countB,countW);
    }

    //イベント
    //三つのイベントの中でどれが起こるか
    private void SearchEvent() {
        int cor = 0;
        if (!SearchCornerEvent()) {
            cor = 1;
        }
        eventName eventRan = (eventName)Enum.ToObject(typeof(eventName), UnityEngine.Random.Range(cor, eventQty));
        uiCon.EventUi(eventRan);
        switch (eventRan) {
            case eventName.Corner:
                CornerEvent();
                break;
            case eventName.Change:
                ChangeEvent();
                break;
            case eventName.Site:
                SiteEvent();
                break;
            default:
                break;
        }
        eventUse = true;
    }

    //CornerEventができるか
    private bool SearchCornerEvent() {
        for (int z = 0; z < boardMax; z += boardMax-1) {
            for (int x = 0; x < boardMax; x += boardMax - 1) {
                if (pieceBoard[z, x] != turnBW.Not)//四つ角に駒が置かれていたらtrueを返す
                {
                    return true;
                }
            }
        }
        return false;
    }

    //角に置かれた駒が消える
    private void CornerEvent() {
        for (int z = 0; z < boardMax; z += boardMax - 1)
        {
            for (int x = 0; x < boardMax; x += boardMax - 1)
            {
                if (pieceBoard[z, x] != turnBW.Not)
                {
                    Instantiate(cornerEffect, new Vector3(x, effectY, -z), Quaternion.identity);
                    pieceBoard[z, x] = turnBW.Not;
                    StartCoroutine(CornerEventAnimation(z, x));
                }
            }
        }

        StartCoroutine(EventEnd());
    }

    //CornerEventのアニメーション
    private IEnumerator CornerEventAnimation(int z,int x) {
        float move = pieceBox[z, x].transform.position.y;
        while (move < 10)
        {
            move += Time.deltaTime * moveYSpeedUp;
            pieceBox[z, x].transform.position = new Vector3(pieceBox[z, x].transform.position.x, move, pieceBox[z, x].transform.position.z);
            yield return null;
        }
        Destroy(pieceBox[z, x]);
    }


    //置かれている駒の色が全て逆になる
    private void ChangeEvent()
    {
        Instantiate(changeEffect);
        for (int z = 0; z < boardMax; z++){
            for (int x = 0; x < boardMax; x++){
                if (pieceBoard[z, x] == turnBW.Black)
                {
                    pieceBoard[z, x] = turnBW.White;
                    StartCoroutine(ReverseAnimation(z, x, false));
                }
                else if (pieceBoard[z, x] == turnBW.White) {
                    pieceBoard[z, x] = turnBW.Black;
                    StartCoroutine(ReverseAnimation(z, x, false));
                }
            }
        }

        StartCoroutine(EventEnd());
    }

    //まだ駒を置いていない一か所に最終的に駒が置かれていると得点のコマ数＋3される
    private void SiteEvent() {
        while (true) {
            int xRan = UnityEngine.Random.Range(0, boardMax);
            int zRan = UnityEngine.Random.Range(0, boardMax);
            if (pieceBoard[zRan, xRan] == turnBW.Not) {
                Instantiate(siteEffect, new Vector3(xRan, effectY, -zRan), Quaternion.identity);
                site[0] = xRan;
                site[1] = zRan;
                break;
            }
        }
        StartCoroutine(EventEnd());
    }

    private IEnumerator EventEnd() {
        yield return new WaitForSeconds(eventStop);
        TurnUi();
        CountPiece();
        CanPut(true);
        canPut = true;
    }
}
