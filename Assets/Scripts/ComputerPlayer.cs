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
    private HashSet<Vector2Int> previousMoves = new HashSet<Vector2Int>(); // Запоминаем предыдущие ходы

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
            selectedWolf = null;  // Волк, который сделает лучший ход
            bestWolfMove = Vector2Int.zero;
            float bestScore = float.MinValue;

            // Оцениваем ходы всех волков и выбираем лучшего
            foreach (PieceController wolf in boardManager.GetWolves())
            {
                List<Vector2Int> possibleMoves = boardManager.GetHighlightedTiles(wolf); // Получаем доступные ходы для текущего волка

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
                // Подсвечиваем возможные ходы выбранного волка
                boardManager.HighlightPossibleMoves(selectedWolf);

                // Используем Invoke для вызова метода через строку
                Invoke(nameof(PerformWolfMove), 0.5f);
            }
            else
            {
                Debug.LogWarning("Нет доступных ходов для волка.");
            }
        }
    }


    private void PerformSheepMove()
    {
        PieceController sheep = boardManager.GetSheep();
        Vector2Int sheepPosition = sheep.currentPosition;

        // Используем новый метод для расчета безопасного хода
        Vector2Int bestMove = CalculateSafeMoveForSheep(sheep);

        if (bestMove != sheepPosition)
        {
            boardManager.MovePiece(sheep, bestMove);
            Debug.Log($"Овца переместилась на {bestMove}");

            if (boardManager.HasSheepReachedTop(sheep))
            {
                Debug.Log("Овца достигла верхней части доски. Победа овцы.");
            }
        }
        else
        {
            Debug.LogWarning("Овца не нашла безопасного хода.");
        }

        boardManager.ResetHighlights();
    }


    private float EvaluateMoveForSheep(Vector2Int move, PieceController sheep)
    {
        float score = 0f;

        // Приоритет движения наверх, если путь не заблокирован
        if (move.x < sheep.currentPosition.x && !IsTopPathBlocked(move))
        {
            score += 1000f;  // Большой бонус за движение наверх
        }
        else if (IsTopPathBlocked(move))
        {
            score -= 200f;  // Штраф за заблокированный путь наверх
            score += EvaluateProximityToFreedom(move, sheep);
        }

        // Проверка расстояния до ближайшего волка
        float minDistanceToWolf = GetDistanceToClosestWolf(move);
        if (minDistanceToWolf <= 2f && !WillSheepBeTrapped(move))
        {
            score += 300f;  // Бонус за агрессивное движение к волкам, если это безопасно
        }

        // Проверка на возможность выхода из тупика через несколько ходов
        if (WillSheepBeTrapped(move))
        {
            score -= 500f;  // Штраф за тупик
        }
        else
        {
            score += 200f;  // Бонус за безопасный ход
        }

        return score;
    }

    // Оценка безопасности хода овцы с учётом нескольких шагов вперёд
    private bool WillSheepBeTrapped(Vector2Int move)
    {
        // Получаем все возможные будущие ходы овцы после текущего перемещения
        List<Vector2Int> futureMoves = boardManager.GetHighlightedTiles(boardManager.GetSheep());

        // Проверяем несколько ходов вперёд, чтобы понять, не загонит ли овца себя в угол
        for (int i = 0; i < futureMoves.Count; i++)
        {
            Vector2Int futureMove = futureMoves[i];
            if (!IsMoveLeadingToTrap(futureMove))
            {
                return false;  // Если есть хотя бы один безопасный ход, овца не будет загнана в угол
            }
        }

        return true;  // Если все возможные ходы ведут в тупик, овца будет окружена
    }

    // Проверяем, приведет ли ход к окружению со стороны волков
    private bool IsMoveLeadingToTrap(Vector2Int futureMove)
    {
        foreach (var wolf in boardManager.GetWolves())
        {
            // Если волки могут окружить овцу через 1-2 хода
            if (Vector2Int.Distance(wolf.currentPosition, futureMove) <= 2f)
            {
                return true;  // Ход приведет к блокировке
            }
        }

        return false;  // Ход безопасен
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
                score += 300f;  // Бонус за нахождение свободы
            }
        }

        // Если все направления заблокированы, минимизируем потери
        if (!foundFreedom)
        {
            score -= 300f;  // Штраф за тупик
        }

        return score;
    }



    // Метод для проверки заблокирован ли путь наверх
    private bool IsTopPathBlocked(Vector2Int move)
    {
        // Проверяем несколько клеток наверху на наличие волков
        for (int x = move.x - 1; x >= 0; x--)
        {
            if (boardManager.IsPositionOccupied(new Vector2Int(x, move.y)))
            {
                return true;  // Если на пути есть волк, путь заблокирован
            }
        }
        return false;
    }


    // Метод для оценки, есть ли рядом волк
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


    // Проверка, свободна ли клетка в определённой позиции
    private bool IsWithinBoundsAndFree(Vector2Int position)
    {
        return boardManager.IsWithinBounds(position) && !boardManager.IsPositionOccupied(position);
    }

    // Получение возможных направлений движения
    private List<Vector2Int> GetPossibleDirections()
    {
        return new List<Vector2Int>
    {
        new Vector2Int(-1, -1), // Вверх влево
        new Vector2Int(-1, 1),  // Вверх вправо
        new Vector2Int(1, -1),  // Вниз влево
        new Vector2Int(1, 1)    // Вниз вправо
    };
    }



    private bool IsPathBlockedByWolves(Vector2Int move)
    {
        foreach (var wolf in boardManager.GetWolves())
        {
            // Если волки блокируют движение по прямой вперед
            if (Mathf.Abs(wolf.currentPosition.x - move.x) <= 1 && Mathf.Abs(wolf.currentPosition.y - move.y) <= 1)
            {
                return true; // Путь заблокирован волками
            }
        }
        return false;
    }

    private bool CanMoveLeftOrDown(PieceController sheep)
    {
        // Проверяем, есть ли возможность для движения влево или вниз
        Vector2Int currentPos = sheep.currentPosition;
        Vector2Int leftMove = currentPos + new Vector2Int(-1, 0); // Влево
        Vector2Int downMove = currentPos + new Vector2Int(0, -1); // Вниз

        // Возвращаем true, если хотя бы одно из этих направлений доступно
        return boardManager.IsMoveValid(currentPos, leftMove, true) || boardManager.IsMoveValid(currentPos, downMove, true);

    }

    private Vector2Int CalculateSafeMoveForSheep(PieceController sheep)
    {
        List<Vector2Int> possibleMoves = boardManager.GetHighlightedTiles(sheep);
        Vector2Int bestMove = sheep.currentPosition;
        float bestScore = float.MinValue;

        foreach (var move in possibleMoves)
        {
            if (boardManager.IsDiagonalMove(sheep.currentPosition, move)) // Проверяем только диагональные ходы
            {
                float moveScore = 0f;

                // Проверяем, находится ли овца на границе игрового поля
                if (!boardManager.IsWithinBounds(move))
                {
                    moveScore -= 200f;  // Штраф за попытку уйти за границы поля
                }

                // Овца должна двигаться наверх, если путь свободен
                if (move.x < sheep.currentPosition.x && !IsTopPathBlocked(move))
                {
                    moveScore += 300f;  // Преимущество за движение наверх
                }

                // Если на пути тупик или граница, штрафуем
                if (WillSheepBeTrapped(move))
                {
                    moveScore -= 100f;  // Штраф за потенциальный тупик
                }

                // Овца должна быть смелой и обходить волков
                float minDistanceToWolf = GetDistanceToClosestWolf(move);
                if (minDistanceToWolf <= 2f)
                {
                    moveScore += 50f;  // Награда за движение ближе к волкам, если это даёт выход
                }

                // Проверка на углы. Если овца движется в угол, штрафуем
                if (IsMoveTowardsCorner(move))
                {
                    moveScore -= 150f;  // Штраф за движение к углам
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
        // Проверяем, находится ли клетка в углу
        return (move.x == 0 || move.x == boardManager.boardSize - 1) &&
               (move.y == 0 || move.y == boardManager.boardSize - 1);
    }




    private float GetDistanceToClosestWolf(Vector2Int sheepPosition)
    {
        float minDistance = float.MaxValue;

        // Получаем всех волков с доски
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
            Debug.Log($"Волк переместился на {bestWolfMove}");
        }

        boardManager.ResetHighlights();
    }






    private Vector2Int CalculateBestMoveForWolf(PieceController wolf, PieceController sheep)
    {
        List<Vector2Int> possibleMoves = boardManager.GetHighlightedTiles(wolf);

        // Если нет доступных ходов, возвращаем текущую позицию
        if (possibleMoves.Count == 0)
        {
            Debug.LogWarning("Нет доступных ходов для волка.");
            return wolf.currentPosition;
        }

        Vector2Int bestMove = possibleMoves[0];  // По умолчанию первый ход
        float bestScore = float.MinValue;

        // Проверяем все возможные ходы
        foreach (var move in possibleMoves)
        {
            float score = EvaluateMoveForWolf(move, sheep);

            // Проверяем наилучший ход, даже если оценка отрицательная
            if (score > bestScore || bestScore == float.MinValue)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        // Если ни один ход не лучше текущего, выбираем первый доступный
        if (bestMove == wolf.currentPosition && possibleMoves.Count > 0)
        {
            bestMove = possibleMoves[0];  // Выбираем первый доступный ход
            Debug.Log("Не нашлось лучшего хода, выбран первый доступный ход.");
        }

        Debug.Log($"Лучший ход для волка: {bestMove} с оценкой: {bestScore}");
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