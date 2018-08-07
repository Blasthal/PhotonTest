using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manage
	: MonoBehaviour
{
    private const string Version = "v1.0.0";
	private bool keyLock = false;


    #region Unity Method
	// Use this for initialization
	void Start ()
	 {
		keyLock = false;

		// Photon Realtimeのサーバーへ接続、ロビーへ入室
		PhotonNetwork.ConnectUsingSettings(Version);
    }

	void FixedUpdate()
	{
		// 左クリックが押されたら、オブジェクトを読み込む
		if (Input.GetMouseButtonDown(0) && keyLock)
		{
			GameObject mySyncObj = PhotonNetwork.Instantiate(
				"Cube"
				, new Vector3(9.0f, 0.0f, 0.0f)
				, Quaternion.identity
				, 0
				);

			// 動きを付けるためにRigidbodyを取得し、力を加える
			Rigidbody mySyncObjRB = mySyncObj.GetComponent<Rigidbody>();
			mySyncObjRB.isKinematic = false;
			float randPow = Random.Range(1.0f, 5.0f);
			mySyncObjRB.AddForce(Vector3.left * randPow, ForceMode.Impulse);
        }
	}
    #endregion // Unity Method

    #region Photon Method
    // ロビーに入室した
    void OnJoinedLobby()
	{
		// とりあえずどこかのルームへ入室する
		PhotonNetwork.JoinRandomRoom();
	}

	// ルームへ入室した
	void OnJoinedRoom()
	{
		// キーをアンロック
		keyLock = true;

		// 入室が完了したことを出力
		Debug.Log("ルームへ入室しました");
	}

	// ルームの入室に失敗した
	void OnPhotonRandomJoinFailed()
	{
		// 自分でルームを作成して入室
		PhotonNetwork.CreateRoom(null);
	}

    //// Photonのシリアライズ？
    //void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.isWriting)
    //    {
    //        //このプレイヤーを所有している: 他プレイヤーにデータを送信
    //        //stream.SendNext((int)controllerScript._characterState);
    //        stream.SendNext(transform.position);
    //        stream.SendNext(transform.rotation);
    //    }
    //    else
    //    {
    //        //ネットワークプレイヤー, データを受信
    //        controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
    //        correctPlayerPos = (Vector3)stream.ReceiveNext();
    //        correctPlayerRot = (Quaternion)stream.ReceiveNext();
    //    }
    //}
    #endregion // Photon Method
}
