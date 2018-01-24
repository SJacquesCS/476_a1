using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public bool mKinematic;
    public bool mSeek;
    public float mMaxAcceleration;
    public float mMaxVelocity;
    public float mMaxAngularVelocity;
    public float mSlowRadius;
    public float mArriveRadius;
    public float mTimeToArrive;

    public GameObject mTarget;
    public GameObject mArena;

    private Vector3 mVelocity;
    private Vector3 mAcceleration;
    private Vector3 mAngularVelocity;
    private Vector3 mDirection;
    private Vector3 mLookingAt;

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

        if (mKinematic)
        {
            if (mSeek)
                KinematicSeek();
            else
                KinematicFlee();
        }
        else
        {
            if (mSeek)
                DynamicSeek();
            else
                DynamicFlee();
        }

        ClampToArena();

        //if (mChangeTargetTimer <= 0)
        //{
        //    mChangeTargetTimer = 2.5f;
        //    int randomizer = Random.Range(0, NPCs.Length);
        //    mTarget = NPCs[randomizer];
        //}

        //if (mChangeTargetTimer > 0)
        //    mChangeTargetTimer -= 0.01f;

        Vector3 newRot = Vector3.RotateTowards(transform.forward, mDirection, mMaxAngularVelocity * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newRot);
    }

    private void KinematicSeek()
    {
        mDirection = mTarget.transform.position - transform.position;
        mVelocity = mMaxVelocity * (mDirection.normalized / Mathf.Sqrt(2));
        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }

    private void KinematicFlee()
    {
        mDirection = transform.position - mTarget.transform.position;
        mVelocity = mMaxVelocity * (mDirection.normalized / Mathf.Sqrt(2));
        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }

    private void DynamicSeek()
    {
        mDirection = mTarget.transform.position - transform.position;
        mAcceleration = mMaxAcceleration * (mDirection.normalized / Mathf.Sqrt(2));
        mVelocity = mVelocity + (mAcceleration * Time.deltaTime);

        if (mVelocity.magnitude > mMaxVelocity)
            mVelocity = Vector3.ClampMagnitude(mVelocity, mMaxVelocity);

        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }

    private void DynamicFlee()
    {
        mDirection = transform.position - mTarget.transform.position;
        mAcceleration = mMaxAcceleration * (mDirection.normalized / Mathf.Sqrt(2));
        mVelocity = mVelocity + (mAcceleration * Time.deltaTime);

        if (mVelocity.magnitude > mMaxVelocity)
            mVelocity = Vector3.ClampMagnitude(mVelocity, mMaxVelocity);

        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }

    private void KinematicArrive()
    {
        //if (direction.magnitude < mArriveRadius)
        //    mVelocity = Vector3.zero;
        //else if (direction.magnitude < mSlowRadius)
        //{
        //    if ((mVelocity.magnitude / mTimeToArrive) < mMaxVelocity)
        //        mVelocity = mVelocity / mTimeToArrive;
        //}
    }

    private void ClampToArena()
    {
        if (transform.position.x > mArena.transform.position.x + (mArena.transform.localScale.x / 2))
            transform.position = new Vector3(mArena.transform.position.x - (mArena.transform.localScale.x / 2), transform.position.y, transform.position.z);
        else if (transform.position.x < mArena.transform.position.x - (mArena.transform.localScale.x / 2))
            transform.position = new Vector3(mArena.transform.position.x + (mArena.transform.localScale.x / 2), transform.position.y, transform.position.z);

        if (transform.position.z > mArena.transform.position.z + (mArena.transform.localScale.z / 2))
            transform.position = new Vector3(transform.position.x, transform.position.y, mArena.transform.position.z - (mArena.transform.localScale.z / 2));
        else if (transform.position.z < mArena.transform.position.z - (mArena.transform.localScale.z / 2))
            transform.position = new Vector3(transform.position.x, transform.position.y, mArena.transform.position.z + (mArena.transform.localScale.z / 2));
    }
}
