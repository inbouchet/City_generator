using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall
{    
    private int nombrePillier,rayon,petitRayon;

    public Wall(int nombrePillier,int rayon,int petitRayon)
    {
        this.nombrePillier = nombrePillier;
        this.rayon = rayon;
        this.petitRayon = petitRayon;
    }

    public List<Vector2> createPoints()
    {   
        List<Vector2> points = new List<Vector2>();
        // definition de la partie du perimetre du cercle ou on veut avoir notre point
        float anlgeChange = 90.0f;
        float angleStart = 0.0f;
        float angleEnd = anlgeChange;

        for(int i = 0 ; i < nombrePillier ; i++ )
        {   
            // Creation d'un premier cercle
            float randomAngle = Random.Range(angleStart, angleEnd);
            float x = rayon * Mathf.Cos(randomAngle * Mathf.Rad2Deg);
            float y = rayon * Mathf.Sin(randomAngle * Mathf.Rad2Deg);

            // Creation d'un plus petit cercle
            Vector3 randomCercle = Random.insideUnitSphere * petitRayon;
            Vector2 point = new Vector2(x + randomCercle.x , y +  randomCercle.y);
            points.Add(point);
            
            foreach(Vector2 p in points)
            {
                if(Vector2.Distance(point,p) < 60f && Vector2.Distance(point,p) != 0)
                {
                    points.Remove(point);
                    break;
                }
            }
            
            // gerer un cart de cercle a la fois
            if(angleEnd >= 360.0f)
            {
                angleStart = 0.0f;
                angleEnd = anlgeChange;
            }
            else
            {
                angleStart += anlgeChange;
                angleEnd += anlgeChange;
            }
        }
        points.Add(points[0]);
        return points;
    }


    public List<Vector3> createWalls(List<Vector2> points)
    {
        
        Vector2 center = Geometry.FindCenter(points);

        List<Vector2> FinalPoints = Geometry.SortAdjListToRight(points,center);

        List<Vector3> walls = new List<Vector3>();
        foreach(Vector2 point in FinalPoints)
        {
            walls.Add(new Vector3(point.x , 0.5f , point.y));
        }
        return walls;
    }
}
