using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UImainMenu
    : MonoBehaviour
{
    //テキスト格納
    public Text playerStatusText;
    public Text battleStartBtn;
    public Text unlockText;
    public Text unlockBtn;
    public Text lvupNum;
    //オブジェクト関連
    public GameObject unlockBtnObj;
    public GameObject lvupObj;
    // レベルアップメッセージ用
    private float mesTimer;


    void Start()
    {
        mesTimer = 0.0f;
    }


    void Update()
    {
        // 画面表示
        lvupNum.text = VariableManage.currentLv.ToString();
        playerStatusText.text =
            "PlayerClass : " + VariableManage.currentLv +
            " NextClass " + VariableManage.currentExp +
            " / " + VariableManage.nextExp;

        // レベルアップメッセージ
        if (VariableManage.showLvupMes)
        {
            if (mesTimer == 0.0f)
            {
                lvupObj.SetActive(true);
            }
            else if (mesTimer > 3.0f)
            {
                mesTimer = 0.0f;
                VariableManage.showLvupMes = false;
                lvupObj.SetActive(false);
            }
            mesTimer += Time.deltaTime;
        }
    }

    public void jumpBattleScene()
    {
        VariableManage.initializeVariable();

        //Application.LoadLevel("battle");
        SceneManager.LoadScene("battle");
    }
}
