using System.Collections.Generic;
using UnityEngine;

public class Covers: ICover
{
    float _SafeDistance;
    float _MaxDistance;
    LayerMask _BuildingsLayer;
    List<CoverPoint> _Covers;

    public Covers(MapGrid Map, LayerMask BuildingsLayer, float FieldOffsetX, float FieldOffsetY, float SafeDistance, float MaxDistance)
    {
        if (Map != null && Map.Tiles != default(bool[,]))
        {
            _Covers = new List<CoverPoint>();
            int offsetX = Map.Offset_X;
            int offsetY = Map.Offset_Y;
            float fieldOffsetX = Map.Field_Offset_X;
            float fieldOffsetY = Map.Field_Offset_Y;
            CoverPoint[] previousCovers = new CoverPoint[Map.Tiles.GetLength(1) - 1];
            int[] distances = new int[Map.Tiles.GetLength(1) - 1];
            CoverPoint previousCover = null;
            int distance = 0;
            for (int x = 1; x < Map.Tiles.GetLength(0) - 1; x++)
            {
                if (previousCover != null)
                {
                    previousCover.SetMaxOffsetCount(distance);
                    previousCover = null;
                }
                distance = 0;
                for (int y = 1; y < Map.Tiles.GetLength(1) - 1; y++)
                {
                    if (Map.Tiles[x, y])
                    {
                        if (!Map.Tiles[x, y + 1])
                        {
                            if (Map.Tiles[x - 1, y] && Map.Tiles[x - 1, y + 1])
                            {
                                CoverPoint point = new CoverPoint(new Vector3(x - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY),
                                    new Vector3(x - 1 - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY));
                                _Covers.Add(point);
                                previousCovers[y] = point;
                                distances[y] = 0;
                            }
                            else if (Map.Tiles[x + 1, y] && Map.Tiles[x + 1, y + 1])
                            {
                                CoverPoint point = new CoverPoint(new Vector3(x - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY),
                                    new Vector3(x + 1 - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY));
                                _Covers.Add(point);

                                if (previousCovers[y] == null)
                                {
                                    point.SetMaxOffsetCount(distances[y]);
                                    distances[y] = 0;
                                }
                                else
                                {
                                    previousCovers[y].SetMaxOffsetCount(distance + 1);
                                    point.SetMaxOffsetCount(distance + 1);
                                    previousCovers[y] = null;
                                    distances[y] = 0;
                                }
                            }
                            else
                            {
                                distances[y] += 1;
                            }
                        }
                        else if (!Map.Tiles[x, y - 1])
                        {
                            if (Map.Tiles[x - 1, y] && Map.Tiles[x - 1, y - 1])
                            {
                                CoverPoint point = new CoverPoint(new Vector3(x - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY),
                                    new Vector3(x - 1 - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY));
                                _Covers.Add(point);
                                previousCovers[y] = point;
                                distances[y] = 0;
                            }
                            else if (Map.Tiles[x + 1, y] && Map.Tiles[x + 1, y - 1])
                            {
                                CoverPoint point = new CoverPoint(new Vector3(x - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY),
                                    new Vector3(x + 1 - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY));
                                _Covers.Add(point);

                                if (previousCovers[y] == null)
                                {
                                    point.SetMaxOffsetCount(distances[y]);
                                    distances[y] = 0;
                                }
                                else
                                {
                                    previousCovers[y].SetMaxOffsetCount(distance + 1);
                                    point.SetMaxOffsetCount(distance + 1);
                                    previousCovers[y] = null;
                                    distances[y] = 0;
                                }
                            }
                            else
                            {
                                distances[y] += 1;
                            }
                        }
                        else if (!Map.Tiles[x + 1, y])
                        {
                            if (Map.Tiles[x + 1, y - 1] && Map.Tiles[x, y - 1])
                            {
                                CoverPoint point = new CoverPoint(new Vector3(x - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY),
                                    new Vector3(x - offsetX + fieldOffsetX, y - 1 - offsetY + fieldOffsetY));
                                _Covers.Add(point);
                                previousCover = point;
                                distance = 0;
                            }
                            else if (Map.Tiles[x + 1, y + 1] && Map.Tiles[x, y + 1])
                            {
                                CoverPoint point = new CoverPoint(new Vector3(x - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY),
                                    new Vector3(x - offsetX + fieldOffsetX, y + 1 - offsetY + fieldOffsetY));
                                _Covers.Add(point);

                                if (previousCover == null)
                                {
                                    point.SetMaxOffsetCount(distance);
                                    distance = 0;
                                }
                                else
                                {
                                    previousCover.SetMaxOffsetCount(distance + 1);
                                    point.SetMaxOffsetCount(distance + 1);
                                    previousCover = null;
                                    distance = 0;
                                }
                            }
                            else
                            {
                                distance += 1;
                            }
                        }
                        else if (!Map.Tiles[x - 1, y])
                        {
                            if (Map.Tiles[x, y - 1] && Map.Tiles[x - 1, y - 1])
                            {
                                CoverPoint point = new CoverPoint(new Vector3(x - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY),
                                    new Vector3(x - offsetX + fieldOffsetX, y - 1 - offsetY + fieldOffsetY));
                                _Covers.Add(point);
                                previousCover = point;
                                distance = 0;
                            }
                            else if (Map.Tiles[x, y + 1] && Map.Tiles[x - 1, y + 1])
                            {
                                CoverPoint point = new CoverPoint(new Vector3(x - offsetX + fieldOffsetX, y - offsetY + fieldOffsetY),
                                    new Vector3(x - offsetX + fieldOffsetX, y + 1 - offsetY + fieldOffsetY));
                                _Covers.Add(point);

                                if (previousCover == null)
                                {
                                    point.SetMaxOffsetCount(distance);
                                    distance = 0;
                                }
                                else
                                {
                                    previousCover.SetMaxOffsetCount(distance + 1);
                                    point.SetMaxOffsetCount(distance + 1);
                                    previousCover = null;
                                    distance = 0;
                                }
                            }
                            else
                            {
                                distance += 1;
                            }
                        }
                    }
                    else if (distance > 0)
                    {
                        if (previousCover != null)
                        {
                            previousCover.SetMaxOffsetCount(distance);
                            previousCover = null;
                        }
                        distance = 0;
                    }
                    else if (distances[y] > 0)
                    {
                        if (previousCovers[y] != null)
                        {
                            previousCovers[y].SetMaxOffsetCount(distances[y]);
                            previousCovers[y] = null;
                        }
                        distances[y] = 0;
                    }
                }
            }
            for (int z = 0; z < previousCovers.Length; z++)
            {
                if (previousCovers[z] != null)
                {
                    previousCovers[z].SetMaxOffsetCount(distances[z]);
                }
            }
        }
        _BuildingsLayer = BuildingsLayer;
        _SafeDistance = SafeDistance;
        _MaxDistance = MaxDistance;
    }

