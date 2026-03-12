using UnityEngine;

public class Tile
{
    public Vector3 WorldPosition;
    private int _gridPosX;
    private int _gridPosY;

    public Tile(int gridPosX, int gridPosY, Vector3 worldPosition)
    {
        _gridPosX = gridPosX;
        _gridPosY = gridPosY;
        WorldPosition = worldPosition;
    }

    public int GetPosX()
    {
        return _gridPosX;
    }

    public int GetPosY() 
    { 
        return _gridPosY; 
    }
}
