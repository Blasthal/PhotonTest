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
                    myAnimator.SetLayerWeight(0, 1.0f);
                }
            }
            // 撃破時
            currentHealth = VariableManage.currentHealth;
            if (currentHealth == 0.0f)
            {
                destroyTimer += Time.deltaTime;
                myAnimator.SetLayerWeight(0, destroyTimer * 2.0f);
                mySmoke.SetActive(true);
            }
            else
            {
                myAnimator.SetLayerWeight(0, 2.0f);
                mySmoke.SetActive(false);
                destroyTimer = 0.0f;
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
}
