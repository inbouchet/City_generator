using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PR
{
public class PrimaryRoad
{
    private GameObject sphere;
    private GameObject road;
    private List<Vector2> wallTowers;

    private List<Vector3> doorPositions;
    private Vector3 circlCenter;
    public List<List<Vector2>> connections;
    int citySize;

    public PrimaryRoad(List<Vector3> doorPositions , List<Vector2> wallTowers , Vector2 circlCenter ,int citySize , string sphereName, string roadName)
    {
        this.sphere = Resources.Load(sphereName) as GameObject;
        this.road = Resources.Load(roadName) as GameObject;

        this.doorPositions = doorPositions;
        this.circlCenter = new Vector3(circlCenter.x , 0 , circlCenter.y);
        this.citySize = citySize;
        this.wallTowers = wallTowers;

        List<Vector2> intersection = IntersectionPoints();
        connections = DoorIntersectionConnection(intersection);
        
        DrawRoad(connections);
    } 
    
    public List<Vector2> IntersectionPoints()
    {
        // Random intersection point creation
        int numberOfPoints = (int)(doorPositions.Count / 2);
        
        List<Vector2> intersectionPoints = new List<Vector2>();
        
        // We reduce the radius of 30%
        int error = (35 * citySize)/100;
        int radius = citySize - error;
        
        for(int i = 0 ; i < numberOfPoints ; i++)
        {

            float anlgeChange = 90.0f;
            float angleStart = 0.0f;
            float angleEnd = anlgeChange;
            Vector2 point;

            do{
                float randomAngle = Random.Range(angleStart, angleEnd);
                float x = radius * Mathf.Cos(randomAngle * Mathf.Rad2Deg);
                float y = radius * Mathf.Sin(randomAngle * Mathf.Rad2Deg);
                point = new Vector2(circlCenter.x + x , circlCenter.y +  y);
            }
            while (!Geometry.IsInside(wallTowers, point) 
                    && Geometry.pointsTooClose(intersectionPoints,point,radius/2));

            Vector3 pointInSpace = new Vector3(point.x , 0 , point.y);

            intersectionPoints.Add(point);

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
        
        return intersectionPoints;
    }

    public List<List<Vector2>> DoorIntersectionConnection(List<Vector2> intersectionPoints)
    {
        List<List<Vector2>> data = new List<List<Vector2>>();
        List<Vector2> centerPoints = new List<Vector2>();

        // Making intersection point in the first row which will be useful
        for(int i = 0 ; i < intersectionPoints.Count ; i++)
        {
            centerPoints.Add(intersectionPoints[i]);
            if(i == intersectionPoints.Count - 1)
            {
                centerPoints.Add(intersectionPoints[0]);
            }
        }

        data.Add(centerPoints);

        foreach(Vector2 p in intersectionPoints)
        {
            Dictionary<float,Vector2> distances = new Dictionary<float,Vector2>();
            foreach(Vector3 pDoor in doorPositions)
            {
                distances.Add(Vector2.Distance(p,new Vector2(pDoor.x , pDoor.z)),new Vector2(pDoor.x , pDoor.z));
            }

            float maxDistance1 = float.MaxValue;
            float maxDistance2 = float.MaxValue;
            Vector2 selectedDoor1 = new Vector2(0,0); 
            Vector2 selectedDoor2 = new Vector2(0,0); 

            foreach(var d in distances)
            {
                if(d.Key < maxDistance1){
                    maxDistance2 = maxDistance1;
                    selectedDoor2 = selectedDoor1;
                    selectedDoor1 = d.Value;
                    maxDistance1 = d.Key;
                }
                else if(d.Key < maxDistance2){
                    selectedDoor2 = d.Value;
                    maxDistance2 = d.Key;
                }
            }

            List<Vector2> connectionPoints = new List<Vector2>();
            connectionPoints.Add(selectedDoor1);
            connectionPoints.Add(p);
            connectionPoints.Add(selectedDoor2);
            
            data.Add(connectionPoints);
        }

        //Adding doors that are not connected to random intersection points
        foreach(Vector3 pDoor in doorPositions)
        {
            Vector2 v2PDoor = new Vector2(pDoor.x,pDoor.z);
            bool exists = false;
            foreach(var listConnexion in data)
            {
                if(listConnexion.Contains(v2PDoor))
                {
                    exists = true;
                }
            }

            if(!exists)
            {
                List<Vector2> connectionPoints = new List<Vector2>();
                connectionPoints.Add(v2PDoor);
                
                int index = Random.Range(0,intersectionPoints.Count);
                connectionPoints.Add(intersectionPoints[index]);
                data.Add(connectionPoints);
            }
            
        }

        return data;
    }

    // Return a list of a list of each two point drawing a road
    // public List<List<Vector2>> getPrimaryRoadDoublePoints 

    public void DrawRoad(List<List<Vector2>> connections)
    {
        foreach(var c in connections)
        {
            for (int i = 0 ; i < c.Count - 1; i++)
            {
                float distances = Vector2.Distance(c[i], c[i+1]);
                Vector3 direction = ( new Vector3(c[i+1].x,0,c[i+1].y) - new Vector3(c[i].x,0,c[i].y)).normalized;
                Vector3 startPosition = new Vector3(c[i].x,0,c[i].y);
                
                var endPostition = startPosition + (direction*distances);
                var spawnPosition = (startPosition + endPostition)/2;
                
                var roadObject = GameObject.Instantiate(road, spawnPosition, Quaternion.identity);
                roadObject.name = "road";
                
                roadObject.transform.rotation = Quaternion.LookRotation(direction,new Vector3(0,1,0));
                roadObject.gameObject.transform.localScale += new Vector3(0,0,distances);
            }
        }        
    }
}
}
