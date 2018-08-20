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

    // オブジェクト格納
    public Rigidbody myRigid;


    private void Start()
    {
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
    }

    private void FixedUpdate()
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
}
