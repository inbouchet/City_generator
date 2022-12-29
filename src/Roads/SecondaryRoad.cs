using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SR
{
public class SecondaryRoad
{
    private GameObject pointPrefab;
    private GameObject road;
    private LSystem lSystem;
    private string axiome;
    private Dictionary<char,string> regles;
    private List<Vector2> pointTrapping;
    bool roadtrapping = false;

    private float lengthMax, angleVariance;

    public SecondaryRoad(string pointPrefabName, string roadName, float lengthMax, float angleVariance)
    {
        pointPrefab = Resources.Load(pointPrefabName) as GameObject;
        road = Resources.Load(roadName) as GameObject;

        this.angleVariance = angleVariance;
        this.lengthMax = lengthMax;
    }

    // Sets the Lsystem parameters and create the roads
    public List<List<Vector2>> MakeRoad(string axiome,List<Dictionary<char,string>> regles, int nLsystemRecursion, Vector3 startPosition)
    {
        lSystem = new LSystem(regles,axiome,nLsystemRecursion);
        string generationLsystem = lSystem.Generate();
        List<List<Vector2>> result = RoadCreation(generationLsystem ,startPosition);
        roadtrapping = false;
        return result;
    }

    public void RoadTrapping(List<Vector2> points)
    {
        roadtrapping = true;
        
        Vector2 center = Geometry.FindCenter(points);

        pointTrapping = Geometry.SortAdjListToRight(points , center);
    }

    private List<List<Vector2>> RoadCreation(string sequence , Vector3 startPosition)
    {
        Stack<routePosition> savePoints = new Stack<routePosition>();
        List<List<Vector2>> positions = new List<List<Vector2>>();
        Vector3 position = startPosition;

        Vector3 direction = Vector3.forward;
        
        float length;
        float angle;

        foreach(var letter in sequence)
        {
            length = Random.Range(10,lengthMax) * 5;
            angle = Random.Range(angleVariance,angleVariance);
            Grammaire fonction = (Grammaire)letter;
            switch (fonction)
            {
                case Grammaire.save:
                    savePoints.Push(new routePosition(position,direction,length));
                    break;
                case Grammaire.load:
                    if (savePoints.Count > 0)
                    {
                        routePosition tmp = savePoints.Pop();
                        direction = tmp.direction;
                        length = tmp.length;
                        position = tmp.position;
                    }
                    else
                    {
                        Debug.Log("no more saved point in stack, change axiome or rule !! ;)\n");
                    }
                    break;
                case Grammaire.draw:
                    startPosition = position;
                    position += direction * length;

                    Vector2 start = new Vector2(startPosition.x , startPosition.z);
                    Vector2 end = new Vector2(position.x , position.z);

                    List<Vector2> pairConnection = new List<Vector2>{start,end};
                    

                    // Does not draw road outside pointTraping List
                    if(roadtrapping)
                    {
                        
                        if (Geometry.IsPointInPolygon(start, pointTrapping.ToArray()) && Geometry.IsPointInPolygon(end, pointTrapping.ToArray()))
                        {
                            positions.Add(pairConnection);
                            drawRoad(startPosition, direction, length);
                        }
                    }
                    else
                    {
                        positions.Add(pairConnection);
                        drawRoad(startPosition, direction, length);
                    }                
                    
                    break;
                case Grammaire.turnRight:
                    direction = Quaternion.AngleAxis(angle , Vector3.up) * direction;
                    break;
                case Grammaire.turnLeft:
                    direction = Quaternion.AngleAxis(-angle , Vector3.up) * direction;
                    break;
                default:
                    break;
            }
        }

        positions = positions.Distinct().ToList();
        return positions;
    }

    void drawRoad(Vector3 start , Vector3 direction, float length)
    {
        var endPostition = start + (direction*length);
        var spawnPosition = (start + endPostition)/2;
        
        var roadObject = GameObject.Instantiate(road, spawnPosition, Quaternion.identity);
        roadObject.name = "road";
        
        roadObject.transform.rotation = Quaternion.LookRotation(direction,new Vector3(0,1,0));
        roadObject.gameObject.transform.localScale += new Vector3(0,0,length);
    }

}
}