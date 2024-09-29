using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private GameField _game;
    public string Name;
    
    public Tile Setup(GameField gameField)
    {
        _game = gameField;
        return this;
    }
    
    public void OnMouseDown()
    {
        DeleteTile();
    }

    public void DeleteTile()
    {
        gameObject.SetActive(false);
        _game.DropTiles(this);
        _game.FindEmptyCell(this);
    }
}
