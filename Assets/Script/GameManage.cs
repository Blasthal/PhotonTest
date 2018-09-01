using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManage
    : MonoBehaviour
{
    private void Start()
    {
        // 初期設定
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        // Photon Realtime のサーバーへ接続（ロビーへ入出）
        PhotonNetwork.ConnectUsingSettings(null);
    }

    private void Update()
    {
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
        PhotonNetwork.CreateRoom(null);
    }

    // 無事にルームへ入室
    private void OnJoinedRoom()
    {
        // オブジェクトを読み込み
        //GameObject myPlayer = PhotonNetwork.Instantiate(
        PhotonNetwork.Instantiate(
            "character/t01"
            , new Vector3(440.0f, 15.0f, -560.0f)
            , Quaternion.identity
            , 0
            );
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
}