    public CoverPoint GetCover(Vector3 Position, Vector3 Target, float ViewDistance, float BuildingsRadius)
    {
        CoverPoint result = null;
        MapGrid map = GameController.Instance.Map;
        if (!map.Tiles[Mathf.FloorToInt(Position.x) + map.Offset_X, Mathf.FloorToInt(Position.y) + map.Offset_Y] ||
            !map.Tiles[Mathf.FloorToInt(Target.x) + map.Offset_X, Mathf.FloorToInt(Position.y) + map.Offset_Y])
        {
            return result;
        }
        if (_Covers != null && _Covers.Count > 0)
        {
            int index = 0;
            while (index < _Covers.Count)
            {
                float distance_target = Vector3.Distance(_Covers[index].Front, Target);
                float distance_unit = Vector3.Distance(_Covers[index].Position, Position);
                if (distance_target < ViewDistance && distance_target > _SafeDistance &&
                    distance_unit < _MaxDistance && distance_target > distance_unit)
                {
                    if (result == null)
                    {
                        if (!Seeable(_Covers[index].Position, Target, BuildingsRadius) &&
                            Seeable(_Covers[index].Front, Target, BuildingsRadius))
                        {
                            result = _Covers[index];
                        }
                    }
                    else if (Vector3.Distance(_Covers[index].Position, Position) < Vector3.Distance(result.Position, Position) &&
                        !Seeable(_Covers[index].Position, Target, BuildingsRadius) &&
                        Seeable(_Covers[index].Front, Target, BuildingsRadius))
                    {
                        result = _Covers[index];
                    }
                }
                index += 1;
            }
            _Covers.Remove(result);
        }
        return result;
    }

    public void FreeCover(CoverPoint Point)
    {
        if (Point != null)
        {
            Point.Reset();
            _Covers.Add(Point);
        }
    }

    bool Seeable(Vector3 Position, Vector3 Target, float Radius)
    {
        float distance = Vector3.Distance(Position, Target);
        Vector3 direction = (Target - Position);
        return (!Physics2D.CircleCast(Position, Radius, direction, distance, _BuildingsLayer));
    }
}
