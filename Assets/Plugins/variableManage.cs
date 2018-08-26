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
    }
}
