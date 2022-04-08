using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace PathFinding
{
    //寻路坐标系
    //      z
    //      ⬆
    //      0➡x

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
        public const uint maxPathCost = 500;
        public const uint legCost = 10;
        public const uint hypotenuseCost = 14;
        private static int startX;
        private static int startZ;
        private static int endX;
        private static int endZ;
        //关于碰撞体寻路和普通寻路：
        //两种寻路的唯一不同在于CanReachTarget
        //对碰撞体寻路的关键是尽可能的走到离碰撞体边缘最近的点
        //所以寻路过程中用ClosestPointOnBounds动态确定终点，其余均与普通寻路相同
        //注意：使用碰撞体寻路的条件不是目标有碰撞体，而是目标覆盖了网格（建筑物等）
        //碰撞体寻路解决的是终点不可达（被网格覆盖）的情况

        //寻路过程中只有在CanStand中会转换为数组索引，其余均为真实的世界坐标
        public static List<Vector3> FindPath(bool[,] grids, Vector3 start, Vector3 end, float coverRadius)
        {
            if(start.x == end.x && start.z == end.z) return null;
            if(!CanStand(grids, end, coverRadius))
            {
                Debug.Log("wrong terminal");
                return null;
            }
            if(DirectlyReachable(grids, start, end, coverRadius)) return new List<Vector3>(){start, end};
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
                if(center.F > maxPathCost)
                {
                    Debug.Log("pathfind fail");
                    Vector3 last = start;
                    for(int i = 0; i < 20; ++i)
                    {
                        GameObject go = new GameObject();
                        go.transform.position = closeList[i].GetVector();
                        Debug.DrawLine(last, go.transform.position, Color.blue, 180);
                        last = go.transform.position;
                    }
                    break;
                }
                List<PathPoint> surroundPoint = GetSurroundPoint(grids, center, coverRadius);
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
                        if(CanReachTarget(item, end, coverRadius))
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
                List<Vector3> path = GetPath(closeList, start, end);
                List<Vector3> result = GetSmoothPath(grids, path, coverRadius);
                return result;
            }
            else return null;
        }
        public static List<Vector3> FindPath(bool[,] grids, Vector3 start, Collider target, float coverRadius)
        {
            Vector3 end = target.transform.position;
            if(start.x == end.x && start.z == end.z) return null;
            if(DirectlyReachable(grids, start, end, coverRadius)) return new List<Vector3>(){start, end};
            PriorityQueue<PathPoint> openList = new PriorityQueue<PathPoint>();
            List<PathPoint> closeList = new List<PathPoint>();

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
                if(center.F > maxPathCost)
                {
                    Debug.Log("pathfind fail");
                    break;
                }
                List<PathPoint> surroundPoint = GetSurroundPoint(grids, center, coverRadius);
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
                        if(CanReachTarget(item, target, coverRadius))
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
                List<Vector3> path = GetPath(closeList, start, end);
                List<Vector3> result = GetSmoothPath(grids, path, coverRadius);
                return result;
            }
            else return null;
        }
        //从closeList中提取路径，仅保留拐点，并在首尾加入真正的起点和终点
        private static List<Vector3> GetPath(List<PathPoint> pointList, Vector3 start, Vector3 end)
        {
            PathPoint p = pointList[pointList.Count - 1];
            List<Vector3> path = new List<Vector3>(){end};
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
            path.Add(start);
            path.Reverse();
            return path;
        }
        //连接可直达的点，去除不必要的拐点，进行路径平滑
        private static List<Vector3> GetSmoothPath(bool[,] grids, List<Vector3> path, float coverRadius)
        {
            List<Vector3> result = new List<Vector3>();
            int pre = 0;
            int next = path.Count - 1;
            for(; next > pre; --next)
            {
                //UnityEngine.Debug.Log(path[pre]);
                //UnityEngine.Debug.Log(path[next]);
                //相邻点无需检测肯定直接可达
                if(next == pre + 1 || DirectlyReachable(grids, path[pre], path[next], coverRadius))
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

        private static bool CanReachTarget(PathPoint curr, Collider target, float radius)
        {
            Vector3 pos = curr.GetVector();
            return CanReachTarget(curr, target.ClosestPointOnBounds(pos), radius);
        }
        //因为我们寻找的过程中每次是一格一格移动的，所以只要终点距离我们不超过一格（加上我们自身的碰撞半径），
        //我们就认为已经能到达了终点（因为在一格内不可能存在阻碍）
        private static bool CanReachTarget(PathPoint curr, Vector3 dst, float radius)
        {
            int dis = Mathf.FloorToInt(radius) + 1;
            Vector3 pos = curr.GetVector();
            return Mathf.Abs(pos.x - dst.x) <= dis && Mathf.Abs(pos.z - dst.z) <= dis;
        }
        private static List<PathPoint> GetSurroundPoint(bool[,] grids, PathPoint center, float coverRadius)
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
            if(CanStand(grids, indexX, indexZ + 1, coverRadius))
            {
                two = true;
                surroundPoint.Add(new PathPoint(center.x, (short)(center.z + 1)));
            }
            //four
            if(CanStand(grids, indexX - 1, indexZ, coverRadius))
            {
                four = true;
                surroundPoint.Add(new PathPoint((short)(center.x - 1), center.z));
            }
            //five
            if(CanStand(grids, indexX + 1, indexZ, coverRadius))
            {
                five = true;
                surroundPoint.Add(new PathPoint((short)(center.x + 1), center.z));
            }
            //seven
            if(CanStand(grids, indexX, indexZ - 1, coverRadius))
            {
                seven = true;
                surroundPoint.Add(new PathPoint(center.x, (short)(center.z - 1)));
            }
            //one
            if((two || four) && CanStand(grids, indexX - 1, indexZ + 1, coverRadius))
            {
                surroundPoint.Add(new PathPoint((short)(center.x - 1), (short)(center.z + 1)));
            }
            //three
            if((two || five) && CanStand(grids, indexX + 1, indexZ + 1, coverRadius))
            {
                surroundPoint.Add(new PathPoint((short)(center.x + 1), (short)(center.z + 1)));
            }
            //six
            if((four || seven) && CanStand(grids, indexX - 1, indexZ - 1, coverRadius))
            {
                surroundPoint.Add(new PathPoint((short)(center.x - 1), (short)(center.z - 1)));
            }
            //eight
            if((five || seven) && CanStand(grids, indexX + 1, indexZ - 1, coverRadius))
            {
                surroundPoint.Add(new PathPoint((short)(center.x + 1), (short)(center.z - 1)));
            }
            return surroundPoint;
        }
        private static bool CanStand(bool[,] grids, int x, int z, float coverRadius)
        {
            return CanStand(grids, new Vector3(x, 0, z), coverRadius);
        }
        private static bool CanStand(bool[,] grids, Vector3 pos, float coverRadius)
        {
            short rows = (short)grids.GetLength(0);
            short columns = (short)grids.GetLength(1);
            pos.x += columns / 2;
            pos.z += rows / 2;

            for(int i = ToolMethod.EightTwoRoundToInt(pos.x - coverRadius); i < ToolMethod.TwoEightRoundToInt(pos.x + coverRadius); ++i)
            {
                for(int j = ToolMethod.EightTwoRoundToInt(pos.z - coverRadius); j < ToolMethod.TwoEightRoundToInt(pos.z + coverRadius); ++j)
                {
                    //Debug.Log(i + " " + j);
                    if(i < 0 || j < 0 || i >= rows || j >= columns || grids[i, j]) return false;
                }
            }
            return true;
        }
        private static bool DirectlyReachable(bool[,] grids, Vector3 start, Vector3 end, float coverRadius)
        {
            float gradient;
            if(end.x == start.x) gradient = float.MaxValue;//避免除0
            else gradient = (end.z - start.z) / (end.x - start.x);
            //和FindPath中的取整起点终点方法一样
            int startX = Mathf.FloorToInt(start.x);
            int startZ = Mathf.FloorToInt(start.z);
            int endX = Mathf.FloorToInt(end.x);
            int endZ = Mathf.FloorToInt(end.z);
            if(end.x > start.x) startX++;
            else if(end.x < start.x) endX++;
            if(end.z > start.z) startZ++;
            else if(end.z < start.z) endZ++;
            //斜率小于1时以x带入方程取点，大于1时代入z保证不会漏掉格子的检测
            if(Mathf.Abs(gradient) <= 1)
            {
                int beginLoop = Mathf.Min(startX, endX), endLoop = Mathf.Max(startX, endX);
                for(int i = beginLoop; i <= endLoop; ++i)
                {
                    float x = i, z = gradient * (i - start.x) + start.z;
                    
                    if(!CanStand(grids, new Vector3(x, 0, z), coverRadius))
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
                for(int i = beginLoop; i <= endLoop; ++i)
                {
                    float x = (gradient != float.MaxValue) ? (i - start.z) / gradient + start.x : start.x;
                    float z = i;
                    if(!CanStand(grids, new Vector3(x, 0, z), coverRadius))
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