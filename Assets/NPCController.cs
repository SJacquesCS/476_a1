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

        KinematicSeek(direction, lookingAt);
        //DynamicSeek(direction, lookingAt);

        //if (direction.magnitude < mArriveRadius)
        //    mVelocity = Vector3.zero;
        //else if (direction.magnitude < mSlowRadius)
        //{
        //    if ((mVelocity.magnitude / mTimeToArrive) < mMaxVelocity)
        //        mVelocity = mVelocity / mTimeToArrive;
        //}

        //if (mChangeTargetTimer <= 0)
        //{
        //    mChangeTargetTimer = 2.5f;
        //    int randomizer = Random.Range(0, NPCs.Length);
        //    mTarget = NPCs[randomizer];
        //}

        //if (mChangeTargetTimer > 0)
        //    mChangeTargetTimer -= 0.01f;

        float angle;

        angle = Vector3.Angle(lookingAt, direction);

        Debug.Log(angle);

        angle = transform.eulerAngles.y + 1;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle, transform.eulerAngles.z);
    }

    private void KinematicSeek(Vector3  direction, Vector3 lookingAt)
    {
        mVelocity = mMaxVelocity * (direction.normalized / Mathf.Sqrt(2));

        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }

    private void DynamicSeek(Vector3 direction, Vector3 lookingAt)
    {
        mAcceleration = mMaxAcceleration * (direction.normalized / Mathf.Sqrt(2));
        mVelocity = mVelocity + (mAcceleration * Time.deltaTime);

        Debug.Log(mVelocity.magnitude);

        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }
}
