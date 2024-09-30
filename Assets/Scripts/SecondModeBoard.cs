using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecondModeBoard : MonoBehaviour
{
    private PieceController2Player selectedPiece = null;
    public Transform boardParent;
    private GameObject[,] board = new GameObject[8, 8];

    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _menu;
    private OptionsController _optionsController;
    [SerializeField] private GameSoundManager _gameSoundManager;

    private bool isSheepTurn = true;

    private void Start()
    {
        _optionsController = GetComponent<OptionsController>();
        PlayerPrefs.SetString("ShouldShowLoadAndWelcome", "yes");
    }

    public void SelectPiece(PieceController2Player piece)
    {
        if (isSheepTurn && !piece.isSheep)
        {
            return;
        }
        if (!isSheepTurn && piece.isSheep)
        {
            return;
        }

        selectedPiece = piece;
        ResetHighlights();
        HighlightPossibleMoves(piece);
    }

    public void OnTileClicked(Vector2Int tilePosition)
    {
        if (selectedPiece != null)
        {
            if (IsMoveValid(tilePosition))
            {
                MovePiece(selectedPiece, tilePosition);

                if (selectedPiece.isSheep && HasSheepReachedTop(selectedPiece))
                {
                    ShowWinPanel();
                    ResetHighlights();
                    return;
                }

                ResetHighlights();

                isSheepTurn = !isSheepTurn;

                if (!isSheepTurn && IsSheepBlocked())
                {
                    ShowLosePanel();
                    return;
                }
            }
            selectedPiece = null;
        }
    }

    private bool IsSheepBlocked()
    {
        PieceController2Player sheep = FindObjectOfType<PieceController2Player>();
        if (sheep == null || !sheep.isSheep)
        {
            return false;
        }

        Vector2Int currentPos = sheep.currentPosition;

        bool topRightBlocked = IsTileBlocked(currentPos + new Vector2Int(1, 1));
        bool topLeftBlocked = IsTileBlocked(currentPos + new Vector2Int(-1, 1));
        bool bottomRightBlocked = IsTileBlocked(currentPos + new Vector2Int(1, -1));
        bool bottomLeftBlocked = IsTileBlocked(currentPos + new Vector2Int(-1, -1));

        return topRightBlocked && topLeftBlocked && bottomRightBlocked && bottomLeftBlocked;
    }

    private bool IsTileBlocked(Vector2Int position)
    {
        if (!IsWithinBounds(position))
        {
            return true;
        }

        Transform tile = boardParent.GetChild(position.x * 8 + position.y);
        bool isBlocked = tile.childCount > 0;
        return isBlocked;
    }

    private bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
    }

    private bool HasSheepReachedTop(PieceController2Player piece)
    {
        return piece.currentPosition.x == 0;
    }

    private bool IsMoveValid(Vector2Int position)
    {
        if (IsWithinBounds(position))
        {
            Transform tile = boardParent.GetChild(position.x * 8 + position.y);

            foreach (Transform child in tile)
            {
                if (child.GetComponent<PieceController2Player>() != null)
                {
                    return false;
                }
            }

            Color tileColor = tile.GetComponent<UnityEngine.UI.Image>().color;
            return tileColor == new Color(1, 0, 0.75f, 1);
        }
        return false;
    }

    public void MovePiece(PieceController2Player piece, Vector2Int newPosition)
    {
        if (!IsWithinBounds(newPosition))
        {
            return;
        }

        Transform newTile = boardParent.GetChild(newPosition.x * 8 + newPosition.y);
        StartCoroutine(SmoothMove(piece, newTile));
        piece.currentPosition = newPosition;
    }

    private IEnumerator SmoothMove(PieceController2Player piece, Transform newTile)
    {
        Vector3 startPosition = piece.transform.position;
        Vector3 endPosition = newTile.position;

        float moveDuration = 0.25f;
        float elapsedTime = 0f;

        _gameSoundManager.PlayMoveSound();
        _optionsController.TryLightHaptic();

        while (elapsedTime < moveDuration)
        {
            piece.transform.GetChild(0).gameObject.SetActive(true);
            piece.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        piece.transform.position = endPosition;
        piece.transform.SetParent(newTile);
        piece.transform.localPosition = Vector3.zero;
        piece.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void HighlightPossibleMoves(PieceController2Player piece)
    {
        Vector2Int currentPos = piece.currentPosition;

        if (piece.isSheep)
        {
            HighlightTileIfAvailable(currentPos + new Vector2Int(1, 1)); 
            HighlightTileIfAvailable(currentPos + new Vector2Int(-1, 1));
            HighlightTileIfAvailable(currentPos + new Vector2Int(1, -1));
            HighlightTileIfAvailable(currentPos + new Vector2Int(-1, -1));
        }
        else
        {
            HighlightTileIfAvailable(currentPos + new Vector2Int(1, 1)); 
            HighlightTileIfAvailable(currentPos + new Vector2Int(1, -1));
        }
    }

    private void HighlightTileIfAvailable(Vector2Int position)
    {
        if (IsWithinBounds(position))
        {
            if (boardParent.GetChild(position.x * 8 + position.y).childCount == 0)
            {
                HighlightTile(position);
            }
        }
    }

    public void ResetHighlights()
    {
        foreach (Transform tile in boardParent)
        {
            tile.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0);
        }
    }

    private void HighlightTile(Vector2Int position)
    {
        if (IsWithinBounds(position))
        {
            _gameSoundManager.TapOnPieceSound();
            _optionsController.TryLightHaptic();
            Transform tile = boardParent.GetChild(position.x * 8 + position.y);
            tile.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 0, 0.75f, 1);
        }
    }

    public void ReloadGame2()
    {
        StartCoroutine(WaitTOReload());
    }

    private IEnumerator WaitTOReload()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        StartCoroutine(WaitToBack());
    }

    private IEnumerator WaitToBack()
    {
        PlayerPrefs.SetString("ShouldShowLoadAndWelcome", "no");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MenuScreen");
    }

    public void OpenOptions()
    {
        _menu.SetActive(false);
        _options.SetActive(true);
        _optionsController.HideShowBtn();
    }

    public void CloseOptions()
    {
        _options.SetActive(false);
        _menu.SetActive(true);
    }

    public void ShowWinPanel()
    {
        StartCoroutine(WaitForWin());
    }

    private IEnumerator WaitForWin()
    {
        yield return new WaitForSeconds(0.5f);
        _gameSoundManager.PlayWinSound();
        _winPanel.SetActive(true);
    }

    public void ShowLosePanel()
    {
        StartCoroutine(WaitForLose());
    }

    private IEnumerator WaitForLose()
    {
        yield return new WaitForSeconds(0.5f);
        _gameSoundManager.PlayLoseSound();
        _losePanel.SetActive(true);
    }
}