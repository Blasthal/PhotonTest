using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactreMove
    : MonoBehaviour
{
    // 機体のパラメータ設定
    public float maxSpd;
    public float cornering;
    public float basePower;
    public float maxHealth;

    // オブジェクト格納
    public Rigidbody myRigid;
    public PhotonView myPV;
    public Camera myCam;
    private GameObject hitObject;
    // 撃破時
    private float revivalTimer;


    private void Start()
    {
        // 自分が読み込んだオブジェクトではない場合
        if (!myPV.isMine)
        {
            myRigid.isKinematic = true;
            myCam.transform.gameObject.SetActive(false);

            // このスクリプトは破棄する
            Destroy(this);
        }
        // ほか
        revivalTimer = 0.0f;
        VariableManage.currentHealth = maxHealth;
    }

    private void Update()
    {
        // PC動作確認
        if (!Application.isMobilePlatform)
        {
            if (Input.GetKey(KeyCode.W))
            {
                VariableManage.movingYaxis = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                VariableManage.movingYaxis = -1;
            }
            else
            {
                VariableManage.movingYaxis = 0;
            }
            if (Input.GetKey(KeyCode.A))
            {
                VariableManage.movingXaxis = 1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                VariableManage.movingXaxis = -1;
            }
            else
            {
                VariableManage.movingXaxis = 0;
            }
        }

        // 被弾処理
        if (hitObject != null)
        {
            // スクリプトを取得
            mainShell hitShell = hitObject.GetComponent<mainShell>();
            // ダメージ
            VariableManage.currentHealth -= hitShell.pow;
            if (VariableManage.currentHealth < 0)
            {
                VariableManage.currentHealth = 0;
            }
            // オブジェクトを空にする
            hitObject = null;
        }
        // hpが0になったとき
        if (VariableManage.currentHealth == 0.0f)
        {
            revivalTimer += Time.deltaTime;
            VariableManage.controlLock = true;
            if (10.0f < revivalTimer)
            {
                revivalTimer = 0.0f;
                VariableManage.controlLock = false;
                VariableManage.currentHealth = maxHealth;
            }
            // 姿勢制御
            float xAngle = transform.rotation.eulerAngles.x;
            float zAngle = transform.rotation.eulerAngles.z;
            if (180.0f < xAngle) { xAngle = xAngle - 360.0f; }
            if (180.0f < zAngle) { zAngle = zAngle - 360.0f; }

            if (30.0f < xAngle) { xAngle = 30.0f; }
            else if (xAngle < -30.0f) { xAngle = -30.0f; }

            if (30.0f < zAngle) { zAngle = 30.0f; }
            else if (zAngle < -30.0f) { zAngle = -30.0f; }

            transform.rotation = Quaternion.Euler(
                new Vector3(
                    xAngle
                    , transform.rotation.eulerAngles.y
                    , zAngle)
                    );
        }
    }

    private void FixedUpdate()
    {
        if (!VariableManage.controlLock)
        {
            // 移動処理
            if (VariableManage.movingYaxis != 0)
            {
                if (myRigid.velocity.magnitude < maxSpd)
                {
                    Vector3 force = transform.TransformDirection(Vector3.forward)
                        * basePower
                        * 11.0f
                        * VariableManage.movingYaxis;
                    myRigid.AddForce(force);
                }
                // 旋回処理
                if (myRigid.angularVelocity.magnitude < myRigid.velocity.magnitude * 0.3f)
                {
                    Vector3 force = transform.TransformDirection(Vector3.up)
                        * cornering
                        * VariableManage.movingXaxis
                        * -90.0f;
                    myRigid.AddTorque(force);
                }
                else
                {
                    myRigid.angularVelocity = (myRigid.velocity.magnitude * 0.3f) * myRigid.angularVelocity.normalized;
                }
            }
        }
        // 姿勢制御
        Vector3 raycastStartPos =
            new Vector3(
                transform.position.x
                , transform.position.y + 1.0f
                , transform.position.z
                );
        RaycastHit rHit;
        if (!Physics.Raycast(
            raycastStartPos
            , transform.TransformDirection(-Vector3.up)
            , out rHit
            , 3.0f)
            )
        {
            // 地表に接していなければ下方向に力を与える
            myRigid.AddForce(Vector3.up * -50.0f);
        }
        //
    }

    // 衝突時に呼ばれる
    private void OnCollisionEnter(Collision collision)
    {
        // bulletレイヤーに処理を限定
        if (collision.gameObject.layer == 9)
        {
            hitObject = collision.gameObject;
        }
    }
}
