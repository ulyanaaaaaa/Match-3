using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    [field:SerializeField] private List<Sprite> _sprites = new List<Sprite>();
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private int _gridDimension = 8;
    [SerializeField] private float _distance = 1.0f;
    [SerializeField] private GameObject GameOverMenu;
    [SerializeField] private TextMeshProUGUI MovesText;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private int _startingMoves = 50;
    private GameObject[,] _grid;
    private int _numMoves;
    private int _score;
    private int NumMoves
    {
        get
        {
            return _numMoves;
        }

        set
        {
            _numMoves = value;
            MovesText.text = _numMoves.ToString();
        }
    }
    
    private int Score
    {
        get
        {
            return _score;
        }

        set
        {
            _score = value;
            ScoreText.text = _score.ToString();
        }
    }

    private void Awake()
    {
        Instance = this;
        Score = 0;
        NumMoves = _startingMoves;
    }
    
    private void Start()
    {
        _grid = new GameObject[_gridDimension, _gridDimension];
        GameOverMenu.SetActive(false);
        InitGrid();
    }

    private void InitGrid()
    {
        Vector3 positionOffset = transform.position - new Vector3(_gridDimension * _distance / 2.0f, _gridDimension * _distance / 2.0f, 0);

        for (int row = 0; row < _gridDimension; row++)
        {
            for (int column = 0; column < _gridDimension; column++)
            {
                GameObject newTile = Instantiate(_tilePrefab);

                List<Sprite> possibleSprites = new List<Sprite>(_sprites);

                Sprite left1 = GetSpriteAt(column - 1, row);
                Sprite left2 = GetSpriteAt(column - 2, row);
                if (left2 != null && left1 == left2)
                {
                    possibleSprites.Remove(left1);
                }

                Sprite down1 = GetSpriteAt(column, row - 1);
                Sprite down2 = GetSpriteAt(column, row - 2);
                if (down2 != null && down1 == down2)
                {
                    possibleSprites.Remove(down1);
                }

                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();
                renderer.sprite = possibleSprites[Random.Range(0, possibleSprites.Count)];

                Tile tile = newTile.AddComponent<Tile>();
                tile.Position = new Vector2Int(column, row);

                newTile.transform.parent = transform;
                newTile.transform.position = new Vector3(column * _distance, row * _distance, 0) + positionOffset;

                _grid[column, row] = newTile;
            }
        }
    }

    private Sprite GetSpriteAt(int column, int row)
    {
        if (column < 0 || column >= _gridDimension
         || row < 0 || row >= _gridDimension)
            return null;
        GameObject tile = _grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer.sprite;
    }

    private SpriteRenderer GetSpriteRendererAt(int column, int row)
    {
        if (column < 0 || column >= _gridDimension
         || row < 0 || row >= _gridDimension)
            return null;
        GameObject tile = _grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer;
    }

    public void SwapTiles(Vector2Int tile1Position, Vector2Int tile2Position)
    {
        GameObject tile1 = _grid[tile1Position.x, tile1Position.y];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();
        
        GameObject tile2 = _grid[tile2Position.x, tile2Position.y];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;

        bool changesOccurs = CheckMatches();
        if(!changesOccurs)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite = temp;
        }
        else
        {
            NumMoves--;
            do
            {
                FillHoles();
            } while (CheckMatches());
            if (NumMoves <= 0)
            {
                NumMoves = 0;
                GameOver();
            }
        }
    }

    private bool CheckMatches()
    {
        HashSet<SpriteRenderer> matchedTiles = new HashSet<SpriteRenderer>();
        for (int row = 0; row < _gridDimension; row++)
        {
            for (int column = 0; column < _gridDimension; column++)
            {
                SpriteRenderer current = GetSpriteRendererAt(column, row);

                List<SpriteRenderer> horizontalMatches = FindColumnMatchForTile(column, row, current.sprite);
                if (horizontalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(horizontalMatches);
                    matchedTiles.Add(current);
                }

                List<SpriteRenderer> verticalMatches = FindRowMatchForTile(column, row, current.sprite);
                if (verticalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(verticalMatches);
                    matchedTiles.Add(current);
                }
            }
        }

        foreach (SpriteRenderer renderer in matchedTiles)
        {
            renderer.sprite = null;
        }
        Score += matchedTiles.Count;
        return matchedTiles.Count > 0;
    }

    private List<SpriteRenderer> FindColumnMatchForTile(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = col + 1; i < _gridDimension; i++)
        {
            SpriteRenderer nextColumn = GetSpriteRendererAt(i, row);
            if (nextColumn.sprite != sprite)
            {
                break;
            }
            result.Add(nextColumn);
        }
        return result;
    }

    private List<SpriteRenderer> FindRowMatchForTile(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = row + 1; i < _gridDimension; i++)
        {
            SpriteRenderer nextRow = GetSpriteRendererAt(col, i);
            if (nextRow.sprite != sprite)
            {
                break;
            }
            result.Add(nextRow);
        }
        return result;
    }

    private void FillHoles()
    {
        for (int column = 0; column < _gridDimension; column++)
        {
            for (int row = 0; row < _gridDimension; row++)
            {
                while (GetSpriteRendererAt(column, row).sprite == null)
                {
                    SpriteRenderer current = GetSpriteRendererAt(column, row);
                    SpriteRenderer next = current;
                    for (int filler = row; filler < _gridDimension - 1; filler++)
                    {
                        next = GetSpriteRendererAt(column, filler + 1);
                        current.sprite = next.sprite;
                        current = next;
                    }

                    next.sprite = _sprites[Random.Range(0, _sprites.Count)];
                }
            }
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
        PlayerPrefs.SetInt("score", Score);
        GameOverMenu.SetActive(true);
    }
}
