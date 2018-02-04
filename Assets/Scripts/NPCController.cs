using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public enum Mode {Kinematic, Dynamic};
    public enum State {Tagged, Wander, Frozen, Help, Flee, None};

    public Mode mMode;
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
    public float mFleeDistance;

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
    private bool mIsHelping;
    private bool mIsFleeing;

    private GameObject[] mNPCs;
    private GameObject mArena;
    private GameObject mTarget;
    private GameObject mGameController;
    private GameObject mTaggedNPC;

    private void Start()
    {
        mTimer = 0;
        mIsHelping = mIsFleeing = false;
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
                TaggedAction();
                break;
            case State.Wander:
                WanderAction();
                break;
            case State.None:
                mVelocity = Vector3.zero;
                break;
            case State.Flee:
                FleeAction();
                break;
            case State.Help:
                HelpAction();
                break;
        }

        ClampToArena();
    }

    private void TaggedAction()
    {
        AcquireTarget();

        if (mMode == Mode.Kinematic)
            KinematicSeek();
        else
            DynamicSeek();

        Orientate();
    }

    private void WanderAction()
    {
        List<GameObject> frozenNPCs = new List<GameObject>();
        
        foreach (GameObject NPC in GameObject.FindGameObjectsWithTag("NPC"))
        {
            if (NPC.GetComponent<NPCController>().mState == State.Frozen)
                frozenNPCs.Add(NPC);
            else if (NPC.GetComponent<NPCController>().mState == State.Tagged)
                mTaggedNPC = NPC;
        }

        float taggedNPCDistance = (mTaggedNPC.transform.position - transform.position).magnitude;

        if (taggedNPCDistance < mFleeDistance)
        {
            ChangeState(State.Flee);
        }
        else if (frozenNPCs.Count > 0)
        {
            ChangeState(State.Help);
        }
        else
        {
            ChangeState(State.Wander);
            Wander();
        }

        Orientate();
    }

    private void FleeAction()
    {
        mTarget = mTaggedNPC;

        float taggedDistance = (mTarget.transform.position - transform.position).magnitude;

        if (taggedDistance > mFleeDistance)
            ChangeState(State.Wander);
        else
        {
            switch (mMode)
            {
                case Mode.Kinematic:
                    KinematicFlee();
                    break;
                case Mode.Dynamic:
                    DynamicFlee();
                    break;
            }
        }

        Orientate();
    }

    private void HelpAction()
    {
        float taggedDistance = (mTaggedNPC.transform.position - transform.position).magnitude;

        if (taggedDistance < mFleeDistance)
            ChangeState(State.Flee);
        else
        {
            float smallestDistance = float.MaxValue;
            int frozenCtr = 0;

            foreach (GameObject NPC in GameObject.FindGameObjectsWithTag("NPC"))
            {
                float distance = (NPC.transform.position - transform.position).magnitude;

                if (distance < smallestDistance && NPC.GetComponent<NPCController>().mState == State.Frozen)
                {
                    frozenCtr++;
                    mTarget = NPC;
                    smallestDistance = distance;
                }
            }

            if (frozenCtr > 0)
            {
                switch (mMode)
                {
                    case Mode.Kinematic:
                        KinematicSeek();
                        break;
                    case Mode.Dynamic:
                        DynamicSeek();
                        break;
                }
            }
            else
                ChangeState(State.Wander);
        }

        Orientate();
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

        if (mDirection.magnitude < mArriveRadius)
            Arrive();

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
            mTarget = new GameObject();

            Vector2 randomCircle = Random.insideUnitCircle * mWanderRadius;
            mTarget.transform.position = transform.position + (transform.forward * 5) + new Vector3(randomCircle.x, 0, randomCircle.y);

            mTimer = mWanderChangeTimer;
        }

        mDirection = mTarget.transform.position - transform.position;
        mVelocity = mMaxVelocity * mDirection.normalized;

        Debug.Log(mMaxVelocity);

        transform.position = transform.position + (mVelocity * Time.deltaTime);

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
            if ((NPC.GetComponent<NPCController>().mState == State.Wander || NPC.GetComponent<NPCController>().mState == State.Flee || NPC.GetComponent<NPCController>().mState == State.Help)
                && (transform.position - NPC.transform.position).magnitude < smallestDistance)
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
        
        if (mState == State.Tagged && (npcController.mState == State.Wander || npcController.mState == State.Flee || npcController.mState == State.Help))
        {
            npcController.ChangeState(State.Frozen);
            gc.OnNPCTouch(1, 0, -1);
            AcquireTarget();
        }
        else if (mState == State.Help && npcController.mState == State.Frozen)
        {
            npcController.ChangeState(State.Wander);
            gc.OnNPCTouch(-1, 0, 1);
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
        switch (state)
        {
            case State.Tagged:
                mMaxVelocity = 13;
                ChangeMaterial(mTagged, mWhite);
                if (transform.childCount < 5)
                {
                    GameObject fire = Instantiate(mFire, transform.position, Quaternion.identity);
                    fire.transform.parent = gameObject.transform;
                }
                AcquireTarget();
                break;

            case State.Wander:
                mMaxVelocity = 10;
                mTimer = 0;
                ChangeMaterial(mUntagged, mBlack);
                if (transform.childCount > 4)
                    Destroy(transform.GetChild(4).gameObject);
                break;

            case State.Frozen:
                ChangeMaterial(mFrozen, mWhite);
                break;

            case State.Help:
                ChangeMaterial(mHelping, mBlack);
                break;

            case State.Flee:
                ChangeMaterial(mFleeing, mBlack);
                break;
        }

        mAcceleration = Vector3.zero;
        mVelocity = Vector3.zero;

        mState = state;
    }
}
