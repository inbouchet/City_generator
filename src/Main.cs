using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private WL.WallBehavior wall;
    private PR.PrimaryRoad primaryRoad;
    private SR.SecondaryRoad secondaryRoutes;
    private SE.Cells cells;
    private HO.Houses houses;
    void Start()
    {
        int nTowers = 15;
        int citySize = 400;
        int cityTowersRandomiser = 50;

        wall = new WL.WallBehavior("castle_wall","castle_tower","gate" , nTowers , citySize , cityTowersRandomiser);
        wall.MakeWall();
        
        primaryRoad = new PR.PrimaryRoad(wall.doorPoints, wall.points , wall.cityCenter,citySize, "Sphere", "route");

        cells = new SE.Cells(primaryRoad.connections , wall.getVec2DoorPoints(), wall.points);

        string axiome = "+A-B";
        List<Dictionary<char,string>> regles = new List<Dictionary<char,string>>();
        Dictionary<char,string> regle = new Dictionary<char, string>{
            {'A',"F[--AE]F[+AE]F"},
            {'B',"F"}
        };

        Dictionary<char,string> regle1 = new Dictionary<char, string>{
            {'A',"F[++EA]F[-AE]F"},
            {'B',"F"}
        };

        Dictionary<char,string> regle2 = new Dictionary<char, string>{
            {'A',"F[--EA]F[+AE]F"},
            {'B',"F"}
        };

        regles.Add(regle);
        regles.Add(regle1);
        regles.Add(regle2);
        
        Vector3 secondaryRoadStartPosition = new Vector3(0,0,0);
        SR.SecondaryRoad secondaryRoutes = new SR.SecondaryRoad("Sphere" , "route" ,10, 90f);

        List<List<Vector2>> secondaryRoads = new List<List<Vector2>>();

        foreach(var cell in cells.cycles)
        {
            Vector2 center = Geometry.FindCenter(cell);
            Vector3 centerPosition = new Vector3(center.x, 0, center.y);

            secondaryRoutes.RoadTrapping(cell);
        
            secondaryRoads = secondaryRoads.Concat(secondaryRoutes.MakeRoad(axiome, regles, 6 , centerPosition)).ToList();
        }
        
        List<string> houseAsstes = new List<string>{
            {"house1"},
            {"civilian_house_16"}
        };

        List<string> objectAsstes = new List<string>{
            {"box"},
            {"barrel_group"},
            {"barrel"},
            {"barrel"},
            {"barrel"},
            {"pine_a"},
            {"wagon"},
            {"wagon"},
            {"wagon"},
            {"dead_tree_a"},
            {"dead_tree_a"},
            {"dead_tree_a"},
            {"street_oil_light"},
            {"street_oil_light"},
            {"street_oil_light"}
        };
        
        houses = new HO.Houses(houseAsstes,"fountain_a", objectAsstes);

        for(int i = 0 ; i < primaryRoad.connections.Count ; i++)
        {
            for(int j = 0 ; j < primaryRoad.connections[i].Count - 1 ; j++)
            {
                houses.putHousesOnRoads(primaryRoad.connections[i][j] , primaryRoad.connections[i][j+1],15f,10f,15f,10f);
            }
        }
        
        var pointPrefab = Resources.Load("Sphere") as GameObject;

        for(int i = 0 ; i < secondaryRoads.Count ; i++)
        {
            List<Vector2> listSecondaryRoad = secondaryRoads[i];
            for(int j = 0 ; j < listSecondaryRoad.Count - 1; j++)
            {
                Vector3 start = new Vector3(listSecondaryRoad[j].x,0,listSecondaryRoad[j].y);
                Vector3 end = new Vector3(listSecondaryRoad[j+1].x,0,listSecondaryRoad[j+1].y);

                houses.putHousesOnRoads(listSecondaryRoad[j],listSecondaryRoad[j+1],7f,5f,10f,0f);
            }
        }


        foreach(var c in cells.cycles)
        {
            houses.putHousesInCenter(c,40);
            houses.putBuildingsInCell(c,5f,10f,true);
        }
        houses.putBuildingsInCell(wall.points,5f,10f,false);

        var trees = new HO.Houses(objectAsstes,"fountain_a", objectAsstes,houses.numHouse);
        trees.putBuildingsInCell(wall.points,5f,10f,false);   

    }
}
