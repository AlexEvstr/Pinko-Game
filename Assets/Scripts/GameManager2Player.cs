using UnityEngine;

public class GameManager2Player : MonoBehaviour
{
    private bool isSheepTurn = true;

    public void EndTurn()
    {
        isSheepTurn = !isSheepTurn;
    }

    public bool IsSheepTurn()
    {
        return isSheepTurn;
    }
}
