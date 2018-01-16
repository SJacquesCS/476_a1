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

        Debug.Log(Mathf.PI / 2 + ", " + Mathf.PI / 2 * Mathf.Rad2Deg);

        transform.eulerAngles = new Vector3(1, (Mathf.PI / 2) * Mathf.Rad2Deg, 1);
    }

    void Update () {
        Vector3 direction = mTarget.transform.position - transform.position;
        Vector3 lookingAt = transform.rotation.eulerAngles;

        mVelocity = mMaxVelocity * direction.normalized / Mathf.Sqrt(2);

        float angle = Mathf.Cos((Vector3.Dot(lookingAt, direction)) / (lookingAt.magnitude * direction.magnitude));

        Debug.Log(angle);

        Vector3 newPos = transform.position + (mVelocity * Time.deltaTime);
        Vector3 newRot = transform.rotation.eulerAngles + (new Vector3(1, (angle * Mathf.Rad2Deg), 1) * Time.deltaTime);

        transform.rotation = Quaternion.Euler(newRot);
        transform.position = newPos;
    }
}
