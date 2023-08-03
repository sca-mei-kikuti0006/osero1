using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainConCP : MonoBehaviour
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


}
