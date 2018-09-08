using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIbattle : MonoBehaviour
{
    //テキスト格納
    public Text infoText;
    public Text timerText;
    public Text wepCurrentText;
    public Text wepStandbyText;
    public Text redTeamText;
    public Text blueTeamText;
    public Text healthText;
    public Text returnText;

    //オブジェクト格納
    public GameObject returnMenu;
    public GameObject mapUIobj;
    public GameObject winLoseBase;
    public GameObject winText;
    public GameObject loseText;

    // 仮想操作パッド関連
    private float currentXpos;
    private float currentYpos;
    private float startXpos;
    private float startYpos;
    private bool touchStart;

    // その他
    private float messageTimer;

    void Start()
    {
        currentXpos = 0.0f;
        currentYpos = 0.0f;
        touchStart = false;
        messageTimer = 0.0f;
        infoText.text = "";
    }

void Update()
    {
        // 仮想操作パッド
        // 位置取得関連
        for (int i = 0; i < Input.touchCount; ++i)
        {
            // 画面の左下に指があるか判定
            if (Input.GetTouch(i).position.x < (Screen.width / 2.5f))
            {
                if (Input.GetTouch(i).position.y < (Screen.height / 2.0f))
                {
                    // 指があった場合、座標を格納
                    currentXpos = Input.GetTouch(i).position.x;
                    currentYpos = Input.GetTouch(i).position.y;
                    if (!touchStart)
                    {
                        // タッチした瞬間の座標を保存
                        startXpos = currentXpos;
                        startYpos = currentYpos;
                        touchStart = true;
                    }
                }
            }
        }

        if (Input.touchCount == 0)
        {
            // 画面に指が触れていないときは、座標を初期化
            currentXpos = 0.0f;
            currentYpos = 0.0f;
            startXpos = 0.0f;
            startYpos = 0.0f;
            touchStart = false;
        }

        // モバイル時のみ動作
        if (Application.isMobilePlatform)
        {
            // 移動計算 - X軸
            if ((startXpos - currentXpos) < (Screen.width * -0.05f))
            {
                // 右を入力
                VariableManage.movingXaxis = -1;
            }
            else if ((startXpos - currentXpos) > (Screen.width * 0.05f))
            {
                // 左を入力
                VariableManage.movingXaxis = 1;
            }
            else
            {
                // 0を入力
                VariableManage.movingXaxis = 0;
            }

            // 移動計算 - Y軸
            if ((startYpos - currentYpos) < (Screen.height * -0.08f))
            {
                // 上を入力
                VariableManage.movingYaxis = 1;
            }
            else if ((startYpos - currentYpos) > (Screen.height * 0.08f))
            {
                // 下を入力
                VariableManage.movingYaxis = -1;
            }
            else
            {
                // 0を入力
                VariableManage.movingYaxis = 0;
            }
        }

        // 画面表示
        string text = "HP:" + VariableManage.currentHealth;
        // ★敵味方のチーム分けが分かりづらいのでテキトーに判別できる文字列を出す
        text += ",T:" + VariableManage.myTeamID;
        healthText.text = text;

        timerText.text = Mathf.Round(VariableManage.timeRest).ToString();

        if (VariableManage.myTeamID == 1)
        {
            blueTeamText.text = "D" + VariableManage.team1Rest + "_L" + VariableManage.base1Rest;
            redTeamText.text = "D" + VariableManage.team2Rest + "_L" + VariableManage.base2Rest;
        }
        else
        {
            blueTeamText.text = "D" + VariableManage.team2Rest + "_L" + VariableManage.base2Rest;
            redTeamText.text = "D" + VariableManage.team1Rest + "_L" + VariableManage.base1Rest;
        }

        // 画面表示（メッセージ）
        if (VariableManage.infomationMessage != 0)
        {
            if (VariableManage.infomationMessage == 1)
            {
                infoText.text = "味方が撃破されました";
            }
            else if (VariableManage.infomationMessage == 2)
            {
                infoText.text = "敵を撃破しました";
            }
            VariableManage.infomationMessage = 0;
            messageTimer = 3.0f;
        }
        if (messageTimer > 0.0f)
        {
            // ３秒後にメッセージを削除
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0.0f)
            {
                infoText.text = "";
            }
        }

        // 勝敗用
        if (VariableManage.finishedGame)
        {
            if (VariableManage.myTeamID == VariableManage.gameResult)
            {
                winText.SetActive(true);
            }
            else
            {
                loseText.SetActive(true);
            }
            winLoseBase.SetActive(true);
        }
    }

    //コンフィグ表示用ボタン
    public void configToggle()
    {
        if (returnMenu.GetActive())
        {
            returnMenu.SetActive(false);
        }
        else
        {
            returnMenu.SetActive(true);
        }
    }

    //メインメニューへ戻る
    public void returnMainMenu()
    {
        //Application.LoadLevel("mainMenu");
        SceneManager.LoadScene("mainMenu");
    }

    // 武器発射ボタン
    public void fireWep()
    {
        VariableManage.fireWeapon = true;
    }

    // マップ表示切り替えボタン
    public void ShowMap()
    {
        if (VariableManage.mapEnabled)
        {
            mapUIobj.SetActive(false);
            VariableManage.mapEnabled = false;
        }
        else
        {
            mapUIobj.SetActive(true);
            VariableManage.mapEnabled = true;
        }
    }
}
