using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManage
    : MonoBehaviour
{
    // Photon用変数定義
    ExitGames.Client.Photon.Hashtable myPlayerHash;
    ExitGames.Client.Photon.Hashtable myRoomHash;
    string[] roomProps = { "time" };
    private PhotonView scenePV;

    // 敵味方判別
    public int myTeamID;
    private float tagTimer;

    // スタート地点用
    private Vector2 myStartPos;

    // 勝敗情報用
    private int tc1tmp;
    private int tc2tmp;
    private float bc1tmp;
    private float bc2tmp;
    private bool sendOnce;
    private string standardTime;
    private string serverTime;
    private bool countStart;

    // ほか
    private bool loadOnce;
    private float shiftTimer;


    private void Start()
    {
        // 初期設定
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        myTeamID = 0;
        loadOnce = false;
        scenePV = PhotonView.Get(this.gameObject);
        tagTimer = 0.0f;

        sendOnce = false;
        tc1tmp = VariableManage.team1Rest;
        tc2tmp = VariableManage.team1Rest;
        bc1tmp = VariableManage.base1Rest;
        bc2tmp = VariableManage.base2Rest;
        standardTime = "";
        serverTime = "";
        countStart = false;

        // スタート地点計算
        Vector2 rndPos = Vector2.zero;
        while (true)
        {
            rndPos = Random.insideUnitCircle * 150.0f;
            if (rndPos.x < -20.0f)
            {
                if (20.0f < rndPos.y)
                {
                    break;
                }
            }
        }
        myStartPos = new Vector2((592.0f + rndPos.x), (-592 + rndPos.y));

        shiftTimer = 0.0f;

        // Photon Realtime のサーバーへ接続（ロビーへ入出）
        PhotonNetwork.ConnectUsingSettings(null);
    }

    private void Update()
    {
        // ルームに入室が完了していたら
        if (PhotonNetwork.inRoom)
        {
            // ルームのカスタムプロパティへ基準時間を設定
            if (PhotonNetwork.isMasterClient && !countStart)
            {
                myRoomHash["time"] = PhotonNetwork.time.ToString();
                PhotonNetwork.room.SetCustomProperties(myRoomHash);
                countStart = true;
            }
            else if (!countStart)
            {
                // ルームの基準時間を取得
                if (standardTime == "" && standardTime != "0")
                {
                    standardTime = PhotonNetwork.room.CustomProperties["time"].ToString();
                }

                // 現在の基準時間を取得
                if (serverTime == "" && serverTime != "0")
                {
                    serverTime = PhotonNetwork.time.ToString();
                }

                // 時間を比較し、残り時間を算出
                if (standardTime != "" && standardTime != "0"
                    && serverTime != "" && serverTime != "0"
                    )
                {
                    float svT = float.Parse(double.Parse(serverTime).ToString());
                    float stT = float.Parse(double.Parse(standardTime).ToString());

                    Debug.Log(svT + "_" + stT);

                    VariableManage.timeRest = VariableManage.timeRest - Mathf.Round(svT - stT);
                    countStart = true;
                }
            }

            // チーム分けが完了したらオブジェクトを読み込み
            if (!loadOnce && myTeamID != 0)
            {
                loadOnce = true;
                if (myTeamID == 2)
                {
                    myStartPos = myStartPos * -1.0f;
                }
                GameObject myPlayer = PhotonNetwork.Instantiate(
                    "character/t01"
                    , new Vector3(myStartPos.x, 15.0f, myStartPos.y)
                    , Quaternion.identity
                    , 0
                    );
                myPlayer.transform.LookAt(Vector3.zero);
                VariableManage.myTeamID = myTeamID;
            }

            // マスタークライアントで拠点が攻撃された場合、全クライアントへ送信
            if (PhotonNetwork.isMasterClient)
            {
                // 拠点1の耐久力を減らす
                if (VariableManage.team1baseBullet != null)
                {
                    bc1tmp -= VariableManage.team1baseBullet.GetComponent<mainShell>().pow;
                    if (bc1tmp < 0)
                    {
                        bc1tmp = 0;
                    }
                    VariableManage.team1baseBullet = null;
                }
                // 拠点2の耐久力を減らす
                if (VariableManage.team2baseBullet != null)
                {
                    bc2tmp -= VariableManage.team2baseBullet.GetComponent<mainShell>().pow;
                    if (bc2tmp < 0)
                    {
                        bc2tmp = 0;
                    }
                    VariableManage.team2baseBullet = null;
                }
            }

            // 撃破されたとき、マスタークライアントへ情報を送信
            // 撃破されたとき、全プレイヤーに情報を送信しメッセージを表示
            if (VariableManage.currentHealth <= 0.0f)
            {
                if (!sendOnce)
                {
                    sendOnce = true;
                    scenePV.RPC(
                        "SendDestruction"
                        , PhotonNetwork.masterClient
                        , VariableManage.myTeamID
                        );
                    scenePV.RPC(
                        "SendDestructionAll"
                        , PhotonTargets.All
                        , VariableManage.myTeamID
                        );
                }
                else
                {
                    sendOnce = false;
                }

                // 勝敗を確定
                if (PhotonNetwork.isMasterClient && !VariableManage.finishedGame)
                {
                    if (VariableManage.team1Rest <= 0
                        || VariableManage.base1Rest <= 0.0f
                        )
                    {
                        VariableManage.finishedGame = true;
                        VariableManage.gameResult = 2;
                        scenePV.RPC(
                            "SyncFinished"
                            , PhotonTargets.Others
                            , VariableManage.gameResult
                            );
                    }
                    else if (VariableManage.team2Rest <= 0.0f
                        || VariableManage.base2Rest <= 0.0f
                        )
                    {
                        VariableManage.finishedGame = true;
                        VariableManage.gameResult = 1;
                        scenePV.RPC(
                            "SyncFinished"
                            , PhotonTargets.Others
                            , VariableManage.gameResult
                            );
                    }

                    // 時間切れによる決着　より撃破数が多いほうが勝ち
                    // ただし引き分けの場合は拠点の耐久力を参照し
                    // それでもダメならチーム1の勝ち
                    if (VariableManage.timeRest <= 0)
                    {
                        if (VariableManage.team1Rest > VariableManage.team2Rest)
                        {
                            // t1 win
                            VariableManage.finishedGame = true;
                            VariableManage.gameResult = 1;
                            scenePV.RPC(
                                "syncFinished"
                                , PhotonTargets.Others
                                , VariableManage.gameResult
                                );
                        }
                        else if (VariableManage.team1Rest < VariableManage.team2Rest)
                        {
                            // t2 win
                            VariableManage.finishedGame = true;
                            VariableManage.gameResult = 2;
                            scenePV.RPC(
                                "syncFinished"
                                , PhotonTargets.Others
                                , VariableManage.gameResult
                                );
                        }
                        else
                        {
                            // draw
                            if (VariableManage.base1Rest >= VariableManage.base2Rest)
                            {
                                VariableManage.finishedGame = true;
                                VariableManage.gameResult = 1;
                                scenePV.RPC(
                                    "syncFinished"
                                    , PhotonTargets.Others
                                    , VariableManage.gameResult
                                    );
                            }
                            else
                            {
                                VariableManage.finishedGame = true;
                                VariableManage.gameResult = 2;
                                scenePV.RPC(
                                    "syncFinished"
                                    , PhotonTargets.Others
                                    , VariableManage.gameResult
                                    );
                            }
                        }
                    }
                }

                //
            }

            // 時間経過
            if (countStart)
            {
                VariableManage.timeRest -= Time.deltaTime;
                if (VariableManage.timeRest < 0)
                {
                    VariableManage.timeRest = 0;
                }
            }

            // 3秒ごとに一回タグ付け
            tagTimer += Time.deltaTime;
            if (3.0f < tagTimer)
            {
                giveEnemyFlag();
                tagTimer = 0.0f;
            }

            // 決着後、メインメニューへ移動
            if (VariableManage.finishedGame && VariableManage.gameResult != 0.0f)
            {
                shiftTimer += Time.deltaTime;
                // 5秒後に移動
                if (shiftTimer > 5.0f)
                {
                    PhotonNetwork.Disconnect();
                    //Application.LoadLevel("mainMenu");
                    SceneManager.LoadScene("mainMenu");
                }
            }
        }
    }


    // ロビーへ入室完了
    private void OnJoinedLobby()
    {
        // どこかのルームへ入出
        PhotonNetwork.JoinRandomRoom();
    }

    // ロビーへの入室が失敗
    private void OnFailedToConnectToPhoton()
    {
        //Application.LoadLevel("mainMenu");
        SceneManager.LoadScene("mainMenu");
    }

    // ルームへ入室失敗
    private void OnPhotonRandomJoinFailed()
    {
        // 自分でルームを作成
        myRoomHash = new ExitGames.Client.Photon.Hashtable();
        myRoomHash.Add("time", 0);

        //PhotonNetwork.CreateRoom(
        //    Random.Range(1.0f, 100.0f).ToString()
        //    , true
        //    , true
        //    , 8
        //    , myRoomHash
        //    , roomProps
        //    );

        // サンプルとversionが違うのでそれらしく処理する
        RoomOptions options = new RoomOptions();
        options.IsOpen = true;
        options.IsVisible = true;
        options.MaxPlayers = 8;
        // ★特にサンプルと違う部分
        options.CustomRoomProperties = myRoomHash;

        PhotonNetwork.CreateRoom(
            Random.Range(1.0f, 100.0f).ToString()
            , options
            , TypedLobby.Default
            );

        myTeamID = 1;
    }

    // 無事にルームへ入室
    private void OnJoinedRoom()
    {
        //// オブジェクトを読み込み
        ////GameObject myPlayer = PhotonNetwork.Instantiate(
        //PhotonNetwork.Instantiate(
        //    "character/t01"
        //    , new Vector3(440.0f, 15.0f, -560.0f)
        //    , Quaternion.identity
        //    , 0
        //    );
    }

    // Photon Realtime との接続が切断された場合
    private void OnConnectionFail()
    {
        //Application.LoadLevel("mainMenu");
        SceneManager.LoadScene("mainMenu");
    }
    
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            //Application.LoadLevel("mainMenu");
            SceneManager.LoadScene("mainMenu");
        }
    }

    // ルームに誰か別のプレイヤーが入室したとき
    private void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        // 自分がマスタークライアントだったとき
        if (PhotonNetwork.isMasterClient)
        {
            // メンバー振り分け処理
            int allocateNumber = 0;
            // 現在のチーム状況を取得
            GameObject[] team1Players = GameObject.FindGameObjectsWithTag("Player");
            GameObject[] team2Players = GameObject.FindGameObjectsWithTag("Enemy");
            if (team1Players.Length - 1 >= team2Players.Length)
            {
                // playerの方が多い場合
                if (myTeamID == 1) { allocateNumber = 2; }
                else { allocateNumber = 1; }
                scenePV.RPC("allocateTeam", newPlayer, allocateNumber);
            }
            else
            {
                // enmeyの方が多い場合
                if (myTeamID == 2) { allocateNumber = 2; }
                else { allocateNumber = 1; }
                scenePV.RPC("allocateTeam", newPlayer, allocateNumber);
            }

            // 現在の状況を送信
            scenePV.RPC(
                "SendCurrentStatus"
                , newPlayer
                , VariableManage.team1Rest
                , VariableManage.team2Rest
                , VariableManage.base1Rest
                , VariableManage.base2Rest
                );
        }
    }

    // 敵に対してタグ付けを行う
    private void giveEnemyFlag()
    {
        // チームIDが定義されていたら
        if (myTeamID != 0)
        {
            int enID = 1;
            if (myTeamID == 1) { enID = 2; }
            GameObject[] allFriendPlayer = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in allFriendPlayer)
            {
                int id = player.GetComponent<CharacterStatus>().myTeamID;
                if (id == enID)
                {
                    player.tag = "Enemy";
                }
            }
        }
    }

    [PunRPC]
    private void allocateTeam(int teamID)
    {
        if (myTeamID == 0)
        {
            myTeamID = teamID;
        }
    }

    // 自分が撃破されたことを送信するRPC
    [PunRPC]
    private void SendDestruction(int tID)
    {
        if (tID == 1)
        {
            tc1tmp -= 1;

            if (tc1tmp < 0)
            {
                tc1tmp = 0;
            }
        }
        else
        {
            tc2tmp -= 1;

            if (tc2tmp < 0)
            {
                tc2tmp = 0;
            }
        }
    }

    // 自分が撃破されたことを全プレイヤーに送信する
    [PunRPC]
    private void SendDestructionAll(int tID)
    {
        if (myTeamID == tID)
        {
            VariableManage.infomationMessage = 1;
        }
        else
        {
            VariableManage.infomationMessage = 2;
        }
    }

    // 現在の対戦状況を送信するRPC
    [PunRPC]
    private void SendCurrentStatus(int tc1, int tc2, float bc1, float bc2)
    {
        VariableManage.team1Rest = tc1;
        VariableManage.team2Rest = tc2;
        VariableManage.base1Rest = bc1;
        VariableManage.base2Rest = bc2;
    }

    // ゲーム終了を通知するRPC
    [PunRPC]
    private void SyncFinished(int winID)
    {
        VariableManage.finishedGame = true;
        VariableManage.gameResult = winID;
    }
}
