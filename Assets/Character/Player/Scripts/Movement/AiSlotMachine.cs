using UnityEngine;

public class AiSlotMachine : MonoBehaviour
{
    [Header("AiTargeting")]
    [SerializeField] public int slots = 10;
    [SerializeField] float radius = 2f;

    private NpcSlot[] aiSlots = new NpcSlot[0];
    private float dist;
    private float angleDepth;
    private int targetIndex;

    public static AiSlotMachine Instace;

    private void Awake()
    {
        Instace = this;
    }

    private void Start()
    {
        
        aiSlots = new NpcSlot[slots];
        angleDepth = 360f / slots;

        for (int i = 0; i < slots; i++)
        {
            Vector3 tempLocalSpot = Quaternion.Euler(0f, angleDepth * i, 0f) * (Vector3.forward * radius);
            aiSlots[i] = new NpcSlot { localPos = tempLocalSpot, taken = false, playerIndex = i , partent = transform};
        }
    }

    public NpcSlot RequestSlot(Vector3 aiPos)
    {
        dist = float.PositiveInfinity;
   
        for (int i = 0; i < aiSlots.Length; i++)
        {
            if (aiSlots[i].taken)
                continue;

            if ((aiSlots[i].pos - aiPos).magnitude < dist)
            {
                targetIndex = i;
                dist = (aiSlots[i].pos - aiPos).magnitude;
            }
        }

        if (float.IsInfinity(dist))
        {
            return new NpcSlot { playerIndex = -1 };
        }

        aiSlots[targetIndex].taken = true;
        aiSlots[targetIndex].nexusIndex = -1;
        aiSlots[targetIndex].playerIndex = targetIndex;
        return aiSlots[targetIndex];
    }

    public void SetSlotFree(int npcIndex)
    {
        if (npcIndex >= slots || npcIndex == -1) return;

        aiSlots[npcIndex].taken = false;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < aiSlots.Length; i++)
        {
            Gizmos.color = Color.green;
            if (aiSlots[i].taken)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(aiSlots[i].pos, 0.1f);
            }
            else
            {
                Gizmos.DrawSphere(aiSlots[i].pos, 0.1f);
            } 
        }
    }


}
