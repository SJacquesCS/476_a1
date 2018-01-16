using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public bool mType;
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

        mVelocity = (mTarget.transform.position - transform.position) / Mathf.Sqrt(2);

        Vector3 newPos = transform.position + (mVelocity * Time.deltaTime);

        transform.position = newPos;
	}
}
