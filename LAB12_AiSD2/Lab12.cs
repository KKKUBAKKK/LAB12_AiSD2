using System;
using System.Collections.Generic;

namespace ASD
{
    public class WaterCalculator : MarshalByRefObject
    {

        /*
         * Metoda sprawdza, czy przechodząc p1->p2->p3 skręcamy w lewo 
         * (jeżeli idziemy prosto, zwracany jest fałsz).
         */
        private bool leftTurn(Point p1, Point p2, Point p3)
        {
            Point w1 = new Point(p2.x - p1.x, p2.y - p1.y);
            Point w2 = new Point(p3.x - p2.x, p3.y - p2.y);
            double vectProduct = w1.x * w2.y - w2.x * w1.y;
            return vectProduct > 0;
        }


        /*
         * Metoda wyznacza punkt na odcinku p1-p2 o zadanej współrzędnej y.
         * Jeżeli taki punkt nie istnieje (bo cały odcinek jest wyżej lub niżej), zgłaszany jest wyjątek ArgumentException.
         */
        private Point getPointAtY(Point p1, Point p2, double y)
        {
            if (p1.y != p2.y)
            {
                double newX = p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y);
                if ((newX - p1.x) * (newX - p2.x) > 0)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point(p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y), y);
            }
            else
            {
                if (p1.y != y)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point((p1.x + p2.x) / 2, y);
            }
        }


        /// <summary>
        /// Funkcja zwraca tablice t taką, że t[i] jest głębokością, na jakiej znajduje się punkt points[i].
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double[] PointDepths(Point[] points)
        {
            return B(points);
        }

        public (int s, int e)[] Split(Point[] points)
        {
            List<(int s, int e)> lakes = new List<(int s, int e)>();
            (int s, int e) lake = (0, 0);
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].x < points[i - 1].x)
                {
                    lakes.Add(lake);
                    lake.s = i;
                    lake.e = i;
                }
                else
                {
                    lake.e++;
                }
            }
            lakes.Add(lake);

            return lakes.ToArray();
        }

        public double[] B(Point[] points)
        {
            var lakes = Split(points);
            var res = new List<double>();
            for (int i = 0; i < lakes.Length; i++)
            {
                if (lakes[i].e - lakes[i].s >= 2)
                    res.AddRange(A(points, lakes[i].s, lakes[i].e));
                else
                    res.AddRange(new double[lakes[i].e - lakes[i].s + 1]);
            }

            return res.ToArray();
        }

        public List<double> A(Point[] points, int s, int e)
        {
            double[] heights = new double[e - s + 1];

            heights[0] = points[s].y;
            for (int i = 1; i < heights.Length; i++)
            {
                heights[i] = Math.Max(heights[i - 1], points[s + i].y);
            }

            heights[heights.Length - 1] = points[e].y;
            for (int i = heights.Length - 2; i >= 0; i--)
            {
                heights[i] = Math.Min(heights[i], Math.Max(heights[i + 1], points[s + i].y));
            }

            for (int i = 0; i < heights.Length - 1; i++)
            {
                if (points[i + s].y >= heights[i])
                    heights[i] = 0;
                else
                {
                    heights[i] -= points[i + s].y;
                }
            }
            heights[heights.Length - 1] = 0;

            return new List<double>(heights);
        }
        
        /// <summary>
        /// Funkcja zwraca objętość wody, jaka zatrzyma się w górach.
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double WaterVolume(Point[] points)
        {
            double volume = 0;
            var heights = B(points);
            for (int i = 0; i < heights.Length - 1; i++)
            {
                if (heights[i] == 0 && heights[i + 1] == 0)
                    continue;

                var pts = GetPoints(points[i], points[i + 1], heights[i], heights[i + 1]);
                volume += CalculateVolume(pts.left, pts.right, pts.height);
            }

            return volume;
        }

        public (Point left, Point right, double height) GetPoints(Point l, Point r, double hl, double hr)
        {
            if (l.y > r.y + hr)
            {
                var nl = getPointAtY(l, r, r.y + hr);
                return (nl, r, 0);
            }
            
            if (r.y > l.y + hl)
            {
                var nr = getPointAtY(l, r, l.y + hl);
                return (l, nr, 0);
            }
            
            return (l, r, Math.Min(hl, hr));
        }

        public double CalculateVolume(Point left, Point right, double height)
        {
            var dx = right.x - left.x;
            var dy = Math.Abs(right.y - left.y);
            
            return dx * dy / 2 + height * dx;
        }
    }

    [Serializable]
    public struct Point
    {
        public double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
