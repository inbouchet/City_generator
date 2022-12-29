using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry
{
    // Sorting an array of Vector2
    public static void Selection_sort_x(List<Vector2> vec)
    {
        int i, j;
        Vector2 x;
	    for (i = 1; i < vec.Count; i++){
            x = vec[i];
            j = i - 1;
            while (j >= 0 && vec[j].x > x.x){
                vec[j + 1] = vec[j];
                j = j - 1;
            }
            vec[j + 1] = x;
	    }
    }

    // Creates a convexe polygone
    public static List<Vector2> MakePolygone(List<Vector2> points)
    {
        List<Vector2> pointsSortedUpY = new List<Vector2>();
        List<Vector2> pointsSortedDownY = new List<Vector2>();

        float maxY = (float)long.MinValue;
        float minY = (float)long.MaxValue;
        foreach(Vector2 point in points)
        {
            if(point.y > maxY)
            {
                maxY = point.y;
            }
            if(point.y < minY)
            {
                minY = point.y;
            }
        }

        float middlePoint = (maxY + minY) / 2;

        // sorting elements
        for (int i = 0; i < points.Count; i++) {
            if(points[i].y >= middlePoint){
                pointsSortedUpY.Add(points[i]);
            }
            else{
                pointsSortedDownY.Add(points[i]);
            }
        }
        Geometry.Selection_sort_x(pointsSortedUpY);
        Geometry.Selection_sort_x(pointsSortedDownY);

        List<Vector2> FinalPoints = new List<Vector2>();
        
        foreach(Vector2 point in pointsSortedUpY)
        {
            FinalPoints.Add(point);
        }

        for(int i = pointsSortedDownY.Count - 1 ; i >= 0 ; i--)
        {
            FinalPoints.Add(pointsSortedDownY[i]);
        }

        return FinalPoints;
    }

    // IsInside returns a boolean to see if the point is inside the set of Points
    public static bool IsInside(List<Vector2> points, Vector2 point)
    {
        bool isInside = false;

        float x1,x2,y1,y2,x,y;
        x = point.x;
        y = point.y;

        for(int i = 0 ; i < points.Count ; ++i)
        {
            if(i == points.Count - 1)
            {
                x1 = points[i].x;
                x2 = points[0].x;
                y1 = points[i].y;
                y2 = points[0].y;
            }
            else
            {   
                x1 = points[i].x;
                x2 = points[i+1].x;
                y1 = points[i].y;
                y2 = points[i+1].y;
            }

            if(y < y1 != y < y2 && x < (x2-x1) * (y-y1) / (y2-y1) + x1){
                isInside=!isInside;
            }
        }
        return isInside;
    }

    // This function is used to move randomly all points uniformly away from the 0,0 vector
    // with a float parametre for the maximum distance that we should not excede.
    public static List<Vector2> MovePointsRandom(List<Vector2> points, float maxDistance)
    {
        List<Vector2> result = new List<Vector2>();

        Vector2 distance = Random.insideUnitCircle * maxDistance;

        foreach(Vector2 point in points)
        {
            result.Add(new Vector2(point.x + distance.x , point.y + distance.y));
        }

        return result;
    }

    public static Vector2 FindCenter(List<Vector2> cell)
    {
        Vector2 pos = Vector3.zero;
        foreach(Vector2 p in cell)
        {
            pos += p;
        }
        return pos / cell.Count;
    }

    public static bool IsPointInPolygon(Vector2 point, Vector2[] polygon) {
        int polygonLength = polygon.Length, i=0;
        bool inside = false;
        // x, y for tested point.
        float pointX = point.x, pointY = point.y;
        // start / end point for the current polygon segment.
        float startX, startY, endX, endY;
        Vector2 endPoint = polygon[polygonLength-1];           
        endX = endPoint.x; 
        endY = endPoint.y;
        while (i<polygonLength) {
            startX = endX;           startY = endY;
            endPoint = polygon[i++];
            endX = endPoint.x;       endY = endPoint.y;
            //
            inside ^= ( endY > pointY ^ startY > pointY ) /* ? pointY inside [startY;endY] segment ? */
                        && /* if so, test if it is under the segment */
                        ( (pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY) ) ;
        }
        return inside;
    }

    public static List<Vector2> MovePoints(List<Vector2> points, Vector2 point)
    {
        List<Vector2> result = new List<Vector2>();

        foreach(Vector2 p in points)
        {
            result.Add(new Vector2(p.x + point.x , p.y + point.y));
        }

        return result;
    }

    // Return True if a point is too close to the set of points
    public static bool pointsTooClose(List<Vector2> points, Vector2 point, float distance)
    {
        foreach(Vector2 p in points)
        {
            if(Mathf.Abs(Vector2.Distance(p,point)) < distance && Vector2.Distance(p,point) != 0)
            {
                return true;
            }
        }
        return false;
    }

    public static List<Vector2> SortAdjListToRight(List<Vector2> list, Vector2 middlePoint)
    {
        // Getting rid of repeated points.
        list = list.Distinct().ToList();

        Dictionary<Vector2, float> angleOfPoint = new Dictionary<Vector2, float>();
        List<float> angles = new List<float>();

        foreach(var point in list)
        {
            float dx = point.x - middlePoint.x;
            float dy = point.y - middlePoint.y;

            float angle = Mathf.Atan2(dx,dy) * Mathf.Rad2Deg;
            // mapping to 0-360 fron 0-180
            angle = (angle + 360) % 360;

            angles.Add(angle);
            angleOfPoint.Add(point,angle);
        }

        // Sorting by the angle
        angles.Sort();

        List<float> trigonometricAngle = new List<float>();

        foreach(var angle in angles)
        {
            if(angle > 270 &&  angle <= 360)
            {
                trigonometricAngle.Add(angle);
            }
        }

        foreach(var angle in trigonometricAngle)
        {
            angles.Remove(angle);
        }

        foreach(var angle in angles)
        {
            trigonometricAngle.Add(angle);
        }

        List<Vector2> result = new List<Vector2>();
        foreach(var angleDeg in trigonometricAngle)
        {
            foreach(var dic in angleOfPoint)
            {
                if(angleDeg == dic.Value)
                {
                    result.Add(dic.Key);
                    angleOfPoint.Remove(dic.Key);
                    break;
                }
            }
        }

        return result;
    }
}
