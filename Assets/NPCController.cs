using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public bool mType;
    public float mMaxAcceleration;
    public float mMaxAngularVelocity;
    public GameObject mTarget;

    private Vector3 mVelocity;
    private Vector3 mAcceleration;
    private Vector3 mAngularVelocity;

    private void Awake()
    {
        if (!mTarget)
        {
            GameObject[] NPCs = GameObject.FindGameObjectsWithTag("NPC");
            int randomizer = Random.Range(0, NPCs.Length);
            mTarget = NPCs[randomizer];
        }

        Debug.Log(Mathf.PI / 2 + ", " + Mathf.PI / 2 * Mathf.Rad2Deg);

        transform.eulerAngles = new Vector3(1, (Mathf.PI / 2) * Mathf.Rad2Deg, 1);
    }

    void Update () {
        Vector3 direction = mTarget.transform.position - transform.position;
        Vector3 lookingAt = transform.eulerAngles;

        mAcceleration = mMaxAcceleration * (direction.normalized / Mathf.Sqrt(2));
        mVelocity = mVelocity + (mAcceleration * Time.deltaTime);

        transform.position = transform.position + (mVelocity * Time.deltaTime);

        //float angle = Vector3.Angle(lookingAt, direction);
        //Debug.Log(angle);
        //Vector3 newRot = transform.rotation.eulerAngles + (new Vector3(1, (angle), 1) * Time.deltaTime);
        //transform.rotation = Quaternion.Euler(newRot);
    }
}
