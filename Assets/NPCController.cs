using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public bool mType;
    public float mMaxAcceleration;
    public float mMaxVelocity;
    public float mMaxAngularVelocity;
    public float mSlowRadius;
    public float mArriveRadius;
    public float mTimeToArrive;
    public GameObject mTarget;

    private Vector3 mVelocity;
    private Vector3 mAcceleration;
    private Vector3 mAngularVelocity;

    private float mChangeTargetTimer;

    private GameObject[] NPCs;

    private void Awake()
    {
        if (!mTarget)
        {
            NPCs = GameObject.FindGameObjectsWithTag("NPC");
            int randomizer = Random.Range(0, NPCs.Length);
            mTarget = NPCs[randomizer];
        }

        mChangeTargetTimer = 2.5f;
    }

    void Update () {
        Vector3 direction = mTarget.transform.position - transform.position;
        Vector3 lookingAt = transform.eulerAngles;

        Seek(direction, lookingAt);

        if (direction.magnitude < mArriveRadius)
            mVelocity = Vector3.zero;
        else if (direction.magnitude < mSlowRadius)
        {
            if ((mVelocity.magnitude / mTimeToArrive) < mMaxVelocity)
                mVelocity = mVelocity / mTimeToArrive;
        }

        if (mChangeTargetTimer <= 0)
        {
            mChangeTargetTimer = 2.5f;
            int randomizer = Random.Range(0, NPCs.Length);
            mTarget = NPCs[randomizer];
        }

        if (mChangeTargetTimer > 0)
            mChangeTargetTimer -= 0.05f;

        //float angle = Vector3.Angle(lookingAt, direction);

        //if (mVelocity.magnitude > mMaxVelocity)
        //    mVelocity = new Vector3(mMaxVelocity * Mathf.Sin(angle), mVelocity.y, mMaxVelocity * Mathf.Cos(angle));

        //Debug.Log(angle);
        //Vector3 newRot = transform.rotation.eulerAngles + (new Vector3(1, (angle), 1) * Time.deltaTime);
        //transform.rotation = Quaternion.Euler(newRot);
    }

    private void Seek(Vector3 direction, Vector3 lookingAt)
    {
        mAcceleration = mMaxAcceleration * (direction.normalized / Mathf.Sqrt(2));
        mVelocity = mVelocity + (mAcceleration * Time.deltaTime);

        Debug.Log(mVelocity.magnitude);

        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }
}
