using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainConCP : MonoBehaviour
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


}
