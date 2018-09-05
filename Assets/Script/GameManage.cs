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

    // ほか
    private bool loadOnce;


    private void Start()
    {
        // 初期設定
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        myTeamID = 0;
        loadOnce = false;
        scenePV = PhotonView.Get(this.gameObject);
        tagTimer = 0.0f;

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

        // Photon Realtime のサーバーへ接続（ロビーへ入出）
        PhotonNetwork.ConnectUsingSettings(null);
    }

    private void Update()
    {
        // ルームに入室が完了していたら
        if (PhotonNetwork.inRoom)
        {
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
                    , new Vector3(myStartPos.x, 24.0f, myStartPos.y)
                    , Quaternion.identity
                    , 0
                    );
                myPlayer.transform.LookAt(Vector3.zero);
                VariableManage.myTeamID = myTeamID;
            }
            // 3秒ごとに一回タグ付け
            tagTimer += Time.deltaTime;
            if (3.0f < tagTimer)
            {
                giveEnemyFlag();
                tagTimer = 0.0f;
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
}
