using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool isSheepTurn = true; // Овца ходит первой

    public void EndTurn()
    {
        isSheepTurn = !isSheepTurn;
    }

    public bool IsSheepTurn()
    {
        return isSheepTurn;
    }
}
