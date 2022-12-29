using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HO
{
public class Houses
{
    private List<GameObject> houses = new List<GameObject>();
    List<GameObject> aroundFountain = new List<GameObject>();
    GameObject centerElement = new GameObject();

    public int numHouse = 0;
    public Houses(List<string> houseAsset, string fountain , List<string> objects, int numHouse = 0)
    {
        this.numHouse = numHouse;
        this.numHouse++;
        foreach(var house in houseAsset)
        {
            GameObject houseObject = Resources.Load(house) as GameObject;
            houses.Add(houseObject);
        }

        centerElement = Resources.Load(fountain) as GameObject;

        
        foreach(var house in objects)
        {
            GameObject houseObject = Resources.Load(house) as GameObject;
            aroundFountain.Add(houseObject);
        }           
    }
    
    // Put houses from both sides of the road
    public void putHousesOnRoads(Vector2 start, Vector2 end, float widthOfRoad , float spaceBetweenBuildings1 , float spaceBetweenBuildings2, float intersection)
    {
        Vector3 direction = new Vector3(start.x, 0 , start.y) - new Vector3(end.x, 0 , end.y) ;
        Vector3 firstRow = Vector3.Cross(direction.normalized, new Vector3(0,1,0));
        Vector3 secondRow = Vector3.Cross((new Vector3(end.x, 0 , end.y) - new Vector3(start.x, 0 , start.y)).normalized, new Vector3(0,1,0));

        Vector3 startPointFirstSide = new Vector3(start.x , 0 , start.y );
        Vector3 endPointFirstSide = new Vector3(end.x , 0 , end.y );
        startPointFirstSide = startPointFirstSide + (firstRow * widthOfRoad);
        endPointFirstSide = endPointFirstSide + (firstRow * widthOfRoad);
        startPointFirstSide = Vector3.MoveTowards(startPointFirstSide, endPointFirstSide, intersection);
        endPointFirstSide = Vector3.MoveTowards(endPointFirstSide, startPointFirstSide, intersection);

        Vector3 startPointSecondSide = new Vector3(start.x , 0 , start.y );
        Vector3 endPointSecondSide = new Vector3(end.x , 0 , end.y );
        startPointSecondSide = startPointSecondSide + (secondRow * widthOfRoad);
        endPointSecondSide = endPointSecondSide + (secondRow * widthOfRoad);
        startPointSecondSide = Vector3.MoveTowards(startPointSecondSide, endPointSecondSide, intersection);
        endPointSecondSide = Vector3.MoveTowards(endPointSecondSide, startPointFirstSide, intersection);
        
        for(int j = 0 ; j < 2 ; j++)
        {
            
            float length = direction.magnitude;
            Vector3 startDraw = startPointFirstSide;
            Vector3 endDraw = endPointFirstSide;
            Vector3 rows = firstRow;
            if(j == 1)
            {
                startDraw = startPointSecondSide;
                endDraw = endPointSecondSide;
                rows = secondRow;
            }

            while(length > 0)
            {
                // Drawing first side of the road 
                int randomHouse = Random.Range(0 , houses.Count);
                float houseSize = houses[randomHouse].GetComponent<Renderer>().bounds.size.y;
                float spaceBetweenBuildings = Random.Range(spaceBetweenBuildings1 , spaceBetweenBuildings2);
                Vector3 pos = Vector3.MoveTowards(startDraw, endDraw , length - spaceBetweenBuildings);
                Quaternion rot = Quaternion.LookRotation(Vector3.up,rows);
                var houseSpawn = GameObject.Instantiate(houses[randomHouse],pos,rot);
                houseSpawn.transform.Rotate(new Vector3(0,0,-90),Space.Self);
                houseSpawn.name = numHouse.ToString();
                numHouse++;

                length -= houseSize;
            }
        }
    }

    public void putHousesInCenter(List<Vector2> cell, float rayon)
    {
        Vector2 center = Geometry.FindCenter(cell);
        Vector3 centerV3 = new Vector3(center.x + 20, 0 , center.y+20);

        Quaternion rot = Quaternion.LookRotation(Vector3.up,cell[0]);
        var centerObject = GameObject.Instantiate(centerElement,centerV3,rot);
        centerObject.name = numHouse.ToString();
        numHouse++;
        
        float angle = 0;
        
        while(angle < 360.0f)
        {
            int randIndex = Random.Range(0, aroundFountain.Count);
            GameObject building = aroundFountain[randIndex];
            float buildingSize = building.GetComponent<Renderer>().bounds.size.y;

            float randomRayon = Random.Range(rayon/2, rayon*2);
            float angleLimite = Mathf.Atan((buildingSize*2)/randomRayon) * Mathf.Rad2Deg;
            
            // Adding 10% error count into angle
            float usedAngle = ((10 * angleLimite)/100) + angleLimite;  

            rayon = (buildingSize * 2) + ((buildingSize * 2) * 30)/100;
            float x = rayon * Mathf.Cos(usedAngle * Mathf.Rad2Deg);
            float y = rayon * Mathf.Sin(usedAngle * Mathf.Rad2Deg);
            
            Vector3 pos = new Vector3(x + center.x , 0 , y + center.y);
            rot = Quaternion.LookRotation(Vector3.up,centerV3);
            var roundObject = GameObject.Instantiate(building,pos,rot);
            numHouse++;
            roundObject.name = numHouse.ToString();
            roundObject.transform.Rotate(new Vector3(0,0,-90),Space.Self);
            

            angle += usedAngle;
        }        
    }

    public void putBuildingsInCell(List<Vector2> cells , float minSpaceBetweenBuildings, float maxSpaceBetweenBuildings , bool centerActive)
    {
        Vector2 center = new Vector2(0,0);
        if(centerActive)
        {
            center =  Geometry.FindCenter(cells);
        }
        cells = Geometry.MakePolygone(cells);

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX =  float.MinValue;
        float maxY = float.MinValue;
        
        foreach(var point in cells)
        {
            // Finding borders of a rectangle
            if(point.x < minX)
            {
                minX = point.x;
            }
            if(point.y < minY)
            {
                minY = point.y;
            }
            if(point.x > maxX)
            {
                maxX = point.x;
            }
            if(point.y > maxY)
            {
                maxY = point.y;
            }
        }

        // We begin by drawing buildings from the Y axe
        float lengthY = Vector2.Distance(new Vector2(0,minY) , new Vector2(0,maxY));
        float lengthX = Vector2.Distance(new Vector2(minX,0) , new Vector2(maxX,0));

        Vector3 startPointY = new Vector3(minX, 0,maxY);
        Vector3 endPointY = new Vector3(minX,0,minY);

        Vector3 startPointX = new Vector3(minX, 0,minY);
        Vector3 endPointX = new Vector3(maxX,0,minY);
        Vector3 directionX = (endPointX - startPointX).normalized;
        Vector3 directionY = (startPointX - endPointX).normalized;
        Vector3 pos = new Vector3(0,0,0);
        Quaternion rot = Quaternion.LookRotation(Vector3.up,center);

        while(lengthX > 0)
        {
            float houseSize = 0;
            while(lengthY > 0)
            {
                int randomHouse = Random.Range(0 , houses.Count);
                houseSize = houses[randomHouse].GetComponent<Renderer>().bounds.size.y;

                float randY = Random.Range(minSpaceBetweenBuildings,maxSpaceBetweenBuildings);
                float randX = Random.Range(0,minSpaceBetweenBuildings);
                pos = Vector3.MoveTowards(startPointY, endPointY , lengthY - houseSize - randX );

                float rand = Random.Range(0,100);
                if(rand > 50)
                {
                    pos = pos + (directionX * randX);
                }
                else
                {
                    pos = pos + (directionY * randX);
                }
                if(Geometry.IsInside(cells ,new Vector2(pos.x,pos.z)))
                {
                    var houseSpawn = GameObject.Instantiate(houses[randomHouse],pos,rot);
                    houseSpawn.transform.Rotate(new Vector3(0,0,-90),Space.Self);
                    houseSpawn.name = numHouse.ToString();
                    numHouse++;
                }
                
                lengthY -= houseSize;
            }

            lengthX -= maxSpaceBetweenBuildings;
           
            startPointY = startPointY + (directionX * maxSpaceBetweenBuildings);
            endPointY = endPointY + (directionX * maxSpaceBetweenBuildings);
            lengthY = Vector2.Distance(new Vector2(0,minY) , new Vector2(0,maxY));
                      
        }

    }
    
}
}