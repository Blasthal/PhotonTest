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

    void Start()
    {

    }


    void Update()
    {

    }

    public void jumpBattleScene()
    {
        VariableManage.initializeVariable();

        //Application.LoadLevel("battle");
        SceneManager.LoadScene("battle");
    }
}
