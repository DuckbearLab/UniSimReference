using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* ===================================================================================
* TerrainLoadUtils - 
* This scripts is calculating all the tiles that need to loaded 
* with dynamic loading - by rotation.
 * 
 * An exaple for what it should look like:
 * file: \\files\docs\Unity\Perrformance docs\DynaminLoadingByRotation.xlsx
* =================================================================================== */

public static class TerrainLoaderUtils 
{

    /// <summary>
    /// Get a list of the tiles location that need to be loaded.
    /// </summary>
    public static List<Vector2> GetTilesByDirection(NetStructs.Direction direction, Vector2 currentLocation, int layersToLoad)
    {
        switch (direction)
        {
            case NetStructs.Direction.North:
                return GetNorthTiles(currentLocation, layersToLoad);
            case NetStructs.Direction.NorthEast:
                return GetNorthEastTiles(currentLocation, layersToLoad);
            case NetStructs.Direction.East:
                return GetEastTiles(currentLocation, layersToLoad);
            case NetStructs.Direction.SouthEast:
                return GetSouthEastTiles(currentLocation, layersToLoad);
            case NetStructs.Direction.South:
                return GetSouthTiles(currentLocation, layersToLoad);
            case NetStructs.Direction.SouthWest:
                return GetSouthWestTiles(currentLocation, layersToLoad);
            case NetStructs.Direction.West:
                return GetWestTiles(currentLocation, layersToLoad);
            case NetStructs.Direction.NorthWest:
                return GetNorthWestTiles(currentLocation, layersToLoad);
        }

        return null;
    }

	private static List<Vector2> GetNorthTiles(Vector2 currentLocation, int layersToLoad)
    {
        List<Vector2> tiles = new List<Vector2>(); 

        for (int x = (int)currentLocation.x - 1; x <= currentLocation.x + 1; x++)
        {
            for (int y = (int)currentLocation.y; y <= currentLocation.y + 4; y++)
            {
                Vector2 tileLocation = new Vector2(x, y);

                if(tileLocation != currentLocation)
                    tiles.Add(tileLocation);
            }
        }

        return tiles;
    }

    private static List<Vector2> GetSouthTiles(Vector2 currentLocation, int layersToLoad)
    {
        List<Vector2> tiles = new List<Vector2>();

        for (int x = (int)currentLocation.x - 1; x <= currentLocation.x + 1; x++)
        {
            for (int y = (int)currentLocation.y; y >= currentLocation.y - 4; y--)
            {
                Vector2 tileLocation = new Vector2(x, y);

                if (tileLocation != currentLocation)
                    tiles.Add(tileLocation);
            }
        }

        return tiles;
    }

    private static List<Vector2> GetEastTiles(Vector2 currentLocation, int layersToLoad)
    {
        List<Vector2> tiles = new List<Vector2>();
        layersToLoad += 2;

        for (int x = (int)currentLocation.x; x <= currentLocation.x + layersToLoad; x++)
        {
            for (int y = (int)currentLocation.y - 1; y <= currentLocation.y + 1; y++)
            {
                Vector2 tileLocation = new Vector2(x, y);

                if (tileLocation != currentLocation)
                    tiles.Add(tileLocation);
            }
        }

        return tiles;
    }

    private static List<Vector2> GetWestTiles(Vector2 currentLocation, int layersToLoad)
    {
        List<Vector2> tiles = new List<Vector2>();
        layersToLoad += 2;

        for (int x = (int)currentLocation.x; x >= currentLocation.x - layersToLoad; x--)
        {
            for (int y = (int)currentLocation.y - 1; y <= currentLocation.y + 1; y++)
            {
                Vector2 tileLocation = new Vector2(x, y);

                if (tileLocation != currentLocation)
                    tiles.Add(tileLocation);
            }
        }

        return tiles;
    }

    private static List<Vector2> GetNorthEastTiles(Vector2 currentLocation, int layersToLoad)
    {
        List<Vector2> tiles = new List<Vector2>();
        layersToLoad += 2;

        int y = (int)currentLocation.y + 1;
        int yLimit = (int)currentLocation.y + layersToLoad;
        for (int x = (int)currentLocation.x; x <= (int)currentLocation.x + 2; x++)
        {
            for (; y <= yLimit; y++)
            {
                Vector2 tileLocation = new Vector2(x, y);
                tiles.Add(tileLocation);
            }

            y = (int)currentLocation.y;
            yLimit--;
        }

        return tiles;
    }

    private static List<Vector2> GetSouthEastTiles(Vector2 currentLocation, int layersToLoad)
    {
        List<Vector2> tiles = new List<Vector2>();
        layersToLoad += 2;

        int y = (int)currentLocation.y - 1;
        int yLimit = (int)currentLocation.y - layersToLoad;
        for (int x = (int)currentLocation.x; x <= (int)currentLocation.x + 2; x++)
        {
            for (; y >= yLimit; y--)
            {
                Vector2 tileLocation = new Vector2(x, y);
                tiles.Add(tileLocation);
            }

            y = (int)currentLocation.y;
            yLimit++;
        }

        return tiles;
    }

    private static List<Vector2> GetSouthWestTiles(Vector2 currentLocation, int layersToLoad)
    {
        List<Vector2> tiles = new List<Vector2>();
        layersToLoad += 2;

        int y = (int)currentLocation.y - 1;
        int yLimit = (int)currentLocation.y - layersToLoad;
        for (int x = (int)currentLocation.x; x >= (int)currentLocation.x - 2; x--)
        {
            for (; y >= yLimit; y--)
            {
                Vector2 tileLocation = new Vector2(x, y);
                tiles.Add(tileLocation);
            }

            y = (int)currentLocation.y;
            yLimit++;
        }

        return tiles;
    }

    private static List<Vector2> GetNorthWestTiles(Vector2 currentLocation, int layersToLoad)
    {
        List<Vector2> tiles = new List<Vector2>();
        layersToLoad += 2;

        int y = (int)currentLocation.y + 1;
        int yLimit = (int)currentLocation.y + layersToLoad;
        for (int x = (int)currentLocation.x; x >= (int)currentLocation.x - 2; x--)
        {
            for (; y <= yLimit; y++)
            {
                Vector2 tileLocation = new Vector2(x, y);
                tiles.Add(tileLocation);
            }

            y = (int)currentLocation.y;
            yLimit--;
        }

        return tiles;
    }
}
