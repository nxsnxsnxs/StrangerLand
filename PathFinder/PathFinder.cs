using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace PathFinding
{
    public static class PathFinder
    {
        /// <summary>
        /// 路径点
        /// </summary>
        private class PathPoint : IComparable<PathPoint>
        {
            public short x;
            public short z;

            public PathPoint parent;
            
            public uint F;
            public uint G;
            public uint H;
            public PathPoint(short _x, short _z)
            {
                x = _x;
                z = _z;
                F = 0;
                G = 0;
                H = 0;
                parent = null;
            }
            public void Init(PathPoint _parent, int _endX, int _endZ)
            {
                parent = _parent;
                uint cost;
                if(x != parent.x && z != parent.z) cost = PathFinder.hypotenuseCost;
                else cost = PathFinder.legCost;
                G = parent.G + cost;
                H = (uint)(Mathf.Abs(_endZ - z) + Mathf.Abs(_endX - x)) * PathFinder.legCost;
                F = G + H;
            }
            public void TryUpdateParent(PathPoint newParent)
            {
                uint cost;
                if(x != newParent.x && z != newParent.z) cost = PathFinder.hypotenuseCost;
                else cost = PathFinder.legCost;
                if(newParent.G + cost < G)
                {
                    parent = newParent;
                    G = parent.G + cost;
                    F = G + H;
                }
            }
            public bool Equal(PathPoint pathPoint)
            {
                return (pathPoint.x == x && pathPoint.z == z);
            }
            public int CompareTo(PathPoint other)
            {
                if(other.F < F) return 1;
                else return 0;
            }
            public Vector3 GetVector()
            {
                return new Vector3(x, 0, z);
            }
            public string DebugStr
            {
                get => x + "," + z;
            }
        }

        public const uint legCost = 10;
        public const uint hypotenuseCost = 14;
        private static int startX;
        private static int startZ;
        private static int endX;
        private static int endZ;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grids"></param>grids为false表示上面没有建筑物覆盖
        /// <param name="start"></param>起点
        /// <param name="end"></param>终点
        /// <returns></returns>
        public static List<Vector3> FindPath(bool[,] grids, Vector3 start, Vector3 end)
        {
            if(start.x == end.x && start.z == end.z) return null;
            PriorityQueue<PathPoint> openList = new PriorityQueue<PathPoint>();
            List<PathPoint> closeList = new List<PathPoint>();
            //为了使角色不在寻路时走“回头路”，我们需要在取网格时根据方向而不是直接floor
            //简单说应该朝着靠近的方向取整
            startX = Mathf.FloorToInt(start.x);
            endX = Mathf.FloorToInt(end.x);
            startZ = Mathf.FloorToInt(start.z);
            endZ = Mathf.FloorToInt(end.z);
            if(end.x > start.x) startX++;
            else if(end.x < start.x) endX++;
            if(end.z > start.z) startZ++;
            else if(end.z < start.z) endZ++;

            openList.Enqueue(new PathPoint((short)startX, (short)startZ));
            bool find = false;
            while(openList.Count != 0 && !find)
            {
                PathPoint center = openList.Dequeue();
                if(center.F > 400)
                {
                    Debug.Log("pathfind fail");
                    break;
                }
                List<PathPoint> surroundPoint = GetSurroundPoint(grids, center);
                /*Debug.Log(center.x + " " + center.z);
                foreach (var item in surroundPoint)
                {
                    Debug.Log(item.x + " " + item.z);
                }*/
                foreach (var item in surroundPoint)
                {
                    if(closeList.Exists(item.Equal)) continue;
                    if(openList.Exists(item.Equal))
                    {
                        item.TryUpdateParent(center);
                    }
                    else
                    {
                        item.Init(center, endX, endZ);
                        if(item.x == endX && item.z == endZ)
                        {
                            find = true;
                            closeList.Add(center);
                            closeList.Add(item);
                            break;
                        }                          
                        openList.Enqueue(item);
                    }
                }
                if(!find) closeList.Add(center);
            }
            if(find)
            {
                PathPoint p = closeList[closeList.Count - 1];
                List<Vector3> path = new List<Vector3>();
                //仅保留拐点
                Vector2 lastDirection = new Vector2(0, 0);
                while(p.parent != null)
                {
                    PathPoint q = p.parent;
                    Vector2 direction = new Vector2(q.x - p.x, q.z - p.z);
                    if(direction != lastDirection) path.Add(p.GetVector());
                    lastDirection = direction;
                    p = q;
                }
                path.Add(p.GetVector());
                path.Reverse();
                //以精确点替代
                path[0] = start;
                path[path.Count - 1] = end;
                //连接可直达的点，去除不必要的拐点，进行路径平滑
                List<Vector3> result = new List<Vector3>();
                int pre = 0;
                int next = path.Count - 1;
                for(; next > pre; --next)
                {
                    //UnityEngine.Debug.Log(path[pre]);
                    //UnityEngine.Debug.Log(path[next]);
                    //相邻点无需检测肯定直接可达
                    if(next == pre + 1 || DirectlyReachable(grids, path[pre], path[next]))
                    {
                        //UnityEngine.Debug.Log(true);
                        result.Add(path[pre]);
                        pre = next;
                        next = path.Count;
                    }
                    //else UnityEngine.Debug.Log(false);
                }
                //添加剩余不可简化的点
                while(pre < path.Count)
                {
                    result.Add(path[pre++]);
                }
                return result;
            }
            else
            {
                List<Vector3> result = new List<Vector3>();
                result.Add(start);
                result.Add(end);
                return result;
            }
        }
        private static List<PathPoint> GetSurroundPoint(bool[,] grids, PathPoint center)
        {
            short indexX = center.x;
            short indexZ = center.z;
            //Debug.Log(indexX + " " + indexZ);

            List<PathPoint> surroundPoint = new List<PathPoint>();
            //1  2  3
            //4  0  5
            //6  7  8
            bool two = false, four = false, five = false, seven = false;

            //这里做了一些改动，因为人物存在一定体积的碰撞体，所以人要从x，y过的时候仅（x，y）为false是不够的
            //（x， y）为false仅能说明xy右上角的方格内没有建筑，但人物实际上需要（x-1，y）（x-1，y-1）（x，y-1）都为false
            //这周围四格都没有建筑才能站在（x，y）这一点
            //two
            if(Walkable(grids, indexX, indexZ + 1))
            {
                two = true;
                surroundPoint.Add(new PathPoint(center.x, (short)(center.z + 1)));
            }
            //four
            if(Walkable(grids, indexX - 1, indexZ))
            {
                four = true;
                surroundPoint.Add(new PathPoint((short)(center.x - 1), center.z));
            }
            //five
            if(Walkable(grids, indexX + 1, indexZ))
            {
                five = true;
                surroundPoint.Add(new PathPoint((short)(center.x + 1), center.z));
            }
            //seven
            if(Walkable(grids, indexX, indexZ - 1))
            {
                seven = true;
                surroundPoint.Add(new PathPoint(center.x, (short)(center.z - 1)));
            }
            //one
            if((two || four) && Walkable(grids, indexX - 1, indexZ + 1))
            {
                surroundPoint.Add(new PathPoint((short)(center.x - 1), (short)(center.z + 1)));
            }
            //three
            if((two || five) && Walkable(grids, indexX + 1, indexZ + 1))
            {
                surroundPoint.Add(new PathPoint((short)(center.x + 1), (short)(center.z + 1)));
            }
            //six
            if((four || seven) && Walkable(grids, indexX - 1, indexZ - 1))
            {
                surroundPoint.Add(new PathPoint((short)(center.x - 1), (short)(center.z - 1)));
            }
            //eight
            if((five || seven) && Walkable(grids, indexX + 1, indexZ - 1))
            {
                surroundPoint.Add(new PathPoint((short)(center.x + 1), (short)(center.z - 1)));
            }
            return surroundPoint;
        }
        private static bool Walkable(bool[,] grids, int x, int z)
        {
            //终点的那个点永远可达(因为不会实际站在那个点)
            //主要是考虑采用自动搜寻建筑物目标（向目标碰撞体发射线取终点）的方式，终点那个点对应的格子一定是被建筑物覆盖不可走的，
            //那样的话无论如何都会寻路失败，但寻路不应该因为那个点而失败所以添加此特判
            //但与此同时也就要求FindPath不应该传入NotWalkable的终点
            if(x == endX && z == endZ) return true;

            short rows = (short)grids.GetLength(0);
            short columns = (short)grids.GetLength(1);
            x += columns / 2;
            z += rows / 2;
            return (x >= 0 && x < rows && z >= 0 && z < columns &&
             !grids[x, z] && (x - 1 < 0 || !grids[x - 1, z]) && 
             (z - 1 < 0 || !grids[x, z - 1]) && (x - 1 < 0 || z - 1 < 0 || !grids[x - 1, z - 1]));
        }
        private static bool DirectlyReachable(bool[,] grids, Vector3 start, Vector3 end)
        {
            float gradient;
            if(end.x == start.x) gradient = float.MaxValue;//避免除0
            else gradient = (end.z - start.z) / (end.x - start.x);
            
            int startX = Mathf.FloorToInt(start.x);
            int startZ = Mathf.FloorToInt(start.z);
            int endX = Mathf.FloorToInt(end.x);
            int endZ = Mathf.FloorToInt(end.z);
            //斜率小于1时以x带入方程取点，大于1时代入z保证不会漏掉格子的检测
            if(Mathf.Abs(gradient) <= 1)
            {
                int beginLoop = Mathf.Min(startX, endX), endLoop = Mathf.Max(startX, endX);
                for(int i = beginLoop + 1; i < endLoop; ++i)
                {
                    int x = i, z = Mathf.FloorToInt(gradient * (i - start.x) + start.z);
                    if(!Walkable(grids, x, z))
                    {
                        //UnityEngine.Debug.DrawLine(start, new Vector3(x, 0, z), Color.blue, 180);
                        return false;
                    }
                    //UnityEngine.Debug.DrawLine(start, new Vector3(x, 0, z), Color.yellow, 180);
                }
            }
            else
            {
                int beginLoop = Mathf.Min(startZ, endZ), endLoop = Mathf.Max(startZ, endZ);
                for(int i = beginLoop + 1; i < endLoop; ++i)
                {
                    int x = (gradient != float.MaxValue) ? Mathf.FloorToInt((i - start.z) / gradient + start.x) : startX;
                    int z = i;
                    if(!Walkable(grids, x, z))
                    {
                        //UnityEngine.Debug.DrawLine(start, new Vector3(x, 0, z), Color.blue, 180);
                        return false;
                    }
                    //UnityEngine.Debug.DrawLine(start, new Vector3(x, 0, z), Color.yellow, 180);
                }
            }
            return true;
        }
    }
}