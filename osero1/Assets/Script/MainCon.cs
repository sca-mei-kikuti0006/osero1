using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�I�Z���݂̂̋@�\�̃X�N���v�g

public class MainCon : MonoBehaviour
{
    [SerializeField] private GameObject piece;
    [SerializeField] private GameObject show;//�u����ꏊ

    private enum turnBW
    {
        Black = 0,
        White = 180, //piece���Ђ�����Ԃ��̂�180
        Not = 2
    }
    private turnBW turn = turnBW.Black;//�ǂ����̃^�[����
    private turnBW notTurn = turnBW.White;//�^�[������Ȃ���

    //�Ֆʃf�[�^
    private turnBW[,] board = new turnBW[8, 8];
    //��f�[�^
    private GameObject[,] pieceBox = new GameObject[8, 8];
    //�u����Ƃ�
    private bool[,] canOver = new bool[8, 8];
    private GameObject[,] overBox = new GameObject[8, 8]; //�u����ꏊ�ɐݒu����ԃ}�[�N

    private bool canPut = false;//���݂��u���鏊�����邩
    private int end = 0;//�I���

    //�Ђ�����Ԃ���X�g
    List<int> overListX = new List<int>();
    List<int> overListZ = new List<int>();

    //��̐�UI
    [SerializeField] private Text BUi;
    [SerializeField] private Text WUi;
    private int countB = 0;
    private int countW = 0;

    //�^�[���̕��̐Fui
    [SerializeField] private Image CUi;
    [SerializeField] private Image CUi2;

    //���U���g
    [SerializeField] private GameObject risu;
    [SerializeField] private Text risuT;

    // Start is called before the first frame update
    void Start()
    {
        //�Ֆʃf�[�^����
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i == 3 && j == 4) || (i == 4 && j == 3))
                { //������
                    board[i, j] = turnBW.Black;
                    pieceBox[i, j] = Instantiate(piece, new Vector3(j, 0.07f, -i), Quaternion.Euler(0, 0, 0));

                }
                else if ((i == 3 && j == 3) || (i == 4 && j == 4))
                {//������
                    board[i, j] = turnBW.White;
                    pieceBox[i, j] = Instantiate(piece, new Vector3(j, 0.07f, -i), Quaternion.Euler(0, 0, 180));

                }
                else
                {                      //�����z�u�ȊO�͋�
                    board[i, j] = turnBW.Not;
                }

            }
        }

        CanOver();

        risu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        //�u����ꏊ���Ȃ��Ƃ�
        if (!canPut)
        {
            if (end == 2)
            {
                GameEnd();
            }
            else
            {
                TurnChange();
                end++;
            }
        }

        //��ݒu
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 13.0f))
        {
            if (Input.GetMouseButtonDown(0))
            {
                int z = (int)hit.transform.position.z * -1;
                int x = (int)hit.transform.position.x;

                if (canOver[z, x] == true)
                {//��u����ꏊ��������
                    pieceBox[z, x] = Instantiate(piece, new Vector3(hit.transform.position.x, 0.07f, hit.transform.position.z), Quaternion.Euler(0, 0, (int)turn));
                    board[z, x] = turn;
                    TurnOver(z, x, true);
                    end = 0;
                    TurnChange();//�^�[�����
                }
            }
        }
        
    }


    //�^�[�����
    private void TurnChange()
    {
        if (turn == turnBW.Black)
        {
            turn = turnBW.White;
            notTurn = turnBW.Black;

            CUi.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
            CUi2.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);

        }
        else if (turn == turnBW.White)
        {
            turn = turnBW.Black;
            notTurn = turnBW.White;

            CUi.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 255.0f);
            CUi2.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 255.0f);
        }

        CanOver();
        CountPiece();
    }

    private void CanOver()
    {//��̐ݒu���\��
        canOver = new bool[8, 8];
        canPut = false;

        foreach (GameObject show in overBox)
        {
            Destroy(show);
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == turnBW.Not)
                {
                    TurnOver(i, j, false);
                }
                else
                {
                    canOver[i, j] = false;
                }
                if (canOver[i, j])
                {
                    overBox[i, j] = Instantiate(show, new Vector3(j, 0.07f, -i), Quaternion.Euler(90.0f, 0, 0));
                    canPut = true;
                }
            }
        }
    }

    private void CountPiece()
    { //��̐�
        countB = 0;
        countW = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == turnBW.Black)
                {
                    countB++;
                }
                else if (board[i, j] == turnBW.White)
                {
                    countW++;
                }
            }
        }
        BUi.text = string.Format("{00:D2}", countB);
        WUi.text = string.Format("{00:D2}", countW);

    }

    //�u���邩
    private void TurnOver(int Z, int X, bool over)
    {
        int[] _X = new int[] { -1, -1, 0, 1, 1,  1,  0, -1 };
        int[] _Z = new int[] {  0,  1, 1, 1, 0, -1, -1, -1 };//�΂̗�8����

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

                if (x < 0 || 7 < x || z < 0 || 7 < z)
                {
                    break;
                }

                if (firstF)
                {
                    if (board[z, x] != notTurn)
                    {
                        break;
                    }
                    firstF = false;
                }

                if (board[z, x] == notTurn)
                {
                    overListX.Add(x);
                    overListZ.Add(z);

                }
                else if (board[z, x] == turn)
                {
                    canOver[Z, X] = true;
                    if (over == true){
                        StartCoroutine(OverPiece(overListX, overListZ));
                    }
                    isOver = true;//while������

                }
                else
                {
                    break;
                }
            }
        }

    }

    //���Ԃ�
    private IEnumerator OverPiece(List<int> listX, List<int> listZ)
    {
        int x, z;
        for (int i = 0; i < listX.Count; i++)
        {
            x = listX[i];
            z = listZ[i];
            board[z, x] = turn;
            Debug.Log(turn);
            //pieceBox[z, x].transform.Rotate(new Vector3(0, 0, 180));
            StartCoroutine(OverAnimation(z,x));
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator OverAnimation(int z,int x) {
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
    }

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

