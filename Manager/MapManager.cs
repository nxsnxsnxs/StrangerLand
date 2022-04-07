using System;
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
    public bool debug;
    public GameObject visualizedMapQuad;
    bool[,] mapGridsState = new bool[100, 100];
    void Awake()
    {
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        
    }
    public List<Vector3> FindPath(Vector3 start, GameObject target, float coverRadius)
    {
        if(!target.GetComponentInChildren<Collider>()) return FindPath(start, target.transform.position, coverRadius);
        return PathFinder.FindPath(mapGridsState, start, target.GetComponentInChildren<Collider>(), coverRadius) ?? new List<Vector3>{start, target.transform.position};
    }
    public List<Vector3> FindPath(Vector3 start, Vector3 end, float coverRadius)
    {
        return PathFinder.FindPath(mapGridsState, start, end, coverRadius) ?? new List<Vector3>{start, end};
    }
    public void RegisterBuildingLand(Collider collider)
    {
        RegisterBuildingLand(collider.bounds.center, collider.bounds.size.x, collider.bounds.size.z);
    }
    public void RegisterBuildingLand(Vector3 center, float length, float width)
    {
        for(int i = ToolMethod.EightTwoRoundToInt(center.x - length / 2); i < ToolMethod.TwoEightRoundToInt(center.x + length / 2); ++i)
        {
            for(int j = ToolMethod.EightTwoRoundToInt(center.z - width / 2); j < ToolMethod.TwoEightRoundToInt(center.z + width / 2); ++j)
            {
                SetCoveredPos(i, j);
                if(debug) Instantiate(visualizedMapQuad, new Vector3(i + 0.5f, 0.01f, j + 0.5f), Quaternion.Euler(90, 0, 0));
                //Debug.Log(String.Format("i:{0},j:{1}", i, j));
            }
        }
    }
    public void UnRegisterBuildingLand(Vector3 center, float length, float width)
    {
        for(int i = ToolMethod.EightTwoRoundToInt(center.x - length / 2); i < ToolMethod.TwoEightRoundToInt(center.x + length / 2); ++i)
        {
            for(int j = ToolMethod.EightTwoRoundToInt(center.z - width / 2); j < ToolMethod.TwoEightRoundToInt(center.z + width / 2); ++j)
            {
                ResetCoveredPos(i, j);
            }
        }
    }
    public bool CanStand(Vector3 pos, Collider coll)
    {
        return CanStand(pos, Mathf.Max(coll.bounds.size.x, coll.bounds.size.z) / 2);
    }
    public bool CanStand(Vector3 pos, float radius)
    {
        //Debug.Log("000");
        for(int i = ToolMethod.EightTwoRoundToInt(pos.x - radius); i < ToolMethod.TwoEightRoundToInt(pos.x + radius); ++i)
        {
            for(int j = ToolMethod.EightTwoRoundToInt(pos.z - radius); j < ToolMethod.TwoEightRoundToInt(pos.z + radius); ++j)
            {
                //Debug.Log(i + " " + j);
                if(GetGridState(i, j)) return false;
            }
        }
        return true;
    }
    public bool CanArrive(Vector3 start, Vector3 end, float coverRadius)
    {
        return CanStand(end, coverRadius) && PathFinder.FindPath(mapGridsState, start, end, coverRadius) != null;
    }
    public bool CanArrive(Vector3 start, Vector3 end, Collider coll)
    {
        return CanArrive(start, end, Mathf.Max(coll.bounds.size.x, coll.bounds.size.z) / 2);
    }
    private void SetCoveredPos(int x, int z)
    {
        int i = x + mapGridsState.GetLength(1) / 2;
        int j = z + mapGridsState.GetLength(0) / 2;
        if(i < 0 || j < 0 || i >= mapGridsState.GetLength(0) || j >= mapGridsState.GetLength(1)) return;
        mapGridsState[i, j] = true;
        //Debug.Log(String.Format("i:{0},j:{1}", i, j));
    }
    private void ResetCoveredPos(int x, int z)
    {
        int i = x + mapGridsState.GetLength(1) / 2;
        int j = z + mapGridsState.GetLength(0) / 2;
        if(i < 0 || j < 0 || i >= mapGridsState.GetLength(0) || j >= mapGridsState.GetLength(1)) return;
        mapGridsState[i, j] = false;
        //Debug.Log(String.Format("i:{0},j:{1}", i, j));
    }
    private bool GetGridState(int x, int z)
    {
        int i = x + mapGridsState.GetLength(1) / 2;
        int j = z + mapGridsState.GetLength(0) / 2;
        if(i < 0 || j < 0 || i >= mapGridsState.GetLength(0) || j >= mapGridsState.GetLength(1)) return true;
        return mapGridsState[i, j];
    }
    private void DrawVisualizedQuad(int i, int j)
    {

    }
}
