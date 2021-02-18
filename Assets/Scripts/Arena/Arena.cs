using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Arena;
using Photon.Pun;

public class Arena : MonoBehaviour
{
    [SerializeField]
    int row, column;
    [SerializeField]
    Vector2 startPosition;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CreateMap();
        }
    }
    void CreateMap()
    {
        WallTypes randomWallType;
        ArenaCell arenaCell;
        ArenaCell[,] arena = new ArenaCell[row, column];
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                arenaCell = new ArenaCell();
                randomWallType = (WallTypes)Random.Range(1, 3);
                arenaCell.wallTypes = randomWallType;
                arena[r, c] = arenaCell;
            }
        }
        DetectOpenDriveway(arena);
        OpenClosedDriveway(arena);
        DrawMap(arena);
    }
    void DetectOpenDriveway(ArenaCell[,] arena, int r = 0, int c = 0)
    {
        if (arena[r, c].isAccessible.Equals(false)) {
            arena[r, c].isAccessible = true;
            if (c - 1 >= 0)
            {
                if (!arena[r, c - 1].wallTypes.Equals(WallTypes.Vertical))
                {
                    DetectOpenDriveway(arena, r, c - 1);
                }
            }
            if (r - 1 >= 0)
            {
                if (!arena[r - 1, c].wallTypes.Equals(WallTypes.Horizontal))
                {
                    DetectOpenDriveway(arena, r - 1, c);
                }
            }
            if (!arena[r, c].wallTypes.Equals(WallTypes.Horizontal) && r + 1 < row)
            {
                DetectOpenDriveway(arena, r + 1, c);
            }
            if (!arena[r, c].wallTypes.Equals(WallTypes.Vertical) && c + 1 < column)
            {
                DetectOpenDriveway(arena, r, c + 1);
            }
        }
        return;
    }
    void OpenClosedDriveway(ArenaCell[,] arena)
    {
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                if (arena[r, c].isAccessible.Equals(false))
                {
                    if (c - 1 >= 0)
                    {
                        if (arena[r, c - 1].isAccessible.Equals(true))
                        {
                            arena[r, c - 1].wallTypes = WallTypes.None;
                        }
                    }
                    else if (r - 1 >= 0)
                    {
                        if (arena[r - 1, c].isAccessible.Equals(true))
                        {
                            arena[r - 1, c].wallTypes = WallTypes.None;
                        }
                    }
                    else if (arena[r, c + 1].isAccessible.Equals(true) && r + 1 < row)
                    {
                        arena[r, c + 1].wallTypes = WallTypes.None;
                    }
                    else if (arena[r + 1, c].isAccessible.Equals(true) && c + 1 < column)
                    {
                        arena[r + 1, c].wallTypes = WallTypes.None;
                    }

                    for (int r2 = 0; r2 < row; r2++)
                    {
                        for (int c2 = 0; c2 < column; c2++)
                        {
                            arena[r2, c2].isAccessible = false;
                        }
                    }
                    DetectOpenDriveway(arena, 0, 0);
                }
            }
        }
    }
    void DrawMap(ArenaCell[,] arena)
    {
        GameObject childObject;
        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < column; c++)
            {
                if (c == column - 1 && arena[r, c].wallTypes == WallTypes.Vertical)
                {
                    continue;
                }
                else if (r == row - 1 && arena[r, c].wallTypes == WallTypes.Horizontal)
                {
                    continue;
                }
                else
                {
                    if (arena[r, c].wallTypes == WallTypes.Horizontal)
                    {
                        childObject = PhotonNetwork.Instantiate("Horizontal Wall Point", startPosition + new Vector2(c * 2, r * -2), Quaternion.identity);
                        childObject.transform.parent = transform;
                    }
                    else if (arena[r, c].wallTypes == WallTypes.Vertical)
                    {
                        childObject = PhotonNetwork.Instantiate("Vertical Wall Point", startPosition + new Vector2(c * 2, r * -2), Quaternion.identity);
                        childObject.transform.parent = transform;
                    }
                }
            }
        }
    }
}