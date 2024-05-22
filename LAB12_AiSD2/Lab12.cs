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
            Double[] depths = new double[points.Length];
            Point start, end;
            int s = 0, m, e;

            for (int j = 0; j < points.Length; j++)
            {
                (start, s, m) = FindStart(points, s);
                if (m + 1 >= points.Length)
                    break;

                (end, e) = FindEnd(points, m, start);
                if (e == m)
                {
                    s = m;
                    continue;
                }
                
                double l = Math.Min(start.y, end.y);

                for (int i = s + 1; i < e; i++)
                {
                    depths[i] = l - points[i].y;
                }

                s = e;
            }

            return depths;
        }

        public (Point e, int eInd) FindEnd(Point[] points, int m, Point s) // pamietaj o sprawdzeniu m przed wywolaniem
        {
            Point e = points[m + 1];
            int ind = m + 1;
            
            if (e.y > s.y)
                return (e, ind);
            
            for (int i = m + 2; i < points.Length; i++)
            {
                if (points[i].x < e.x)
                    break;
                
                if (points[i].y > s.y)
                {
                    e = points[i];
                    return (e, i);
                }
                else if (points[i].y > e.y)
                {
                    e = points[i];
                    ind = i;
                }
            }

            if (e.y > points[m].y)
                return (e, ind);

            e = new Point(Int32.MinValue, Int32.MinValue);
            return (e, m);
        }

        public (Point p, int s, int m) FindStart(Point[] points, int s)
        {
            Point res = points[s];
            int ind = s;
            int m = s + 1;
            for (int i = s + 1; i < points.Length; i++)
            {
                if (res.y <= points[i].y)
                {
                    ind = i;
                    res = points[i];
                }
                else if (points[i].x > res.x)
                {
                    m = i;
                    break;
                }
            }

            return (res, ind, m);
        }
        
        /// <summary>
        /// Funkcja zwraca objętość wody, jaka zatrzyma się w górach.
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double WaterVolume(Point[] points)
        {
            return -1;
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
