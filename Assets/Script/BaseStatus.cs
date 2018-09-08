using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStatus
    : MonoBehaviour
{
    // どちらのチームの得点か
    public int baseID;


    // 拠点に被弾した
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9)
        {
            if (baseID == 1)
            {
                VariableManage.team1baseBullet = collision.gameObject;
            }
            else
            {
                VariableManage.team2baseBullet = collision.gameObject;
            }
        }
    }
}
