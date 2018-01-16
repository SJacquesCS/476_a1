using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public bool mType;
    public float mMaxVelocity;
    public float mMaxAngularVelocity;
    public GameObject mTarget;

    private Vector3 mVelocity;
    private Vector3 mAngularVelocity;

    private void Awake()
    {
        if (!mTarget)
        {
            GameObject[] NPCs = GameObject.FindGameObjectsWithTag("NPC");
            int randomizer = Random.Range(0, NPCs.Length);
            mTarget = NPCs[randomizer];
        }
    }

    void Update () {
        Vector3 direction = mTarget.transform.position - transform.position;
        mVelocity = mMaxVelocity * direction.normalized / Mathf.Sqrt(2);
        Vector3 currentRot = transform.rotation.eulerAngles;
        mAngularVelocity = mMaxAngularVelocity * direction.normalized / Mathf.Sqrt(2);


        Vector3 newPos = transform.position + (mVelocity * Time.deltaTime);
        //Vector3 newAngle = 

        transform.position = newPos;
	}
}
