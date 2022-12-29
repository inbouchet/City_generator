using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WL
{
public class WallBehavior
{
    private GameObject cube;
    private GameObject wall;
    private GameObject door;
    private GameObject tower;
    public List<Vector2> points;
    public List<Vector3> doorPoints;
    private Wall wallObject;
    public Vector2 cityCenter;

    public WallBehavior(string wallName, string towerName, string doorName , int nTour, int radius, int smallRadius)
    {
        doorPoints = new List<Vector3>();
        wall = Resources.Load(wallName) as GameObject;
        tower = Resources.Load(towerName) as GameObject;
        door = Resources.Load(doorName) as GameObject;

        wallObject = new Wall(nTour,radius,smallRadius);
        points = wallObject.createPoints();
    }

    //Last function to call to draw the wall
    public void MakeWall()
    {
        List<Vector3> walls = new List<Vector3>();
        walls = wallObject.createWalls(points);

        DrawWall(walls);
    }

    public List<Vector2> getVec2DoorPoints()
    {
        List<Vector2> vec2DoorPoints = new List<Vector2>();
        foreach(var p in doorPoints)
        {
            vec2DoorPoints.Add(new Vector2(p.x,p.z));
        }
        return vec2DoorPoints;
    }

    // Move all the points of the wall to a different location in the radius of the variation
    public void MoveCenterWallRandom(float variation)
    {
        List<Vector2> movedPoints = new List<Vector2>();
        points = Geometry.MovePointsRandom(points, variation);
    }

    public void MoveCenterWall(Vector2 newCenter)
    {  
        this.cityCenter = newCenter;

        List<Vector2> movedPoints = new List<Vector2>();
        points = Geometry.MovePoints(points, newCenter);
    }
    
    void DrawWall(List<Vector3> walls)
    {
        for(int i = 0 ; i < walls.Count ; i++)
        {
            Vector3 pA = walls[i];
            Vector3 pB;
            if(i == (walls.Count - 1))
            {
                pB = walls[0];
            }else{
                pB = walls[i+1];
            }

            Vector3 direction = pB - pA;
            float distance = direction.magnitude;
            float constDistance = distance;
            Vector3 tolookat = Vector3.Cross(direction , new Vector3(0,1,0));
            Vector3 rotation = new Vector3(wall.transform.localEulerAngles.x 
                                            ,wall.transform.localEulerAngles.y , wall.transform.localEulerAngles.z );

            var tourelle = GameObject.Instantiate(tower, walls[i] , tower.transform.rotation);
            tourelle.name = "wall";
            
            // Calibration des tours manuelles
            tourelle.transform.Translate(Vector3.right * 7 , Space.Self);

            Vector3 wallbounds = wall.GetComponent<Renderer>().bounds.size;
            Vector3 doorBounds = door.GetComponentInChildren<Renderer>().bounds.size;
            float sizeWall = wallbounds.x;
            float sizeDoor = doorBounds.x;

            GameObject spawnedWall = wall;
            int numberOfTimeDoor = 0;
            while(distance >= sizeWall)
            {
                bool spawn = true;
                if((distance > sizeWall * 3) && (i%2)==0)
                {
                    if((distance > ((constDistance/2) - (sizeDoor/2))) && (distance < ((constDistance/2) + (sizeDoor/2))))
                    {
                        if(numberOfTimeDoor>=1)
                        {
                            spawn = false;
                        }
                        else
                        {
                            spawnedWall = door;
                            sizeWall = sizeDoor;
                            
                            // Store doors positions
                            doorPoints.Add(new Vector3((pB.x + pA.x)/2 , (pB.y + pA.y)/2 , (pB.z + pA.z)/2));
                        }
                        numberOfTimeDoor++;
                    }
                }
                
                if(spawn)
                {
                    Vector3 spawnplace = new Vector3(walls[i].x ,walls[i].y ,walls[i].z );
                    var wallScene = GameObject.Instantiate(spawnedWall, walls[i] , spawnedWall.transform.rotation);
                    wallScene.transform.position = Vector3.MoveTowards(pA, pB , (sizeWall / 2));    
                    wallScene.name = "wall";
                    wallScene.transform.rotation = Quaternion.LookRotation(tolookat);
                    wallScene.transform.Rotate(rotation);
                    wallScene.transform.Translate(Vector3.right * distance,Space.Self);
                }
                
                distance -= sizeWall;

                spawnedWall = wall;
                sizeWall = wallbounds.x;
            }
            
        }
    }

}
}