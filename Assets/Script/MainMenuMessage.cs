using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMessage
    : MonoBehaviour
{
    private void Start()
    {
        // レベルアップ処理
        if (VariableManage.currentExp >= VariableManage.nextExp)
        {
            VariableManage.currentLv += 1;
            VariableManage.currentExp = VariableManage.nextExp - VariableManage.currentExp;
            VariableManage.showLvupMes = true;
        }

        // レベルから次の必要経験値を計算
        VariableManage.nextExp = VariableManage.currentLv * 100;
    }
}
