using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    [SerializeField] private  int _xSize;
    [SerializeField] private int _ySize;
    [SerializeField] private Tile _cell;

    private Tile[,] _cells;
    [SerializeField] private List<Tile> _tiles;


    private void Start()
    {
        CreateField();
    }

    private void CreateField()
    {
        _cells = new Tile[_xSize, _ySize]; 
        
        for (int x = 0; x < _xSize; x++)
        {
            for (int y = 0; y < _ySize; y++)
            {
                _cells[x, y] = Instantiate(_cell, new Vector2(x, y), Quaternion.identity);
                if (y <= 2) 
                {
                    SetTile(x, y, _tiles[Random.Range(0, _tiles.Count)]);
                }
            }
        }
    }
    
    private void SetTile(int x, int y, Tile tile)
    {
        Instantiate(tile, new Vector2(x, y),Quaternion.identity, _cells[x, y].transform).Setup(this);
    }

    public void FindEmptyCell(Tile tile)
    {
        List<Vector2> emptyCells = new List<Vector2>();
        int highestColumn = -1;
        
        for (int x = 0; x < _xSize; x++)
        {
            for (int y = 0; y < _ySize; y++)
            {
                if (_cells[x, y].transform.childCount == 0)
                {
                    if (x > highestColumn)
                    {
                        highestColumn = x;
                        emptyCells.Clear();
                    }

                    if (x == highestColumn)
                    {
                        emptyCells.Add(new Vector2(x, y));
                    }
                }
            }
        }
        
        if (emptyCells.Count > 0)
        {
            Vector2 randomCell = emptyCells[Random.Range(0, emptyCells.Count)];
            tile.transform.parent = _cells[(int)randomCell.x, (int)randomCell.y].transform;
            tile.transform.position = randomCell;
            tile.gameObject.SetActive(true);
        }
    }

    public void DropTiles(Tile tile)
    {
        int x = (int)tile.transform.position.x;
        int y = (int)tile.transform.position.y;
        
        for (int i = y + 1; i < _ySize; i++)
        {
            if (_cells[x, i].transform.childCount != 0)
            {
                Transform tileAbove = _cells[x, i].transform.GetChild(0);
                tileAbove.parent = _cells[x, i - 1].transform;
                tileAbove.position = new Vector2(x, i - 1);
            }
        }
        
        tile.transform.parent = _cells[x, _ySize - 1].transform;
        tile.transform.position = new Vector2(x, _ySize - 1);
        tile.gameObject.SetActive(true);
    }
    
    public void CheckForMatches()
    {
        List<Tile> tilesToDestroy = new List<Tile>();
        
        for (int y = 0; y < _ySize; y++)
        {
            for (int x = 0; x < _xSize - 2; x++)
            {
                if (_cells[x, y].transform.childCount > 0 &&
                    _cells[x + 1, y].transform.childCount > 0 &&
                    _cells[x + 2, y].transform.childCount > 0)
                {
                    Tile tile1 = _cells[x, y].transform.GetChild(0).GetComponent<Tile>();
                    Tile tile2 = _cells[x + 1, y].transform.GetChild(0).GetComponent<Tile>();
                    Tile tile3 = _cells[x + 2, y].transform.GetChild(0).GetComponent<Tile>();

                    if (tile1.Name == tile2.Name && tile2.Name == tile3.Name)
                    {
                        tilesToDestroy.Add(tile1);
                        tilesToDestroy.Add(tile2);
                        tilesToDestroy.Add(tile3);
                    }
                }
            }
        }
        
        for (int x = 0; x < _xSize; x++)
        {
            for (int y = 0; y < _ySize - 2; y++)
            {
                if (_cells[x, y].transform.childCount > 0 &&
                    _cells[x, y + 1].transform.childCount > 0 &&
                    _cells[x, y + 2].transform.childCount > 0)
                {
                    Tile tile1 = _cells[x, y].transform.GetChild(0).GetComponent<Tile>();
                    Tile tile2 = _cells[x, y + 1].transform.GetChild(0).GetComponent<Tile>();
                    Tile tile3 = _cells[x, y + 2].transform.GetChild(0).GetComponent<Tile>();

                    if (tile1.Name == tile2.Name && tile2.Name == tile3.Name)
                    {
                        tilesToDestroy.Add(tile1);
                        tilesToDestroy.Add(tile2);
                        tilesToDestroy.Add(tile3);
                    }
                }
            }
        }
        
        foreach (Tile tile in tilesToDestroy)
        {
            tile.DeleteTile();
        }
    }
}
