using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using PathFinding;
using Tools;
using Player.Construction;

/// <summary>
/// 网格位置（整数）
/// </summary>
public class GridPos
{
    public short x;
    public float y;
    public short z;
    public Vector3 Pos
    {
        get => new Vector3(x, y, z);
    }
    /// <summary>
    /// 将精确坐标转换为网格坐标
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static GridPos GetGridPos(Vector3 pos)
    {
        //向左下角取整
        GridPos gridPos = new GridPos();
        gridPos.x = (short)Mathf.Floor(pos.x);
        gridPos.y = pos.y;
        gridPos.z = (short)Mathf.Floor(pos.z);
        return gridPos;
    }
    public bool Equal(GridPos other)
    {
        if(other == null) return false;
        return x == other.x && z == other.z;
    }
    public string DebugStr
    {
        get { return "GridPos:" + x + " " + z; }
    }

}
public class MapManager : ManagerSingleton<MapManager>
{
    bool[,] mapGridsState;
    void Awake()
    {
        Application.targetFrameRate = 60;
        mapGridsState = new bool[121, 121];
    }
    void Start()
    {
        
    }
    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        return PathFinder.FindPath(mapGridsState, start, end);
    }
    public void AddBuilding(BuildingInfo info)
    {
        short x = (short)(info.center.x + mapGridsState.GetLength(1) / 2);
        short z = (short)(info.center.z + mapGridsState.GetLength(0) / 2);

        for(int i = x - info.stats.length / 2 * 1; i < x + info.stats.length / 2 * 1; ++i)
        {
            for(int j = z - info.stats.width / 2 * 1; j < z + info.stats.width / 2 * 1; ++j)
            {
                mapGridsState[i, j] = true;
                //Debug.Log(i + " " + j);
            }
        }
    }
    public bool HasEnoughLand(GridPos center, BuildingStats stats)
    {
        short x = (short)(center.x + mapGridsState.GetLength(1) / 2);
        short z = (short)(center.z + mapGridsState.GetLength(0) / 2);
        for(int i = x - stats.length / 2 * 1; i < x + stats.length / 2 * 1; ++i)
        {
            for(int j = z - stats.width / 2 * 1; j < z + stats.width / 2 * 1; ++j)
            {
                if(mapGridsState[i, j]) return false;
                //Debug.Log(i + " " + j);
            }
        }
        return true;
    }
    
}
