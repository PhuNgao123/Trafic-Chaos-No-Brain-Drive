using UnityEngine;

public class LaneManager : MonoBehaviour
{
    // 6 lane: 0–2 ngược chiều, 3–5 cùng chiều
    public float[] laneX = { -25f, -15f, -5f, 5f, 15f, 25f };

    public float GetLaneX(int laneIndex)
    {
        return laneX[laneIndex];
    }
}
