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

        int randomTagged = Random.Range(0, untaggedCtr);

        if (newSetup)
        {
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
                    NPC.GetComponent<NPCController>().ChangeState(NPCController.State.Untagged);
            }
        }
        else
        {
            GameObject[] NPCs = GameObject.FindGameObjectsWithTag("NPC");

            Debug.Log(NPCs.Length);

            for (int i = 0; i < NPCs.Length; i++)
            {
                if (i == randomTagged)
                    NPCs[i].GetComponent<NPCController>().ChangeState(NPCController.State.Tagged);
                else
                    NPCs[i].GetComponent<NPCController>().ChangeState(NPCController.State.Untagged);
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
            switch (NPC.GetComponent<NPCController>().mFunction)
            {
                case NPCController.Function.KinematicSeek:
                    Debug.Log("Changing to Dynamic");
                    NPC.GetComponent<NPCController>().mFunction = NPCController.Function.DynamicSeek;
                    break;
                case NPCController.Function.DynamicSeek:
                    Debug.Log("Changing to Kinematic");
                    NPC.GetComponent<NPCController>().mFunction = NPCController.Function.KinematicSeek;
                    break;
                case NPCController.Function.KinematicFlee:
                    NPC.GetComponent<NPCController>().mFunction = NPCController.Function.DynamicFlee;
                    break;
                case NPCController.Function.DynamicFlee:
                    NPC.GetComponent<NPCController>().mFunction = NPCController.Function.KinematicFlee;
                    break;
            }
        }
    }
}
