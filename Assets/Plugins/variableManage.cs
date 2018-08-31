using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableManage : MonoBehaviour
{
    // 移動用変数
    static public int movingXaxis;
    static public int movingYaxis;
    // 攻撃用変数
    static public bool fireWeapon;
    static public GameObject lockonTarget;
    static public bool lockoned;
    // 機体データ用変数
    static public float currentHealth;
    // ほか
    static public bool controlLock;


    private void Start()
    {
        // 変数の初期化
        initializeVariable();
    }

    static public void initializeVariable()
    {
        movingXaxis = 0;
        movingYaxis = 0;
        fireWeapon = false;
        lockoned = false;
        controlLock = false;
        // 試合開始直後に破損しないよう0にしない
        currentHealth = 10.0f;
    }
}
