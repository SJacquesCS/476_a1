using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public enum Function {KinematicSeek, KinematicFlee, DynamicSeek, DynamicFlee, Wander};
    public Function mFunction;
    public bool mArrive;

    public float mMaxAcceleration;
    public float mMaxVelocity;
    public float mMaxAngularVelocity;
    public float mSlowRadius;
    public float mArriveRadius;
    public float mTimeToArrive;
    public float mWanderRadius;
    public float mWanderChangeTimer;

    public GameObject mTarget;
    public GameObject mArena;

    private Vector3 mVelocity;
    private Vector3 mAcceleration;
    private Vector3 mAngularVelocity;
    private Vector3 mDirection;
    private Vector3 mLookingAt;

    private float mTimer;

    private GameObject[] NPCs;

    private void Awake()
    {
        if (mFunction == Function.Wander)
        {
            mTarget = new GameObject();
            Vector2 randomCircle = Random.insideUnitCircle * mWanderRadius;
            mTarget.transform.position = transform.position + (transform.forward * 5) + new Vector3(randomCircle.x, 0, randomCircle.y);
            mTimer = mWanderChangeTimer;
        }
        else if (!mTarget)
        {
            NPCs = GameObject.FindGameObjectsWithTag("NPC");
            int randomizer = Random.Range(0, NPCs.Length);
            mTarget = NPCs[randomizer];
        }
    }

    void Update () {

        switch(mFunction)
        {
            case Function.KinematicSeek:
                KinematicSeek();
                break;
            case Function.KinematicFlee:
                KinematicFlee();
                break;
            case Function.DynamicSeek:
                DynamicSeek();
                break;
            case Function.DynamicFlee:
                DynamicFlee();
                break;
            case Function.Wander:
                Wander();
                break;
        }

        Orientate();

        ClampToArena();
    }

    private void KinematicSeek()
    {
        mDirection = mTarget.transform.position - transform.position;
        mVelocity = mMaxVelocity * mDirection.normalized;

        if (mArrive)
            KinematicArrive();

        Debug.Log(mVelocity.magnitude);

        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }

    private void KinematicFlee()
    {
        mDirection = transform.position - mTarget.transform.position;
        mVelocity = mMaxVelocity * mDirection.normalized;
        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }

    private void DynamicSeek()
    {
        mDirection = mTarget.transform.position - transform.position;
        mAcceleration = mMaxAcceleration * mDirection.normalized;
        mVelocity = mVelocity + (mAcceleration * Time.deltaTime);

        if (mVelocity.magnitude > mMaxVelocity)
            mVelocity = Vector3.ClampMagnitude(mVelocity, mMaxVelocity);

        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }

    private void DynamicFlee()
    {
        mDirection = transform.position - mTarget.transform.position;
        mAcceleration = mMaxAcceleration * mDirection.normalized;
        mVelocity = mVelocity + (mAcceleration * Time.deltaTime);

        if (mVelocity.magnitude > mMaxVelocity)
            mVelocity = Vector3.ClampMagnitude(mVelocity, mMaxVelocity);

        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }

    private void Orientate()
    {
        if (mDirection.magnitude > mArriveRadius)
        {
            Vector3 newRot = Vector3.RotateTowards(transform.forward, mDirection, mMaxAngularVelocity * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newRot);
        }
    }

    private void KinematicArrive()
    {
        if (mDirection.magnitude < mArriveRadius)
            mVelocity = Vector3.zero;
        else if (mDirection.magnitude < mSlowRadius)
        {
            mVelocity = Vector3.Min(mMaxVelocity * mDirection.normalized, mDirection / mTimeToArrive);
        }
    }

    private void Wander()
    {
        if (mTimer > 0)
            mTimer -= Time.deltaTime;

        if (mTimer <= 0)
        {
            mTimer = mWanderChangeTimer;
            Vector2 randomCircle = Random.insideUnitCircle * mWanderRadius;
            mTarget.transform.position = transform.position + (transform.forward * 5) + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
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
