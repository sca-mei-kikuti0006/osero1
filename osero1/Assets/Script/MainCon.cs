using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�I�Z���݂̂̋@�\�̃X�N���v�g

public class MainCon : MonoBehaviour
{
    private enum turnBW
    {
        Black = 0,
        White = 180, //piece���Ђ�����Ԃ��̂�180
        Not = 2
    }
    private turnBW turn = turnBW.Black;//�ǂ����̃^�[����
    private turnBW notTurn = turnBW.White;//�^�[������Ȃ���

    //��f�[�^
    [SerializeField] private GameObject piece;
    private turnBW[,] piseBoard = new turnBW[8, 8];
    private GameObject[,] pieceBox = new GameObject[8, 8];

    //�u����Ƃ�
    [SerializeField] private GameObject show;//�u����ꏊ
    private bool[,] overBoard = new bool[8, 8];
    private GameObject[,] overBox = new GameObject[8, 8];

    private bool canPut = false;//�u���鏊���ꂩ���ł����邩
    private bool end = false;//���݂��u���Ȃ���ΏI���

    //�Ђ�����Ԃ���X�g
    List<int> overListX = new List<int>();
    List<int> overListZ = new List<int>();

    //��̐�UI
    [SerializeField] private Text BUi;
    [SerializeField] private Text WUi;
    private int countB = 0;
    private int countW = 0;

    //ui
    [SerializeField] private Image CUi;
    [SerializeField] private Image CUi2;

    //���U���g
    [SerializeField] private GameObject risu;
    [SerializeField] private Text risuT;

    //���Ԃ�A�j���[�V�����̍Œ��Ɏ��ɐi�܂Ȃ��悤��
    enum step {
        free,//���u���鎞
        put,//���u������
        search,//���Ԃ���𒲂ׂ���
        reverse//��𗠕Ԃ�����
    }
    private step stopCount = step.free;

    // Start is called before the first frame update
    void Start()
    {
        //�Ֆʃf�[�^����
        for (int i = 0; i < 8; i++){
            for (int j = 0; j < 8; j++){

                if ((i == 3 && j == 4) || (i == 4 && j == 3)){ //������
                    piseBoard[i, j] = turnBW.Black;
                    pieceBox[i, j] = Instantiate(piece, new Vector3(j, 0.07f, -i), Quaternion.Euler(0, 0, 0));

                }
                else if ((i == 3 && j == 3) || (i == 4 && j == 4)) {//������
                    piseBoard[i, j] = turnBW.White;
                    pieceBox[i, j] = Instantiate(piece, new Vector3(j, 0.07f, -i), Quaternion.Euler(0, 0, 180));

                }
                else{                      //�����z�u�ȊO�͋�
                    piseBoard[i, j] = turnBW.Not;
                }

            }
        }

        CanPut();

        risu.gameObject.SetActive(false);//���U���g���\��
    }

    // Update is called once per frame
    void Update()
    {
        if(stopCount == step.free) {

            //���݂��u����ꏊ���Ȃ��Ƃ��Q�[�����I���
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

            //��ݒu
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 13.0f))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    int z = (int)hit.transform.position.z * -1;
                    int x = (int)hit.transform.position.x;

                    if (overBoard[z, x] == true){//��u����ꏊ��������
                        stopCount = step.put;//�X�e�b�v��i�߂�
                        pieceBox[z, x] = Instantiate(piece, new Vector3(hit.transform.position.x, 0.07f, hit.transform.position.z), Quaternion.Euler(0, 0, (int)turn));
                        piseBoard[z, x] = turn;
                        SearchReverse(z, x);
                        end = false;
                    }
                }
            }
        }
        else if(stopCount == step.reverse) {//���Ԃ�A�j���[�V�������I�������
            TurnChange();//�^�[�����
            stopCount = step.free;
        }
    }

    //�^�[�����
    private void TurnChange()
    {
        if (turn == turnBW.Black)//�����甒��
        {
            turn = turnBW.White;
            notTurn = turnBW.Black;

            CUi.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
            CUi2.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);

        }
        else if (turn == turnBW.White)//�����獕��
        {
            turn = turnBW.Black;
            notTurn = turnBW.White;

            CUi.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 255.0f);
            CUi2.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 255.0f);
        }

        CountPiece();
        CanPut();
    }

    //���̋�̐�
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

    //��̐ݒu���\��
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
                if (piseBoard[i, j] == turnBW.Not)//�����u����ĂȂ��Ȃ�ݒu�\�����ׂ�
                {
                    SearchReverse(i, j);
                }
                else
                {
                    overBoard[i, j] = false;
                }

                if (overBoard[i, j])//�ݒu�\�Ȃ�\������
                {
                    overBox[i, j] = Instantiate(show, new Vector3(j, 0.07f, -i), Quaternion.Euler(90.0f, 0, 0));
                    canPut = true;
                }
            }
        }
    }

    //�u���邩���ׂ�܂��́A���ۗ��Ԃ��ꏊ��list�ɓ����
    private void SearchReverse(int Z, int X)
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

                if (x < 0 || 7 < x || z < 0 || 7 < z)//�{�[�h�𒴂�����break
                {
                    break;
                }

                if (firstF)//�ŏ��ׂ̗�����̐F���i��x�������s�j
                {
                    if (piseBoard[z, x] != notTurn)//�Ⴄ�Ȃ�break
                    {
                        break;
                    }
                    firstF = false;
                }

                if (piseBoard[z, x] == notTurn)//����̐F�Ȃ�list�ɓ����
                {
                    overListX.Add(x);
                    overListZ.Add(z);

                }
                else if (piseBoard[z, x] == turn)//�����̐F�Ȃ炱���܂ŗ��Ԃ�
                {
                    overBoard[Z, X] = true;
                    if (stopCount == step.put)//���Ԃ�
                    {
                        StartCoroutine(Reverse(overListX, overListZ));
                    }
                    isOver = true;//while������

                }
                else
                {
                    break;
                }
            }
        }
        if(stopCount == step.put) {//�X�e�b�v��i�߂�
            stopCount = step.search;
        }

    }

    //���Ԃ�
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

    //���Ԃ��A�j���[�V����
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

        if(stopCount == step.search){//�X�e�b�v��i�߂�
            yield return 3;
            stopCount = step.reverse;
        }
    }

    //�Q�[���I���A���U���g
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

