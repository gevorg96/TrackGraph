using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    class Graph
    {
        public List<Edge> edges { get; set; }
        public List<Point> track { get; set; }
        private const int del = 2048;
        public Obl[,] obl = new Obl[del,del];
        public Graph()
        {}


        //#1
        public void GetGraph()
        {
            edges = new List<Edge>();
            string path = "ways(with geom).txt";
            using (var sr = new StreamReader(path))
            {
                var str = sr.ReadLine();

                while (!string.IsNullOrEmpty(str))
                {
                    var ws = new List<Point>();
                    var arr = str.Split(';');
                    for (int i = 4; i < arr.Length; i += 2)
                    {
                        ws.Add(new Point()
                        {
                            X = Convert.ToDouble(arr[i]),
                            Y = Convert.ToDouble(arr[i + 1]),
                        });
                    }
                   
                    edges.Add(new Edge()
                    {
                        gid = Convert.ToInt32(arr[0]),
                        source = Convert.ToInt32(arr[1]),
                        target = Convert.ToInt32(arr[2]),
                        //length = Convert.ToDouble(arr[3]),
                        way = ws
                    });
                    sr.ReadLine();
                    str = sr.ReadLine();
                }
            }
        }

        //#2
        public void MakeSectors()
        {
            Point north = new Point(35.1676139739991, 55.9052378296772);    //крайняя точка
            Point south = new Point(40.1988137756592, 55.5261913497181);    //крайняя точка
            Point west = new Point(38.5266716496618, 54.2560335009970);     //крайняя точка
            Point east = new Point(37.590836520902, 56.9382029639867);      //крайняя точка
            Point[] angles = new Point[4];
            double[] distances = new double[2];

            angles[0] = new Point(north.X, west.Y);
            angles[1] = new Point(north.X, east.Y);
            angles[2] = new Point(south.X, east.Y);
            angles[3] = new Point(south.X, west.Y);
            distances[0] = south.X - north.X;
            distances[1] = east.Y - west.Y;
            Point delta = new Point(distances[0]/del, distances[1]/del);
            for (int i = 0; i < del; i++)
            {
                for (int j = 0; j < del; j++)
                {
                    var t1 = new Point(angles[0].X + delta.X * i, angles[0].Y + delta.Y * j);
                    var t2 = new Point(angles[0].X + delta.X * (i+1), angles[0].Y + delta.Y * (j+1));
                    var lst = GetGeometryFromSector(t1, t2);
                    obl[i,j] = new Obl(t1, t2, lst);
                }
            }
        }

        //#3
        public void ParseTrack()
        {
            track = new List<Point>();
            string path = "Coord.txt";
            using (var sr = new StreamReader(path))
            {
                var str = sr.ReadLine();

                while (!string.IsNullOrEmpty(str))
                {
                    var arr = str.Split(';');
                    track.Add(new Point()
                    {
                        X = Convert.ToDouble(arr[0]),
                        Y = Convert.ToDouble(arr[1])
                    });
                    str = sr.ReadLine();
                }
            }
        }

        //#4
        private Obl? FindSector(Point point)
        {
            int[] first = {0, 0};
            int[] last = {del-1, del-1};
            var firstPoint = point;

            while(first[0] < last[0] && first[1] < last[1])
            {
                int[] mid = {first[0] + (last[0] - first[0]) / 2, first[1] + (last[1] - first[1]) / 2};
                if (firstPoint<=obl[mid[0], mid[1]].down)
                {
                    last = mid;
                }
                else if (firstPoint >= obl[mid[0], mid[1]].down)
                {
                    first = new []{mid[0] + 1, mid[1] + 1};

                }

                else if (MoreXLessY(firstPoint, obl[mid[0], mid[1]].down))
                {
                    first[0] = mid[0]+1;
                    last[1] = mid[1];
                }
                else
                {
                    first[1] = mid[1]+1;
                    last[0] = mid[0];
                }
            }

            if (IsThisSector(firstPoint, obl[last[0], last[1]]))
                return obl[last[0], last[1]];
             return null;
        }

        private Obl?[] FindSectors()
        {
            Obl?[] obls = new Obl?[track.Count];
            for (int i = 0; i < track.Count; i++)
            {
                obls[i] = FindSector(track[i]);
            }

            return obls;
        }

        private bool IsThisSector(Point point, Obl obl)
        {
            if (point.X > obl.up.X && point.X < obl.down.X
                                   && point.Y > obl.up.Y && point.Y < obl.down.Y)
                return true;
            return false;
        }
        public static bool MoreXLessY(Point p1, Point p2) => p1.X > p2.X && p1.Y < p2.Y;
        public static bool LessXMoreY(Point p1, Point p2) => p1.X < p2.X && p1.Y > p2.Y;

        //#5
        private List<int> GetGeometryFromSector(Point p1, Point p2)
        {
            List<int> lst = new List<int>();
            var obl = new {up = p1, down = p2};
            for (int i = 0; i < edges.Count; i++)
            {
                var t = edges[i].way;
                var b1 = t[0].X >= obl.up.X && t[0].Y >= obl.up.Y
                                            && t[0].X <= obl.down.X &&
                                            t[0].Y <= obl.down.Y;
                var b2 = t[t.Count - 1].X >= obl.up.X && t[t.Count - 1].Y >= obl.up.Y
                                                      && t[t.Count - 1].X <= obl.down.X &&
                                                      t[t.Count - 1].Y <= obl.down.Y;
                if (b1 || b2)
                {
                    lst.Add(edges[i].gid);
                }
            }

            return lst;
        }

        public List<List<int>> GetOblId()
        {
            List<List<int>> lst = new List<List<int>>();
            var obls = FindSectors();
            foreach (var t in obls)
            {
                lst.Add(t.GetValueOrDefault().edgeId);
            }

            return lst;
        }

    }

    public struct Edge
    {
        public int gid;
        public int source;
        public int target;
        //public double length;
        public List<Point> way;
    }

    public struct Point
    {
        public double X;
        public double Y;

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static bool operator <=(Point p1, Point p2) => p1.X<p2.X && p1.Y<p2.Y;
        public static bool operator >=(Point p1, Point p2) => p1.X > p2.X && p1.Y > p2.Y;

    }

    public struct Obl
    {
        public Point up;
        public Point down;
        public List<int> edgeId;

        public Obl(Point _up, Point _down, List<int> lst)
        {
            up = _up;
            down = _down;
            edgeId = lst;
        }
    }

}
