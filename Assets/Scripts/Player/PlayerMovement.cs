using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Grid _grid;

    private void Start()
    {
        if (_grid != null)
        {
            _grid.SnapToGrid(transform);
        }
    }

    public void Move(Vector2 input)
    {
        if (_grid == null) return;
        Vector3 destination = new Vector3 (input.x * _grid.GetTileSize().x, 0, 0);
        transform.position += destination;
        
        _grid.SnapToGrid(transform);
    }
}
