using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public enum Function {KinematicSeek, KinematicFlee, DynamicSeek, DynamicFlee};
    public enum State {Tagged, Untagged, Frozen, None};

    public Function mFunction;
    public State mState;
    public bool mArrive;

    [Header("Movement Variables")]
    public float mMaxAcceleration;
    public float mMaxVelocity;
    public float mMaxAngularVelocity;
    public float mSlowRadius;
    public float mArriveRadius;
    public float mTimeToArrive;
    public float mWanderRadius;
    public float mWanderChangeTimer;

    [Header("Assignment Variables")]
    public float mSlowSpeed;
    public float mSmallDistance;

    [Header("Game Objects")]
    public GameObject mFire;

    [Header("Materials")]
    public Material mTagged;
    public Material mUntagged;
    public Material mFrozen;
    public Material mFleeing;
    public Material mHelping;
    public Material mBlack;
    public Material mWhite;

    private Vector3 mVelocity;
    private Vector3 mAcceleration;
    private Vector3 mAngularVelocity;
    private Vector3 mDirection;
    private Vector3 mLookingAt;

    private float mTimer;

    private GameObject[] mNPCs;
    private GameObject mArena;
    private GameObject mTarget;
    private GameObject mGameController;

    private void Start()
    {
        mTimer = 0;
        mArena = GameObject.FindGameObjectWithTag("Arena");
        mGameController = GameObject.FindGameObjectWithTag("GameController");
    }

    private void Update () {
        // Check current state
        switch(mState)
        {
            case State.Frozen:
                mVelocity = Vector3.zero;
                break;
            case State.Tagged:
                AcquireTarget();

                if (mFunction == Function.KinematicSeek)
                    KinematicSeek();
                else
                    DynamicSeek();

                Orientate();
                break;
            case State.Untagged:
                Wander();
                break;
            case State.None:
                mVelocity = Vector3.zero;
                break;
        }

        ClampToArena();
    }

    private void KinematicSeek()
    {
        mDirection = mTarget.transform.position - transform.position;
        mVelocity = mMaxVelocity * mDirection.normalized;

        if (mDirection.magnitude < mArriveRadius)
            Arrive();

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

    private void Wander()
    {
        if (mTimer > 0)
            mTimer -= Time.deltaTime;
        else
        {
            if (!mTarget) mTarget = new GameObject();

            Vector2 randomCircle = Random.insideUnitCircle * mWanderRadius;
            mTarget.transform.position = transform.position + (transform.forward * 5) + new Vector3(randomCircle.x, 0, randomCircle.y);

            mTimer = mWanderChangeTimer;
        }

        KinematicSeek();
        Orientate();
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

    private bool CheckVectorSimilarities(Vector3 lhs, Vector3 rhs)
    {
        float precision = 0.1f;
        bool equal = true;

        if (Mathf.Abs(lhs.x - rhs.x) > precision) equal = false;
        if (Mathf.Abs(lhs.y - rhs.y) > precision) equal = false;
        if (Mathf.Abs(lhs.z - rhs.z) > precision) equal = false;

        return equal;
    }

    private void AcquireTarget()
    {
        float smallestDistance = float.MaxValue;

        foreach (GameObject NPC in GameObject.FindGameObjectsWithTag("NPC"))
        {
            if (NPC.GetComponent<NPCController>().mState == State.Untagged && (transform.position - NPC.transform.position).magnitude < smallestDistance)
            {
                mTarget = NPC;
                smallestDistance = (transform.position - NPC.transform.position).magnitude;
            }
        }
    }

    private void Arrive()
    {
        GameController gc = mGameController.GetComponent<GameController>();
        NPCController npcController = mTarget.GetComponent<NPCController>();
        
        if (mState == State.Tagged && npcController.mState == State.Untagged)
        {
            gc.OnNPCTouch(1, 0, -1);
            npcController.ChangeState(State.Frozen);
            AcquireTarget();
        }
        else if (mState == State.Untagged && npcController.mState == State.Frozen)
        {
            gc.OnNPCTouch(-1, 0, 1);
            npcController.ChangeState(State.Untagged);
        }
    }

    private void ChangeMaterial(Material material_1, Material material_2)
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().material = material_1;
        transform.GetChild(1).GetComponent<MeshRenderer>().material = material_1;
        transform.GetChild(2).GetComponent<MeshRenderer>().material = material_2;
        transform.GetChild(3).GetComponent<MeshRenderer>().material = material_2;
    }

    public void ChangeState(State state)
    {
        GameObject fire;

        switch (state)
        {
            case State.Tagged:
                AcquireTarget();
                mMaxVelocity = 15;
                ChangeMaterial(mTagged, mWhite);
                fire = Instantiate(mFire, transform.position, Quaternion.identity);
                fire.transform.parent = gameObject.transform;
                break;
            case State.Untagged:
                mTarget = null;
                if (transform.childCount > 4)
                    Destroy(transform.GetChild(4).gameObject);
                mMaxVelocity = 5;
                ChangeMaterial(mUntagged, mBlack);
                break;
            case State.Frozen:
                ChangeMaterial(mFrozen, mWhite);
                break;
        }

        mVelocity = Vector3.zero;

        mState = state;
    }

    public void SetTarget(GameObject target)
    {
        mTarget = target;
    }
}
