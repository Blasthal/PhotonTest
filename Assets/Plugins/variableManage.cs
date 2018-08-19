using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class variableManage : MonoBehaviour
{
    // 移動用変数
    static public int movingXaxis;
    static public int movingYaxis;

    private void Start()
    {
        // 変数の初期化
        initializeVariable();
    }

    static public void initializeVariable()
    {
        movingXaxis = 0;
        movingYaxis = 0;
    }
}
