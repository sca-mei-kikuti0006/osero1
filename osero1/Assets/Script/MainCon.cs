using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class MainCon : MonoBehaviour
{
    //ーーーーーーーーーー定数ーーーーーーーーーーーーーーーーーーーー
    //盤の縦（横）の数　8×8
    private const int boardMax = 8;

    //ターンと駒設置に使う度数
    public enum turnBW
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

    //ゲームが始まるまでの時間
    private const float stopStart = 1.1f;

    //駒のアニメーション
    //次の駒を裏返すまでの時間
    private const float moveStop = 0.1f;
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

    //CPU
    //強さ
    public enum level 
    {
        Hard,
        Normal,
        Easy,
        Not
    }

    //各マスの評価値
    private static readonly int[,] squareValue = { { 20, 2,13, 8, 8,13, 2,20 },
                                                   {  2, 0, 5, 5, 5, 5, 0, 2 },
                                                   { 13, 5,10, 8, 8,10, 5,13 },
                                                   {  8, 5, 8, 8, 8, 8, 5, 8 },
                                                   {  8, 5, 8, 8, 8, 8, 5, 8 },
                                                   { 13, 5,10, 8, 8,10, 5,13 },
                                                   {  2, 0, 5, 5, 5, 5, 0, 2 },
                                                   { 20, 2,13, 8, 8,13, 2,20 }};
    private const int depth = 3;//何手先まで読むか（今回の手含め）

    //イベント
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
    private bool[,] overBoard = new bool[boardMax, boardMax];
    private GameObject[,] overBox = new GameObject[boardMax, boardMax];

    //ゲームが始めるまでのカウント
    private float stopCount = 0;
    private bool stopEnd = false;

    //駒の数
    private int countB = 0;
    private int countW = 0;
    //ゲームが終わるか
    private bool end = false;

    //処理中は駒を置けないようにする
    private bool canPutP = true;

    //裏返すアニメーション中は次に進まないようにする
    private int animCount = 0;

    //CPU
    //CPUの色
    public static turnBW cpuColor = turnBW.Not;
    //CPUの強さ
    public static level cpuLevel = level.Easy;

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


    //class
    private class CPUDate
    {
        public int value { get; set; }
        public int z { get; set; }
        public int x { get; set; }

    }

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
    }

    // Update is called once per frame
    void Update()
    {
        //ゲームがスタートするまでの待機時間
        if (!stopEnd) {
            stopCount += Time.deltaTime;
            if (stopCount > stopStart)
            {
                AccompanyTurn();
                stopEnd = true;
            }
        }


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
        if (overBoard[z, x] == true&&canPutP)
        {//駒が置ける場所だったら
            pieceBox[z, x] = Instantiate(piece, new Vector3(x, 0.07f, -z), Quaternion.Euler(0, 0, (int)turn));
            pieceBoard[z, x] = turn;

            SearchReverse(SearchReverse(z, x,pieceBoard));//裏返す
            canPutP = false;
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

        if (cpuColor == turn)//CPUのターンか
        {
            CanPut(false);
            CPUputPiece();
        }
        else {
            CanPut(true);
            canPutP = true;
        }
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
        bool canPut = false;
        overBoard = new bool[boardMax, boardMax];

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
                    if (SearchReverse(z, x,pieceBoard).Count > 0) {//設置可能なら表示する
                        overBoard[z, x] = true;
                        canPut = true;
                        if (overPut)
                        {
                            overBox[z, x] = Instantiate(show, new Vector3(x, putY, -z), Quaternion.Euler(90.0f, 0, 0));
                        }
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

    //一マスに対して、八方向の置ける場所をlistで返す(CPUにも使う)
    private List<List<List<int>>> SearchReverse(int z, int x,turnBW[,] board)
    {
        int[,] dir = new int[,] { { -1, 1 }, {  0, 1 }, {  1, 1 },
                                  { -1, 0 },            {  1, 0 },
                                  { -1,-1 }, {  0,-1 }, {  1,-1 }};//石の隣8方向{x,z}

        List<List<List<int>>> lists = new List<List<List<int>>>();//方向事のリスト

        for (int i = 0; i < dir.GetLength(0); i++)
        {
            int x_ = x;
            int z_ = z;
            int xDir = dir[i,0];
            int zDir = dir[i,1];

            bool firstF = true;

            List<List<int>> overLists = new List<List<int>>();//一方向のリスト

            while (true)
            {
                x_ += xDir;
                z_ += zDir;

                List<int> overList = new List<int>();//一マスのリスト

                if (x_ < 0 || 7 < x_ || z_ < 0 || 7 < z_)//ボードを超えたらbreak
                {
                    break;
                }

                if (firstF)//最初の隣が相手の色か（一度だけ実行）
                {
                    if (board[z_, x_] != notTurn)//違うならbreak
                    {
                        break;
                    }
                    firstF = false;
                }

                if (board[z_, x_] == notTurn)//相手の色ならlistに入れる
                {
                    overList.Add(x_);
                    overList.Add(z_);
                    overLists.Add(overList);

                }
                else if (board[z_, x_] == turn)//自分の色ならここまで裏返る
                {
                    lists.Add(overLists);
                    break;
                }
                else
                {
                    break;
                }
            }
        }
       
        return lists;
    }

    //裏返している間に駒を置けないように最後の駒が裏返ってからターン交代に進む
    //裏返す準備
    private void SearchReverse(List<List<List<int>>> lists) {

        for (int i = 0; i < lists.Count; i++)
        {
            StartCoroutine(Reverse(lists[i]));
        }
    }

    //裏返す
    private IEnumerator Reverse(List<List<int>> overLists)
    {
        if (animCount < 0) {
            animCount = 0;
        }
        animCount += overLists.Count;
        int x, z;
        for (int i = 0; i < overLists.Count; i++)
        {
            x = overLists[i][0];
            z = overLists[i][1];
            pieceBoard[z, x] = turn;

            StartCoroutine(ReverseAnimation(z, x));

            yield return new WaitForSeconds(moveStop);
        }
    }

    //裏返すアニメーション
    private IEnumerator ReverseAnimation(int z, int x)
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

        animCount--;
        //裏返す駒の中で最後の駒
        if (animCount == 0) {
            TurnChange();
        }
    }


    //CPU
    private void CPUputPiece() {
        int value = 0;

        List<CPUDate> valueList = new List<CPUDate>();

        for (int z_ = 0; z_ < boardMax; z_++)
        {
            for (int x_ = 0; x_ < boardMax; x_++)
            {
                if (overBoard[z_, x_]) {
                    value = CPUSearchValue(z_, x_);
                    valueList.Add(new CPUDate { value = value, z = z_, x = x_ });
                    
                }
            }
        }

        //CPUのレベルで置く場所を変える
        var sortValueList = valueList.OrderByDescending(v => v.value);
        valueList = new List<CPUDate>();
        foreach (var sort in sortValueList)
        {
            valueList.Add(new CPUDate { value = sort.value, z = sort.z, x = sort.x });
        }

        int levelCount = 0;
        switch (cpuLevel) {
            case level.Easy:
                if (valueList.Count - 1 < (int)level.Easy)
                {
                    levelCount = valueList.Count - 1;
                }
                else
                {
                    levelCount = (int)level.Easy;
                }
                break;
            case level.Normal:
                if (valueList.Count - 1 < (int)level.Normal)
                {
                    levelCount = valueList.Count - 1;
                }
                else
                {
                    levelCount = (int)level.Normal;
                }
                break;
            case level.Hard:
                levelCount = (int)level.Hard;
                break;
        }

        int z = valueList[levelCount].z;
        int x = valueList[levelCount].x;
 

        //置く
        pieceBox[z, x] = Instantiate(piece, new Vector3(x, 0.07f, -z), Quaternion.Euler(0, 0, (int)turn));
        pieceBoard[z, x] = turn;

        SearchReverse(SearchReverse(z, x, pieceBoard));//裏返す
    }


    //CPU
    private int CPUSearchValue(int z,int x) {
        turnBW[,] cpuBoard = new turnBW[boardMax, boardMax];//仮定の盤面
        bool[,] cpuOverBoard = new bool[boardMax, boardMax];

        Array.Copy(pieceBoard, 0, cpuBoard, 0, boardMax * boardMax);

        int value = 0;

        turnBW empty = turnBW.Not;

        int value2 = squareValue[z, x];
        int z_ = z;
        int x_ = x;

        int count = 0;
        do
        {
            if (count > 0)
            {
                //ターン交代
                empty = turn;
                turn = notTurn;
                notTurn = empty;

                //置ける場所調べ
                cpuOverBoard = CPUCanPut(cpuBoard);

                //置ける場所の中で評価がいちばん高いところを探す
                value2 = 0;
                for (int z_2 = 0; z_2 < boardMax; z_2++)
                {
                    for (int x_2 = 0; x_2 < boardMax; x_2++)
                    {
                        if (cpuOverBoard[z_2, x_2])
                        {
                            if (value2 < squareValue[z_2, x_2])
                            {
                                value2 = squareValue[z_2, x_2];
                                z_ = z_2;
                                x_ = x_2;
                            }

                        }
                    }
                }
            }

            //仮定の盤面を変更する
            //置く
            cpuBoard[z_, x_] = turn;
            if (count % 2 == 0)
            {
                value += value2;
            }
            else
            {
                value -= value2;
            }

            //Debug.Log("置いた座標 " + z_ + "," + x_);
            //Debug.Log("置いた評価 " + value2);

            //裏返す
            List<List<List<int>>> lists = SearchReverse(z_, x_, cpuBoard);
            for (int i = 0; i < lists.Count; i++)
            {
                for (int j = 0; j < lists[i].Count; j++)
                {
                    cpuBoard[lists[i][j][1], lists[i][j][0]] = turn;
                }
            }

            //Oll(cpuBoard);

            count++;
        } while (count < depth);


        //Debug.Log("評価 " + value + "\n ーーーーーーーーーーーーーーーーー");
        return value;
    }

    //CPUの置ける場所調べ
    private bool[,] CPUCanPut(turnBW[,] cpuBoard)
    {
        bool[,] CPUOverBoard = new bool[boardMax, boardMax];

        for (int z = 0; z < boardMax; z++)
        {
            for (int x = 0; x < boardMax; x++)
            {
                if (cpuBoard[z, x] == turnBW.Not)//何も置かれてないなら設置可能か調べる
                {
                    if (SearchReverse(z, x, cpuBoard).Count > 0)
                    {
                        CPUOverBoard[z, x] = true;
                    }
                }
                else
                {
                    CPUOverBoard[z, x] = false;
                }
            }
        }

        return CPUOverBoard;
    }


    //CPU動作確認用DebugLog
    //private void Oll(turnBW[,] cpuBoard)
    //{
    //    string log = "";
    //    for (int z = 0; z < boardMax; z++)
    //    {
    //        for (int x = 0; x < boardMax; x++)
    //        {
    //            if (cpuBoard[z, x] == turnBW.Black)
    //            {
    //                log += "● ";
    //            }
    //            else if (cpuBoard[z, x] == turnBW.White)
    //            {
    //                log += "〇 ";
    //            }
    //            else
    //            {
    //                log += "＿ ";
    //            }
    //        }
    //        log += "\n";
    //    }
    //    Debug.Log(log);
    //}


    //イベント
    //三つのイベントの中でどれが起こるか
    private void SearchEvent() {
        int cor = 0;
        if (!SearchCornerEvent())
        {//一つも角に駒が置かれていなかったらCornerEventは発動しない
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
                    StartCoroutine(ReverseAnimation(z, x));
                }
                else if (pieceBoard[z, x] == turnBW.White) {
                    pieceBoard[z, x] = turnBW.Black;
                    StartCoroutine(ReverseAnimation(z, x));
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

    //少し待った後にイベントを終わらせる
    private IEnumerator EventEnd() {
        yield return new WaitForSeconds(eventStop);
        AccompanyTurn();
    }

    //ゲーム終了、リザルト
    private void GameEnd()
    {
        uiCon.ResultUi(countB, countW);
    }
}
