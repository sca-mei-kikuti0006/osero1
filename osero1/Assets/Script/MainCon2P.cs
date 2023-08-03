using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainCon2P : MonoBehaviour
{
    //�[�[�[�[�[�[�[�[�[�[�萔�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    //�Ղ̏c�i���j�̐��@8�~8
    private const int boardMax = 8;

    //�^�[���Ƌ�ݒu�Ɏg���x��
    private enum turnBW
    {
        Black = 0,
        White = 180,//��̗��\�̊p�x
        Not = 1
    }

    //�Ղ̒����̃}�X��(�ŏ��ɒu���Ă����̐ݒu�ꏊ)
    //��
    private static readonly int[,] boardCenterB = {{ 3, 4 },
                                                   { 4, 3 } };
    //��
    private static readonly int[,] boardCenterW = {{ 3, 3 },
                                                   { 4, 4 }};

    //�ݒu����I�u�W�F�N�g�̂����W
    private const float putY = 0.07f;

    //�C�x���g��������������̐ݒu��̐����imin�`max�j
    private const int eventTimingMin = 22;
    private const int eventTimingMax = 42;

    //��𗠕Ԃ����̂����W�̍ō����B�_
    private const float moveY = 0.5f;
    //��𗠕Ԃ����̂����W�ړ��̑��x(�グ��Ƃ�)
    private const float moveYSpeedUp = 4.0f;
    //��𗠕Ԃ����̂����W�ړ��̑��x(������Ƃ�)
    private const float moveYSpeedDo = 4.5f;

    //��𗠕Ԃ��p�x
    private const float moveRo = 180.0f;
    //��𗠕Ԃ����̉�]�̑��x
    private const float moveRoSpeed = 500.0f;

    //�C�x���g�̎��
    public enum eventName
    {
        Corner = 0,
        Change,
        Site,
        Not
    }
    //�C�x���g�̎�ނ̐�
    private const int eventQty = 3;
    //�C�x���g�̏I���܂ł̑ҋ@����
    private const float eventStop = 4.0f;

    //�G�t�F�N�g�̂����W
    private const float effectY = 0.15f;
    //�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[


    private turnBW turn = turnBW.Black;//�ǂ����̃^�[����
    private turnBW notTurn = turnBW.White;//�^�[������Ȃ���

    //��f�[�^
    [SerializeField] private GameObject piece;
    private turnBW[,] pieceBoard = new turnBW[boardMax, boardMax];
    private GameObject[,] pieceBox = new GameObject[boardMax, boardMax];

    //�u����Ƃ�
    [SerializeField] private GameObject show;//�u����ꏊ
    private bool[,] overBoard = new bool[8, 8];
    private GameObject[,] overBox = new GameObject[8, 8];

    //��̐�
    private int countB = 0;
    private int countW = 0;
    //�Q�[�����I��邩
    private bool end = false;

    //�������͋��u���Ȃ��悤�ɂ���
    private bool canPut = true;

    //�C�x���g
    private bool eventUse = false;
    private int eventTime = 0;
    private int[] site = { 10,10 };

    //�G�t�F�N�g
    [Header("�G�t�F�N�g")]
    [SerializeField] private GameObject cornerEffect;
    [SerializeField] private GameObject changeEffect;
    [SerializeField] private GameObject siteEffect;

    //UiCon
    UiCon uiCon;



    // Start is called before the first frame update
    void Start()
    {
        uiCon = GetComponent<UiCon>();

        //�Ֆʃf�[�^����
        for (int z = 0; z < boardMax; z++)
        {
            for (int x = 0; x < boardMax; x++)
            {

                if ((z == boardCenterB[0,0] && x == boardCenterB[0, 1]) || (z == boardCenterB[1, 0] && x == boardCenterB[1, 1]))
                { //������
                    pieceBoard[z, x] = turnBW.Black;
                    pieceBox[z, x] = Instantiate(piece, new Vector3(x, putY, -z), Quaternion.Euler(0, 0, (int)turnBW.Black));

                }
                else if ((z == boardCenterW[0, 0] && x == boardCenterW[0, 1]) || (z == boardCenterW[1, 0] && x == boardCenterW[1, 1]))
                {//������
                    pieceBoard[z, x] = turnBW.White;
                    pieceBox[z, x] = Instantiate(piece, new Vector3(x, putY, -z), Quaternion.Euler(0, 0, (int)turnBW.White));

                }
                else
                {                      //�����z�u�ȊO�͋�
                    pieceBoard[z, x] = turnBW.Not;
                }


            }
        }

        eventTime = UnityEngine.Random.Range(eventTimingMin, eventTimingMax);//�(min�`max�̃����_��)�u���ꂽ���ɃC�x���g�𔭓�������
        AccompanyTurn();
    }

    // Update is called once per frame
    void Update()
    {
        //�ݒu
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

    //���u��
    private void PutPiece(int z,int x) {
        if (overBoard[z, x] == true&&canPut)
        {//��u����ꏊ��������
            pieceBox[z, x] = Instantiate(piece, new Vector3(x, 0.07f, -z), Quaternion.Euler(0, 0, (int)turn));
            pieceBoard[z, x] = turn;

            SearchReverse(z, x,true);//���Ԃ�
            canPut = false;
        }
    }

    //�^�[�����
    private void TurnChange()
    {
        if (turn == turnBW.Black)//�����甒��
        {
            turn = turnBW.White;
            notTurn = turnBW.Black;
        }
        else if (turn == turnBW.White)//�����獕��
        {
            turn = turnBW.Black;
            notTurn = turnBW.White;
        }

        SearchTurn();
    }

    //�u����ꏊ���Ȃ��Ƃ��^�[����������x��シ��
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
        else//�^�[���m���̏���
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

    //�^�[���m���̏���
    private void AccompanyTurn() {
        TurnUi();
        CountPiece();
        CanPut(true);
        canPut = true;
    }

    //�^�[���̐F��ui
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

    //���̋�̐�
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

                    if (site[0] == x&&site[1] == z) {//�C�x���g
                        countB += 3;
                    }
                }
                else if (pieceBoard[z, x] == turnBW.White)
                {
                    countW++;

                    if (site[0] == x && site[1] == z)//�C�x���g
                    {
                        countW += 3;
                    }
                }
            }
        }

        uiCon.CountPieceUi(countB, countW);
    }

    //��̐ݒu���\�ȃ}�X��
    //overPut��false�̎��͒u���ꏊ�����邩���ׂ邾���̎��Atrue�̎��͒u����ꏊ�̕\����
    private bool CanPut(bool overPut)
    {
        overBoard = new bool[boardMax, boardMax];
        bool canPut = false;

        foreach (GameObject show in overBox)//�S�ď���
        {
            Destroy(show);
        }

        for (int z = 0; z < boardMax; z++)
        {
            for (int x = 0; x < boardMax; x++)
            {
                if (pieceBoard[z, x] == turnBW.Not)//�����u����ĂȂ��Ȃ�ݒu�\�����ׂ�
                {
                    if (SearchReverse(z, x,false)) {//�ݒu�\�Ȃ�\������
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

    //�u���邩���ׂ�(reverse=false)�܂��́A���ۗ��Ԃ��ꏊ��list�ɓ����(reverse=true)
    private bool SearchReverse(int z, int x,bool reverse)
    {
        int[,] dir = new int[,] { { -1, 1 }, {  0, 1 }, {  1, 1 },
                                  { -1, 0 },            {  1, 0 },
                                  { -1,-1 }, {  0,-1 }, {  1,-1 }};//�΂̗�8����{x,z}

        bool re = false;//return
        List<List<List<int>>> lists = new List<List<List<int>>>();//�������̃��X�g

        for (int i = 0; i < dir.GetLength(0); i++)
        {
            int X = x;
            int Z = z;
            int xDir = dir[i,0];
            int zDir = dir[i,1];

            bool firstF = true;

            List<List<int>> overLists = new List<List<int>>();//������̃��X�g

            while (true)
            {
                X += xDir;
                Z += zDir;

                List<int> overList = new List<int>();//��}�X�̃��X�g

                if (X < 0 || 7 < X || Z < 0 || 7 < Z)//�{�[�h�𒴂�����break
                {
                    break;
                }

                if (firstF)//�ŏ��ׂ̗�����̐F���i��x�������s�j
                {
                    if (pieceBoard[Z, X] != notTurn)//�Ⴄ�Ȃ�break
                    {
                        break;
                    }
                    firstF = false;
                }

                if (pieceBoard[Z, X] == notTurn)//����̐F�Ȃ�list�ɓ����
                {
                    overList.Add(X);
                    overList.Add(Z);
                    overLists.Add(overList);

                }
                else if (pieceBoard[Z, X] == turn)//�����̐F�Ȃ炱���܂ŗ��Ԃ�
                {
                    if (reverse)//���Ԃ�
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

    //���Ԃ��Ă���Ԃɋ��u���Ȃ��悤�ɍŌ�̋���Ԃ��Ă���^�[�����ɐi��
    //���Ԃ�����
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

    //���Ԃ�
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

    //���Ԃ��A�j���[�V����
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

        //���Ԃ���̒��ōŌ�̋�
        if (last) {
            TurnChange();
        }
    }

    //�Q�[���I���A���U���g
    private void GameEnd()
    {
       uiCon.ResultUi(countB,countW);
    }

    //�C�x���g
    //�O�̃C�x���g�̒��łǂꂪ�N���邩
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

    //CornerEvent���ł��邩
    private bool SearchCornerEvent() {
        for (int z = 0; z < boardMax; z += boardMax-1) {
            for (int x = 0; x < boardMax; x += boardMax - 1) {
                if (pieceBoard[z, x] != turnBW.Not)//�l�p�ɋ�u����Ă�����true��Ԃ�
                {
                    return true;
                }
            }
        }
        return false;
    }

    //�p�ɒu���ꂽ�������
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

    //CornerEvent�̃A�j���[�V����
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


    //�u����Ă����̐F���S�ċt�ɂȂ�
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

    //�܂����u���Ă��Ȃ��ꂩ���ɍŏI�I�ɋ�u����Ă���Ɠ��_�̃R�}���{3�����
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
