using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public int myTeamID;
    private PhotonView myPV;
    private float idSendTimer;


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
    }

    [PunRPC]
    private void SyncMyID(int myID)
    {
        myTeamID = myID;
    }
}
