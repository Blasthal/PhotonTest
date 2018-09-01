using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController
    : MonoBehaviour
{
    public Animator myAnimator;
    private PhotonView myPV;
    private Rigidbody myRigid;
    public GameObject mySmoke;
    private bool hitFlag;
    private float hitFlagTimer;
    private float currentHealth;
    private float destroyTimer;
    // アニメーション同期用
    private string myAnimStatus;
    public Transform yRotObj;
    public Transform xRotObj;


    private void Start()
    {
        hitFlag = false;
        hitFlagTimer = 0.0f;
        destroyTimer = 0.0f;
        myPV = PhotonView.Get(this.gameObject);
        myRigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (myPV.isMine)
        {
            // 移動時
            float myAnimSpd = myRigid.velocity.magnitude / 24.0f;
            myAnimator.speed = myAnimSpd;
            // 被弾時
            if (hitFlag)
            {
                myAnimator.SetLayerWeight(1, hitFlagTimer * 2.0f);
                hitFlagTimer += Time.deltaTime;
                if (2.0f < hitFlagTimer)
                {
                    hitFlag = false;
                    hitFlagTimer = 0.0f;
                    myAnimator.SetLayerWeight(1, 0.0f);
                }
            }
            // 撃破時
            currentHealth = VariableManage.currentHealth;
            if (currentHealth == 0.0f)
            {
                destroyTimer += Time.deltaTime;
                myAnimator.SetLayerWeight(2, destroyTimer * 2.0f);
                mySmoke.SetActive(true);
            }
            else
            {
                myAnimator.SetLayerWeight(2, 0.0f);
                mySmoke.SetActive(false);
                destroyTimer = 0.0f;
            }

            // アニメーション同期用
            int layerWeight_1 = 0;
            int layerWeight_2 = 0;
            if (1.0f <= myAnimator.GetLayerWeight(1))
            {
                layerWeight_1 = 1;
            }
            if (1.0f <= myAnimator.GetLayerWeight(2))
            {
                layerWeight_2 = 1;
            }
            string yRotTmp = Mathf.RoundToInt(yRotObj.localRotation.eulerAngles.y).ToString();
            string xRotTmp = Mathf.RoundToInt(xRotObj.localRotation.eulerAngles.x).ToString();
            string yRotTmp2 = "";
            string xRotTmp2 = "";
            for (int i = 3; yRotTmp.Length < i; --i)
            {
                yRotTmp2 = "0" + yRotTmp2;
            }
            for (int i = 3; xRotTmp.Length < i; --i)
            {
                xRotTmp2 = "0" + xRotTmp2;
            }
            yRotTmp = yRotTmp2 + yRotTmp;
            xRotTmp = xRotTmp2 + xRotTmp;
            int myAnimSpdTmp = Mathf.RoundToInt(myAnimSpd * 100.0f);
            // 各要素を繋げて格納
            myAnimStatus = yRotTmp + xRotTmp + layerWeight_1 + layerWeight_2 + myAnimSpdTmp;
        }
        else
        {
            // ほかのプレイヤーに映る自分自身のアニメーション
            if (myAnimStatus != "" && myAnimStatus != null)
            {
                yRotObj.localRotation = Quaternion.Euler(new Vector3(0.0f, float.Parse(myAnimStatus.Substring(0, 3)), 0.0f));
                xRotObj.localRotation = Quaternion.Euler(new Vector3(0.0f, float.Parse(myAnimStatus.Substring(3, 3)), 0.0f));
                myAnimator.SetLayerWeight(1, float.Parse(myAnimStatus.Substring(6, 1)));
                myAnimator.SetLayerWeight(2, float.Parse(myAnimStatus.Substring(7, 1)));
                myAnimator.speed = float.Parse(myAnimStatus.Substring(8));

                if (1.0f <= myAnimator.GetLayerWeight(2))
                {
                    mySmoke.SetActive(true);
                }
                else
                {
                    mySmoke.SetActive(false);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // bulletレイヤーに処理を限定
        if (collision.gameObject.layer == 9)
        {
            hitFlag = true;
            Debug.Log("#hit");
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext((string)myAnimStatus);
        }
        else
        {
            myAnimStatus = (string)stream.ReceiveNext();
        }
    }
}
