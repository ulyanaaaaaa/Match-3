using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int Position;
    private static Tile _selected;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Select()
    {
        _spriteRenderer.color = Color.grey;
    }

    private void Unselect()
    {
        _spriteRenderer.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (_selected != null)
        {
            if (_selected == this)
                return;
            _selected.Unselect();
            
            if (Vector2Int.Distance(_selected.Position, Position) == 1)
            {
                GridManager.Instance.SwapTiles(Position, _selected.Position);
                _selected = null;
            }
            else
            {
                _selected = this;
                Select();
            }
        }
        else
        {
            _selected = this;
            Select();
        }
    }
}
