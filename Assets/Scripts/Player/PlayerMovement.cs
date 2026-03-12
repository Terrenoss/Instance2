using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _coolDown = 0.1f;
    [SerializeField] private Grid _grid;
    private bool _canMove = true;

    private void Start()
    {
        _grid.SnapToGrid(transform);
    }

    public void Move(Vector2 input)
    {
        if (!_canMove)
        {
            return;
        }
        Vector3 destination = new Vector3 (input.x * _grid.GetTileSize().x, 0, 0);
        transform.position += destination;
        _grid.SnapToGrid(transform);

        StartCoroutine(StartCoolDown());
    }

    IEnumerator StartCoolDown()
    {
        _canMove = false;
        yield return new WaitForSeconds(_coolDown);
        _canMove = true;
    }
}
