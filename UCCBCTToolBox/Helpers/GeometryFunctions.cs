using System.Windows;

namespace UCCBCTToolBox.Helpers
{
    internal class GeometryFunctions
    {
        // Given Y. Get X value of a line passing through point p1 and p2
        public static double GetXFromY(Point p1, Point p2, double y)
        {
            if (p1.X == p2.X)
            {
                return p1.X;
            }
            else if (p1.Y == p2.Y)
            {
                if (y == p1.Y)
                {
                    return p1.X;
                }
                else
                {
                    return double.NaN;
                }
            }
            else
            {
                return p1.X + (y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y);
            }
        }

        // Given X. Get Y value of a line passing through point p1 and p2
        public static double GetYFromX(Point p1, Point p2, double x)
        {
            if (p1.Y == p2.Y)
            {
                return p1.Y;
            }
            else if (p1.X == p2.X)
            {
                if (x == p1.X)
                {
                    return p1.Y;
                }
                else
                {
                    return double.NaN;
                }
            }
            else
            {
                return p1.Y + (x - p1.X) * (p2.Y - p1.Y) / (p2.X - p1.X);
            }
        }

        // Get the middle point of two point
        public static Point GetMidpointOfTwoPoints(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }
        public static Point GetIntersectionOfTwoLine(Point p, Point q, Point r, Point s)
        {
            double a1, a2, b1, b2, c1, c2, m;
            m = (q.X - p.X) / (q.Y - p.Y);
            a1 = 1;
            b1 = -m;
            c1 = -p.X + m * p.Y;

            m = (s.X - r.X) / (s.Y - r.Y);
            a2 = 1;
            b2 = -m;
            c2 = -r.X + m * r.Y;

            Point intersection = new((b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1), (c1 * a2 - c2 * a1) / (a1 * b2 - a2 * b1));

            return intersection;
        }
    }
}
