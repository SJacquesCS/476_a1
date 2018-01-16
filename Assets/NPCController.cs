using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public bool mType;
    public int mSpeed;

    private Rigidbody mRb;
    private Transform mTarget;
    private Vector3 mVelocity;
    private Vector3 mAngularVelocity;

    private void Awake()
    {
        GameObject[] NPCs = GameObject.FindGameObjectsWithTag("NPC");
        int randomizer = Random.Range(0, NPCs.Length);
        mTarget = NPCs[randomizer].transform;
        GetComponent<Rigidbody>().mass = Random.Range(1, 5);

        mRb = GetComponent<Rigidbody>();
    }

    void Update () {
        mVelocity = mSpeed * (1 / Mathf.Sqrt(2) * (mTarget.transform.position - transform.position).normalized);

        mRb.velocity = mVelocity;
	}
}
