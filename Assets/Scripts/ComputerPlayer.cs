using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int position;
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;
    public Node parent;

    public Node(Vector2Int pos)
    {
        position = pos;
    }
}

public class ComputerPlayer : MonoBehaviour
{
    private BoardManager boardManager;
    private HashSet<Vector2Int> previousMoves = new HashSet<Vector2Int>();
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameSoundManager _gameSoundManager;

    private void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
    }

    private PieceController selectedWolf;
    private Vector2Int bestWolfMove;

    public void MakeComputerMove()
    {
        if (boardManager.computerPlaysForSheep)
        {
            PieceController sheep = boardManager.GetSheep();
            if (sheep == null) return;

            boardManager.HighlightPossibleMoves(sheep);
            Invoke(nameof(PerformSheepMove), 0.5f);
        }
        else
        {
            selectedWolf = null;
            bestWolfMove = Vector2Int.zero;
            float bestScore = float.MinValue;

            foreach (PieceController wolf in boardManager.GetWolves())
            {
                List<Vector2Int> possibleMoves = boardManager.GetHighlightedTiles(wolf);

                foreach (Vector2Int move in possibleMoves)
                {
                    float score = EvaluateMoveForWolf(move, boardManager.GetSheep());
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestWolfMove = move;
                        selectedWolf = wolf;
                    }
                }
            }

            if (selectedWolf != null && bestWolfMove != selectedWolf.currentPosition)
            {
                boardManager.HighlightPossibleMoves(selectedWolf);

                Invoke(nameof(PerformWolfMove), 0.5f);
            }
        }
    }


    private void PerformSheepMove()
    {
        PieceController sheep = boardManager.GetSheep();
        Vector2Int sheepPosition = sheep.currentPosition;

        Vector2Int bestMove = CalculateSafeMoveForSheep(sheep);

        if (bestMove != sheepPosition)
        {
            boardManager.MovePiece(sheep, bestMove);

            if (boardManager.HasSheepReachedTop(sheep))
            {
                StartCoroutine(ShowLose());
            }
        }
        else
        {
            StartCoroutine(ShowWin());     
        }

        boardManager.ResetHighlights();
    }

    private IEnumerator ShowWin()
    {
        yield return new WaitForSeconds(0.5f);
        int level = PlayerPrefs.GetInt("CurrentLevel", 1);
        level++;
        PlayerPrefs.SetInt("CurrentLevel", level);
        _winPanel.SetActive(true);
        _gameSoundManager.PlayWinSound();
    }

    private IEnumerator ShowLose()
    {
        yield return new WaitForSeconds(0.5f);
        _losePanel.SetActive(true);
        _gameSoundManager.PlayLoseSound();
    }

    private float EvaluateMoveForSheep(Vector2Int move, PieceController sheep)
    {
        float score = 0f;

        if (move.x < sheep.currentPosition.x && !IsTopPathBlocked(move))
        {
            score += 1000f;
        }
        else if (IsTopPathBlocked(move))
        {
            score -= 200f;
            score += EvaluateProximityToFreedom(move, sheep);
        }

        float minDistanceToWolf = GetDistanceToClosestWolf(move);
        if (minDistanceToWolf <= 2f && !WillSheepBeTrapped(move))
        {
            score += 300f;
        }

        if (WillSheepBeTrapped(move))
        {
            score -= 500f;
        }
        else
        {
            score += 200f;
        }

        return score;
    }

    private bool WillSheepBeTrapped(Vector2Int move)
    {
        List<Vector2Int> futureMoves = boardManager.GetHighlightedTiles(boardManager.GetSheep());

        for (int i = 0; i < futureMoves.Count; i++)
        {
            Vector2Int futureMove = futureMoves[i];
            if (!IsMoveLeadingToTrap(futureMove))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsMoveLeadingToTrap(Vector2Int futureMove)
    {
        foreach (var wolf in boardManager.GetWolves())
        {
            if (Vector2Int.Distance(wolf.currentPosition, futureMove) <= 2f)
            {
                return true;
            }
        }

        return false;
    }


    private float EvaluateProximityToFreedom(Vector2Int move, PieceController sheep)
    {
        float score = 0f;

        List<Vector2Int> possibleDirections = GetPossibleDirections();
        bool foundFreedom = false;

        foreach (var direction in possibleDirections)
        {
            Vector2Int checkPosition = move + direction;

            if (IsWithinBoundsAndFree(checkPosition))
            {
                foundFreedom = true;
                score += 300f;
            }
        }

        if (!foundFreedom)
        {
            score -= 300f;
        }

        return score;
    }



    private bool IsTopPathBlocked(Vector2Int move)
    {
        for (int x = move.x - 1; x >= 0; x--)
        {
            if (boardManager.IsPositionOccupied(new Vector2Int(x, move.y)))
            {
                return true;
            }
        }
        return false;
    }


    private bool IsWolfNearby(Vector2Int move)
    {
        foreach (var wolf in boardManager.GetWolves())
        {
            if (Vector2Int.Distance(move, wolf.currentPosition) <= 1.5f)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsWithinBoundsAndFree(Vector2Int position)
    {
        return boardManager.IsWithinBounds(position) && !boardManager.IsPositionOccupied(position);
    }

    private List<Vector2Int> GetPossibleDirections()
    {
        return new List<Vector2Int>
    {
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 1)
    };
    }


    private bool IsPathBlockedByWolves(Vector2Int move)
    {
        foreach (var wolf in boardManager.GetWolves())
        {
            if (Mathf.Abs(wolf.currentPosition.x - move.x) <= 1 && Mathf.Abs(wolf.currentPosition.y - move.y) <= 1)
            {
                return true;
            }
        }
        return false;
    }

    private bool CanMoveLeftOrDown(PieceController sheep)
    {
        Vector2Int currentPos = sheep.currentPosition;
        Vector2Int leftMove = currentPos + new Vector2Int(-1, 0);
        Vector2Int downMove = currentPos + new Vector2Int(0, -1);

        return boardManager.IsMoveValid(currentPos, leftMove, true) || boardManager.IsMoveValid(currentPos, downMove, true);

    }

    private Vector2Int CalculateSafeMoveForSheep(PieceController sheep)
    {
        List<Vector2Int> possibleMoves = boardManager.GetHighlightedTiles(sheep);
        Vector2Int bestMove = sheep.currentPosition;
        float bestScore = float.MinValue;

        foreach (var move in possibleMoves)
        {
            if (boardManager.IsDiagonalMove(sheep.currentPosition, move))
            {
                float moveScore = 0f;

                if (!boardManager.IsWithinBounds(move))
                {
                    moveScore -= 200f;
                }

                if (move.x < sheep.currentPosition.x && !IsTopPathBlocked(move))
                {
                    moveScore += 300f;
                }

                if (WillSheepBeTrapped(move))
                {
                    moveScore -= 100f;
                }

                float minDistanceToWolf = GetDistanceToClosestWolf(move);
                if (minDistanceToWolf <= 2f)
                {
                    moveScore += 50f;
                }

                if (IsMoveTowardsCorner(move))
                {
                    moveScore -= 150f;
                }

                if (moveScore > bestScore)
                {
                    bestScore = moveScore;
                    bestMove = move;
                }
            }
        }

        return bestMove;
    }

    private bool IsMoveTowardsCorner(Vector2Int move)
    {
        return (move.x == 0 || move.x == boardManager.boardSize - 1) &&
               (move.y == 0 || move.y == boardManager.boardSize - 1);
    }

    private float GetDistanceToClosestWolf(Vector2Int sheepPosition)
    {
        float minDistance = float.MaxValue;

        foreach (var wolf in boardManager.GetWolves())
        {
            float distance = Vector2Int.Distance(sheepPosition, wolf.currentPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }

        return minDistance;
    }

    private void PerformWolfMove()
    {
        if (selectedWolf != null && bestWolfMove != selectedWolf.currentPosition)
        {
            boardManager.MovePiece(selectedWolf, bestWolfMove);

            if (IsSheepBlocked(boardManager.GetSheep()))
            {
                _losePanel.SetActive(true);
            }
        }

        boardManager.ResetHighlights();
    }

    private bool IsSheepBlocked(PieceController sheep)
    {
        List<Vector2Int> sheepMoves = boardManager.GetHighlightedTiles(sheep);

        return sheepMoves.Count == 0;
    }


    private Vector2Int CalculateBestMoveForWolf(PieceController wolf, PieceController sheep)
    {
        List<Vector2Int> possibleMoves = boardManager.GetHighlightedTiles(wolf);

        if (possibleMoves.Count == 0)
        {
            return wolf.currentPosition;
        }

        Vector2Int bestMove = possibleMoves[0];
        float bestScore = float.MinValue;

        foreach (var move in possibleMoves)
        {
            float score = EvaluateMoveForWolf(move, sheep);

            if (score > bestScore || bestScore == float.MinValue)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        if (bestMove == wolf.currentPosition && possibleMoves.Count > 0)
        {
            bestMove = possibleMoves[0];
        }

        return bestMove;
    }

    private float EvaluateMoveForWolf(Vector2Int wolfMove, PieceController sheep)
    {
        float distanceToSheep = Vector2Int.Distance(wolfMove, sheep.currentPosition);
        float score = 10f - distanceToSheep;

        List<Vector2Int> sheepMoves = boardManager.GetHighlightedTiles(sheep);
        foreach (var sheepMove in sheepMoves)
        {
            if (Vector2Int.Distance(wolfMove, sheepMove) < 1.5f)
            {
                score += 5f;
            }
        }

        return score;
    }
}