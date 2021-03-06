using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGrid
{
    public bool[,] Tiles { get; private set; }
    public int Offset_X { get; private set; }
    public int Offset_Y { get; private set; }
    public LayerMask Buildings_Layer { get; private set; }
    public LayerMask Units_Layer { get; private set; }
    public float Field_Offset_X { get; private set; }
    public float Field_Offset_Y { get; private set; }

    public MapGrid(Tilemap BorderTilemap, Tilemap BuildingsTilemap, LayerMask BuildingsLayer, LayerMask UnitsLayer, float FieldOffsetX, float FieldOffsetY)
    {
        Vector3 min_tile = BorderTilemap.CellToWorld(BorderTilemap.cellBounds.min);
        Vector3 size = BorderTilemap.size;
        Offset_X = (int)(min_tile.x * -1);
        Offset_Y = (int)(min_tile.y * -1);
        Tiles = new bool[(int)size.x, (int)size.y];

        for (int x = 1; x < Tiles.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < Tiles.GetLength(1) - 1; y++)
            {
                TileBase tile = BuildingsTilemap.GetTile(new Vector3Int(x - Offset_X, y - Offset_Y, 0));
                Tiles[x, y] = (tile == null);
            }
        }

        Buildings_Layer = BuildingsLayer;
        Units_Layer = UnitsLayer;
        Field_Offset_X = FieldOffsetX;
        Field_Offset_Y = FieldOffsetY;
    }
}
