using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private enum turnBW
    {
        Black = 0,
        White = 180,
        Not = 1
    }
    private turnBW turn = turnBW.Black;//�ǂ����̃^�[����
    private turnBW notTurn = turnBW.White;//�^�[������Ȃ���

    //��f�[�^
    [SerializeField] private GameObject piece;
    private turnBW[,] pieceBoard = new turnBW[8, 8];
    private GameObject[,] pieceBox = new GameObject[8, 8];

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
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {

                if ((i == 3 && j == 4) || (i == 4 && j == 3))
                { //������
                    pieceBoard[i, j] = turnBW.Black;
                    pieceBox[i, j] = Instantiate(piece, new Vector3(j, 0.07f, -i), Quaternion.Euler(0, 0, 0));

                }
                else if ((i == 3 && j == 3) || (i == 4 && j == 4))
                {//������
                    pieceBoard[i, j] = turnBW.White;
                    pieceBox[i, j] = Instantiate(piece, new Vector3(j, 0.07f, -i), Quaternion.Euler(0, 0, 180));

                }
                else
                {                      //�����z�u�ȊO�͋�
                    pieceBoard[i, j] = turnBW.Not;
                }


            }
        }

        eventTime = Random.Range(22, 43);//�22�`42�u���ꂽ���ɃC�x���g�𔭓�������
        CanPut(true);
        CountPiece();
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
            pieceBox[z, x] = Instantiate(piece, new Vector3(x, 0.07f, z*-1.0f), Quaternion.Euler(0, 0, (int)turn));
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
                TurnUi();
                CountPiece();
                CanPut(true);
                canPut = true;
            }
            end = false;
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
        overBoard = new bool[8, 8];
        bool canPut = false;

        foreach (GameObject show in overBox)//�S�ď���
        {
            Destroy(show);
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieceBoard[i, j] == turnBW.Not)//�����u����ĂȂ��Ȃ�ݒu�\�����ׂ�
                {
                    if (SearchReverse(i, j,false)) {//�ݒu�\�Ȃ�\������
                        if (overPut)
                        {
                            overBoard[i, j] = true;
                            overBox[i, j] = Instantiate(show, new Vector3(j, 0.07f, -i), Quaternion.Euler(90.0f, 0, 0));
                        }
                        canPut = true;
                    }
                }
                else
                {
                    overBoard[i, j] = false;
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
            if (i == lists.Count - 1) StartCoroutine(Reverse(lists[i],true));
            else                      StartCoroutine(Reverse(lists[i],false));
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

            if(last&&i == overLists.Count-1) StartCoroutine(ReverseAnimation(z, x,true));
            else                             StartCoroutine(ReverseAnimation(z, x,false));

            yield return new WaitForSeconds(0.1f);
        }
    }

    //���Ԃ��A�j���[�V����
    private IEnumerator ReverseAnimation(int z, int x,bool last)
    {
        float move = pieceBox[z, x].transform.position.y;
        while (move < 0.5f)
        {
            move += Time.deltaTime * 4.0f;
            if (move > 0.5f)
            {
                move = 0.5f;
            }
            pieceBox[z, x].transform.position = new Vector3(pieceBox[z, x].transform.position.x, move, pieceBox[z, x].transform.position.z);
            yield return null;
        }

        move = 0;
        while (move < 180)
        {
            float mo = Time.deltaTime * 500.0f;
            move += mo;
            if (move > 180)
            {
                mo -= move - 180;
                move = 180;
            }
            pieceBox[z, x].transform.Rotate(new Vector3(0, 0,mo));
            yield return null;
        }

        move = pieceBox[z, x].transform.position.y;
        while (move > 0.07f)
        {
            move -= Time.deltaTime * 4.5f;
            if (move < 0.07f)
            {
                move = 0.07f;
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
        int eventRan = Random.Range(cor, 3);
        uiCon.EventUi(eventRan);
        switch (eventRan) {
            case 0:
                CornerEvent();
                break;
            case 1:
                ChangeEvent();
                break;
            case 2:
                SiteEvent();
                break;
            default:
                break;
        }
        eventUse = true;
    }

    //CornerEvent���ł��邩
    private bool SearchCornerEvent() {
        for (int z = 0; z < 8; z += 7) {
            for (int x = 0; x < 8; x += 7) {
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
        for (int z = 0; z < 8; z += 7)
        {
            for (int x = 0; x < 8; x += 7)
            {
                if (pieceBoard[z, x] != turnBW.Not)
                {
                    Instantiate(cornerEffect, new Vector3(x, 0.15f, -z), Quaternion.identity);
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
            move += Time.deltaTime * 4.0f;
            pieceBox[z, x].transform.position = new Vector3(pieceBox[z, x].transform.position.x, move, pieceBox[z, x].transform.position.z);
            yield return null;
        }
        Destroy(pieceBox[z, x]);
    }


    //�u����Ă����̐F���S�ċt�ɂȂ�
    private void ChangeEvent()
    {
        Instantiate(changeEffect);
        for (int z = 0; z < 8; z++){
            for (int x = 0; x < 8; x++){
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
            int xRan = Random.Range(0, 8);
            int zRan = Random.Range(0, 8);
            if (pieceBoard[zRan, xRan] == turnBW.Not) {
                Instantiate(siteEffect, new Vector3(xRan, 0.1f, -zRan), Quaternion.identity);
                site[0] = xRan;
                site[1] = zRan;
                break;
            }
        }
        StartCoroutine(EventEnd());
    }

    private IEnumerator EventEnd() {
        yield return new WaitForSeconds(4.0f);
        TurnUi();
        CountPiece();
        CanPut(true);
        canPut = true;
    }
}
