using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class MainCon : MonoBehaviour
{
    //�[�[�[�[�[�[�[�[�[�[�萔�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    //�Ղ̏c�i���j�̐��@8�~8
    private const int boardMax = 8;

    //�^�[���Ƌ�ݒu�Ɏg���x��
    public enum turnBW
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

    //�Q�[�����n�܂�܂ł̎���
    private const float stopStart = 1.1f;

    //��̃A�j���[�V����
    //���̋�𗠕Ԃ��܂ł̎���
    private const float moveStop = 0.1f;
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

    //CPU
    //����
    public enum level 
    {
        Hard,
        Normal,
        Easy,
        Not
    }

    //�e�}�X�̕]���l
    private static readonly int[,] squareValue = { { 20, 2,13, 8, 8,13, 2,20 },
                                                   {  2, 0, 5, 5, 5, 5, 0, 2 },
                                                   { 13, 5,10, 8, 8,10, 5,13 },
                                                   {  8, 5, 8, 8, 8, 8, 5, 8 },
                                                   {  8, 5, 8, 8, 8, 8, 5, 8 },
                                                   { 13, 5,10, 8, 8,10, 5,13 },
                                                   {  2, 0, 5, 5, 5, 5, 0, 2 },
                                                   { 20, 2,13, 8, 8,13, 2,20 }};
    private const int depth = 3;//�����܂œǂނ��i����̎�܂߁j

    //�C�x���g
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
    private bool[,] overBoard = new bool[boardMax, boardMax];
    private GameObject[,] overBox = new GameObject[boardMax, boardMax];

    //�Q�[�����n�߂�܂ł̃J�E���g
    private float stopCount = 0;
    private bool stopEnd = false;

    //��̐�
    private int countB = 0;
    private int countW = 0;
    //�Q�[�����I��邩
    private bool end = false;

    //�������͋��u���Ȃ��悤�ɂ���
    private bool canPutP = true;

    //���Ԃ��A�j���[�V�������͎��ɐi�܂Ȃ��悤�ɂ���
    private int animCount = 0;

    //CPU
    //CPU�̐F
    public static turnBW cpuColor = turnBW.Not;
    //CPU�̋���
    public static level cpuLevel = level.Easy;

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
    }

    // Update is called once per frame
    void Update()
    {
        //�Q�[�����X�^�[�g����܂ł̑ҋ@����
        if (!stopEnd) {
            stopCount += Time.deltaTime;
            if (stopCount > stopStart)
            {
                AccompanyTurn();
                stopEnd = true;
            }
        }


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
        if (overBoard[z, x] == true&&canPutP)
        {//��u����ꏊ��������
            pieceBox[z, x] = Instantiate(piece, new Vector3(x, 0.07f, -z), Quaternion.Euler(0, 0, (int)turn));
            pieceBoard[z, x] = turn;

            SearchReverse(SearchReverse(z, x,pieceBoard));//���Ԃ�
            canPutP = false;
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

        if (cpuColor == turn)//CPU�̃^�[����
        {
            CanPut(false);
            CPUputPiece();
        }
        else {
            CanPut(true);
            canPutP = true;
        }
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
        bool canPut = false;
        overBoard = new bool[boardMax, boardMax];

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
                    if (SearchReverse(z, x,pieceBoard).Count > 0) {//�ݒu�\�Ȃ�\������
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

    //��}�X�ɑ΂��āA�������̒u����ꏊ��list�ŕԂ�(CPU�ɂ��g��)
    private List<List<List<int>>> SearchReverse(int z, int x,turnBW[,] board)
    {
        int[,] dir = new int[,] { { -1, 1 }, {  0, 1 }, {  1, 1 },
                                  { -1, 0 },            {  1, 0 },
                                  { -1,-1 }, {  0,-1 }, {  1,-1 }};//�΂̗�8����{x,z}

        List<List<List<int>>> lists = new List<List<List<int>>>();//�������̃��X�g

        for (int i = 0; i < dir.GetLength(0); i++)
        {
            int x_ = x;
            int z_ = z;
            int xDir = dir[i,0];
            int zDir = dir[i,1];

            bool firstF = true;

            List<List<int>> overLists = new List<List<int>>();//������̃��X�g

            while (true)
            {
                x_ += xDir;
                z_ += zDir;

                List<int> overList = new List<int>();//��}�X�̃��X�g

                if (x_ < 0 || 7 < x_ || z_ < 0 || 7 < z_)//�{�[�h�𒴂�����break
                {
                    break;
                }

                if (firstF)//�ŏ��ׂ̗�����̐F���i��x�������s�j
                {
                    if (board[z_, x_] != notTurn)//�Ⴄ�Ȃ�break
                    {
                        break;
                    }
                    firstF = false;
                }

                if (board[z_, x_] == notTurn)//����̐F�Ȃ�list�ɓ����
                {
                    overList.Add(x_);
                    overList.Add(z_);
                    overLists.Add(overList);

                }
                else if (board[z_, x_] == turn)//�����̐F�Ȃ炱���܂ŗ��Ԃ�
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

    //���Ԃ��Ă���Ԃɋ��u���Ȃ��悤�ɍŌ�̋���Ԃ��Ă���^�[�����ɐi��
    //���Ԃ�����
    private void SearchReverse(List<List<List<int>>> lists) {

        for (int i = 0; i < lists.Count; i++)
        {
            StartCoroutine(Reverse(lists[i]));
        }
    }

    //���Ԃ�
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

    //���Ԃ��A�j���[�V����
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
        //���Ԃ���̒��ōŌ�̋�
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

        //CPU�̃��x���Œu���ꏊ��ς���
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
 

        //�u��
        pieceBox[z, x] = Instantiate(piece, new Vector3(x, 0.07f, -z), Quaternion.Euler(0, 0, (int)turn));
        pieceBoard[z, x] = turn;

        SearchReverse(SearchReverse(z, x, pieceBoard));//���Ԃ�
    }


    //CPU
    private int CPUSearchValue(int z,int x) {
        turnBW[,] cpuBoard = new turnBW[boardMax, boardMax];//����̔Ֆ�
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
                //�^�[�����
                empty = turn;
                turn = notTurn;
                notTurn = empty;

                //�u����ꏊ����
                cpuOverBoard = CPUCanPut(cpuBoard);

                //�u����ꏊ�̒��ŕ]���������΂񍂂��Ƃ����T��
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

            //����̔Ֆʂ�ύX����
            //�u��
            cpuBoard[z_, x_] = turn;
            if (count % 2 == 0)
            {
                value += value2;
            }
            else
            {
                value -= value2;
            }

            //Debug.Log("�u�������W " + z_ + "," + x_);
            //Debug.Log("�u�����]�� " + value2);

            //���Ԃ�
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


        //Debug.Log("�]�� " + value + "\n �[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[");
        return value;
    }

    //CPU�̒u����ꏊ����
    private bool[,] CPUCanPut(turnBW[,] cpuBoard)
    {
        bool[,] CPUOverBoard = new bool[boardMax, boardMax];

        for (int z = 0; z < boardMax; z++)
        {
            for (int x = 0; x < boardMax; x++)
            {
                if (cpuBoard[z, x] == turnBW.Not)//�����u����ĂȂ��Ȃ�ݒu�\�����ׂ�
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


    //CPU����m�F�pDebugLog
    //private void Oll(turnBW[,] cpuBoard)
    //{
    //    string log = "";
    //    for (int z = 0; z < boardMax; z++)
    //    {
    //        for (int x = 0; x < boardMax; x++)
    //        {
    //            if (cpuBoard[z, x] == turnBW.Black)
    //            {
    //                log += "�� ";
    //            }
    //            else if (cpuBoard[z, x] == turnBW.White)
    //            {
    //                log += "�Z ";
    //            }
    //            else
    //            {
    //                log += "�Q ";
    //            }
    //        }
    //        log += "\n";
    //    }
    //    Debug.Log(log);
    //}


    //�C�x���g
    //�O�̃C�x���g�̒��łǂꂪ�N���邩
    private void SearchEvent() {
        int cor = 0;
        if (!SearchCornerEvent())
        {//����p�ɋ�u����Ă��Ȃ�������CornerEvent�͔������Ȃ�
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

    //�����҂�����ɃC�x���g���I��点��
    private IEnumerator EventEnd() {
        yield return new WaitForSeconds(eventStop);
        AccompanyTurn();
    }

    //�Q�[���I���A���U���g
    private void GameEnd()
    {
        uiCon.ResultUi(countB, countW);
    }
}
