using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public int myTeamID;
    private PhotonView myPV;
    private float idSendTimer;
    // レーダー用
    public GameObject rdSphere;
    public GameObject rdCube;
    private GameObject[] rdEnTemp;
    private GameObject[] rdFrTemp;
    private float playerRefreshTimer;
    private float distRefreshTimer;


    private void Start()
    {
        myTeamID = 0;
        myPV = PhotonView.Get(this.gameObject);
        idSendTimer = 0;
    }

    private void Update()
    {
        // チームIDがデフォルトのままであれば、代入する
        if (myPV.isMine)
        {
            idSendTimer += Time.deltaTime;
            if (3.0f < idSendTimer)
            {
                myTeamID = VariableManage.myTeamID;
                myPV.RPC("SyncMyID", PhotonTargets.Others, myTeamID);
                idSendTimer = 0.0f;
            }
        }

        // 味方のレーダー経路を表示
        if (VariableManage.myTeamID == myTeamID)
        {
            if (VariableManage.mapEnabled && myTeamID != 0)
            {
                rdSphere.SetActive(true);
            }
            else
            {
                rdSphere.SetActive(false);
            }
        }
        else
        {
                rdSphere.SetActive(false);
        }

        // 敵をレーダーに表示する
        if (myPV.isMine)
        {
            // 敵味方一覧を3.5秒間隔でリフレッシュ
            playerRefreshTimer += Time.deltaTime;
            if (3.5f <= playerRefreshTimer)
            {
                rdEnTemp = GameObject.FindGameObjectsWithTag("Enemy");
                rdFrTemp = GameObject.FindGameObjectsWithTag("Player");
            }
            // 全味方のレーダー範囲に敵がいるか2秒間隔で計算
            distRefreshTimer += Time.deltaTime;
            if (2.0f < distRefreshTimer && rdFrTemp != null)
            {
                // 全プレイヤーを取り出す
                foreach (GameObject player in rdFrTemp)
                {
                    // 全敵を取り出す
                    foreach (GameObject enemy in rdEnTemp)
                    {
                        // 総当たりで距離を算出し、レーダー範囲であればオブジェクトをONに
                        float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
                        if (dist < 180.0f && VariableManage.mapEnabled)
                        {
                            enemy.GetComponent<CharacterStatus>().rdCube.SetActive(true);
                        }
                        else
                        {
                            enemy.GetComponent<CharacterStatus>().rdCube.SetActive(false);
                        }
                    }
                }
            }

            // そもそもマップ表示画面でなければレーダー用オブジェクトは強制OFF
            if (!VariableManage.mapEnabled)
            {
                rdCube.SetActive(false);
            }
        }
    }

    [PunRPC]
    private void SyncMyID(int myID)
    {
        myTeamID = myID;
    }
}
