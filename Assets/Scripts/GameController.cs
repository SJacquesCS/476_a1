using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int totalNPC;

    public GameObject npcs;
    public GameObject arena;

    private int frozenCtr, taggedCtr, untaggedCtr;

    private void Start()
    {
        Setup(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeMode();
    }

    private void Setup(bool newSetup)
    {
        frozenCtr = 0;
        taggedCtr = 1;
        untaggedCtr = totalNPC - 1;

        int randomTagged;

        if (newSetup)
        {
            randomTagged = Random.Range(0, untaggedCtr);

            for (int i = 0; i < totalNPC; i++)
            {
                Vector3 randomLocation = new Vector3(
                    Random.Range((-arena.transform.localScale.x / 2), arena.transform.localScale.x / 2),
                    1.0f,
                    Random.Range((-arena.transform.localScale.z / 2), arena.transform.localScale.z / 2)
                );

                GameObject NPC = Instantiate(npcs, randomLocation, Quaternion.identity);

                if (i == randomTagged)
                    NPC.GetComponent<NPCController>().ChangeState(NPCController.State.Tagged);
                else
                    NPC.GetComponent<NPCController>().ChangeState(NPCController.State.Wander);
            }
        }
        else
        {
            GameObject[] NPCs = GameObject.FindGameObjectsWithTag("NPC");

            randomTagged = Random.Range(0, NPCs.Length - 1);

            for (int i = 0; i < NPCs.Length; i++)
            {
                if (i == randomTagged)
                    NPCs[i].GetComponent<NPCController>().ChangeState(NPCController.State.Tagged);
                else
                    NPCs[i].GetComponent<NPCController>().ChangeState(NPCController.State.Wander);
            }
        }
    }

    public void OnNPCTouch(int frozen, int tagged, int untagged)
    {
        frozenCtr += frozen;
        taggedCtr += tagged;
        untaggedCtr += untagged;

        if (frozenCtr >= totalNPC - 1)
        {
            Setup(false);
        }
    }

    private void ChangeMode()
    {
        foreach (GameObject NPC in GameObject.FindGameObjectsWithTag("NPC"))
        {
            switch (NPC.GetComponent<NPCController>().mMode)
            {
                case NPCController.Mode.Kinematic:
                    Debug.Log("Changing to Dynamic");
                    NPC.GetComponent<NPCController>().mMode = NPCController.Mode.Dynamic;
                    break;
                case NPCController.Mode.Dynamic:
                    Debug.Log("Changing to Kinematic");
                    NPC.GetComponent<NPCController>().mMode = NPCController.Mode.Kinematic;
                    break;
            }
        }
    }
}
