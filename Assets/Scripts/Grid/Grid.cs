using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    [SerializeField] private int _numRows = 10;
    [SerializeField] private int _numColumns = 3;
    [SerializeField] private Vector2 _tileSize = Vector2.one;
    private List<Tile> _tileGrid = new List<Tile>();

    private void Awake()
    {
        for (int i = 0; i < _numRows; i++)
        {
            for (int j = 0; j < _numColumns; j++)
            {
                Vector3 tilePosition = new Vector3(j * _tileSize.x, transform.position.y, i * _tileSize.y);
                Tile newTile = new Tile(i, j, tilePosition);
                _tileGrid.Add(newTile);
            }
        }
    }

    public void SnapToGrid(Transform objectToSnap)
    {
        //Finding the nearest tile
        Vector2 nearestTile = Vector2.zero;
        float minDist = Mathf.Infinity;
        for (int i=0; i < _tileGrid.Count; i++)
        {
            Vector2 tilePosition = new Vector2(_tileGrid[i].WorldPosition.x, _tileGrid[i].WorldPosition.z);
            Vector2 objectPosition = new Vector2(objectToSnap.position.x, objectToSnap.position.z);
            float dist = Vector2.Distance(tilePosition, objectPosition);

            if (dist < minDist)
            {
                nearestTile = tilePosition;
                minDist = dist;
            }
        }

        //Snapping the object to the nearest tile without going up or down
        Vector2 nearestTileCenter = new Vector2(nearestTile.x + _tileSize.x / 2, nearestTile.y + _tileSize.y / 2);
        Vector3 destination = new Vector3(nearestTileCenter.x, objectToSnap.position.y, nearestTileCenter.y);
        objectToSnap.position = destination;
    }

    public Vector2 GetTileSize()
    {
        return _tileSize;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Vector3 pos0 = new Vector3();
        Vector3 pos1 = new Vector3();
        for (int i = 0; i <= _numColumns; i++)
        {
            pos0.x = i*_tileSize.x;
            pos0.z = 0;
            pos1.x = i*_tileSize.x;
            pos1.z = _numRows*_tileSize.y;
            Gizmos.DrawLine(
                pos0,
                pos1
            );
        }

        for (int i = 0; i <= _numRows; i++)
        {
            pos0.x = 0;
            pos0.z = i*_tileSize.y;
            pos1.x = _numColumns * _tileSize.x;
            pos1.z = i*_tileSize.y;
            Gizmos.DrawLine(
                pos0,
                pos1
            );
        }
    }
}
