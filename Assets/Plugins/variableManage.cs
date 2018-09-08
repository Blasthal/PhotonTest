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
    // ゲーム管理用変数
    static public int myTeamID;
    static public bool mapEnabled;
    static public GameObject team1baseBullet;
    static public GameObject team2baseBullet;
    // 勝敗用変数
    static public bool finishedGame; // 勝敗が確定されたか
    static public int team1Rest; // チーム1の残り撃破数
    static public int team2Rest; // チーム2の残り撃破数
    static public float base1Rest; // チーム1の拠点の残りHP
    static public float base2Rest; // チーム2の拠点の残りHP
    static public float timeRest; // ゲームの残り時間
    static public int gameResult; // 1-teamID=1の勝利、2-teamID=2の勝利
    // 画面表示用変数
    static public int infomationMessage;


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
        myTeamID = 0;
        mapEnabled = false;
        infomationMessage = 0;
        // 試合開始直後に破損しないよう0にしない
        currentHealth = 10.0f;
        // 勝敗用
        finishedGame = false;
        team1Rest = 20;
        team2Rest = 20;
        base1Rest = 99999.0f;
        base2Rest = 99999.0f;
        timeRest = 400.0f;
        gameResult = 0;
    }
}
