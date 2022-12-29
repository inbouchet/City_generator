using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SE
{
public class Cells
{
    private List<List<Vector2>> primaryRoad; 
    private List<Vector2> borderPoints;
    public List<List<Vector2>> cycles;
    public Cells(List<List<Vector2>> primaryRoad, List<Vector2> doorPositions, List<Vector2> towerPositions)
    {
        this.primaryRoad = primaryRoad;

        towerPositions.RemoveAt(towerPositions.Count - 1);
        doorPositions.AddRange(towerPositions);
        this.borderPoints = Geometry.MakePolygone(doorPositions);

        Dictionary<Vector2,List<Vector2>> adjList = toAdjacentList();

        cycles = Cycle(adjList);        
    } 

    public Dictionary<Vector2,List<Vector2>> toAdjacentList()
    {
        // Create a dictionary for every two points that forms a road.
        List<List<Vector2>> roads = new List<List<Vector2>>();

        foreach(var r in primaryRoad)
        {
            for(int i = 0 ; i < r.Count - 1; i++)
            {
                List<Vector2> tmp = new List<Vector2>{r[i],r[i+1]};
                roads.Add(tmp);        
            }
        }

        //Connecting doors points together, i = 1 cause no door in first row
        for(int i = 0 ; i < borderPoints.Count ; i++)
        {
            if(i == borderPoints.Count - 1)
            {
                List<Vector2> doorConnexion = new List<Vector2>{borderPoints[i],borderPoints[0]};
                roads.Add(doorConnexion);
            }
            else
            {   
                List<Vector2> doorConnexion = new List<Vector2>{borderPoints[i],borderPoints[i+1]};
                roads.Add(doorConnexion);
            }
        }

        // Create an Adjacence List
        Dictionary<Vector2,List<Vector2>> adjacenceList = new Dictionary<Vector2,List<Vector2>>();
        foreach(var r in roads)
        {
            if(!adjacenceList.ContainsKey(r[0]))
            {
                adjacenceList.Add(r[0], new List<Vector2>{r[1]});
            }
            else
            {
                adjacenceList[r[0]].Add(r[1]);
            }

            if(!adjacenceList.ContainsKey(r[1]))
            {
                adjacenceList.Add(r[1], new List<Vector2>{r[0]});
            }
            else
            {
                adjacenceList[r[1]].Add(r[0]);
            }

        }

        foreach(var list in adjacenceList)
        {
            List<Vector2> correctedOrder = Geometry.SortAdjListToRight(list.Value,list.Key);
            list.Value.Clear();
            list.Value.AddRange(correctedOrder);
        }

        return adjacenceList;        
    }

    public List<List<Vector2>> getCyclesBruteForce(Dictionary<Vector2,List<Vector2>> adjList)
    {
        List<List<Vector2>> Cycles = new List<List<Vector2>>();
        
        foreach(var list in adjList)
        {
            foreach(var nextPoint in list.Value)
            {
                Stack<Vector2> stack = new Stack<Vector2>();
                List<Stack<Vector2>> result = new List<Stack<Vector2>>();
                stack.Push(list.Key);
                stack.Push(nextPoint);
                result = recCycle(adjList,result,stack,list.Key,nextPoint,nextPoint,0);

                // Take the minimum cycle
                int min = int.MaxValue;
                List<Vector2> minPoint = new List<Vector2>();
                foreach(var r in result)
                {
                    if(r.Count < min)
                    {
                        min = r.Count;
                        minPoint = new List<Vector2>(r);
                    }
                }

                if(minPoint.Count > 0)
                {
                    Cycles.Add(minPoint);
                }
            }   
        }

        // Delete repeated cycles
        for(int k = 0 ; k < 2 ; k++)
        {
            for(int i = 0 ; i < Cycles.Count - 1 ; i++)
            {
                for(int j = i+1 ; j < Cycles.Count ; j++)
                {
                    bool delete = true;
                    foreach(var point in Cycles[i])
                    {
                        if(!Cycles[j].Contains(point))
                        {
                            delete = false;
                        }
                    }
                    if(delete)
                    {
                        Cycles.Remove(Cycles[j]);
                    }
                }
            }
        }

        return Cycles;
    }

    // Recursive function that gets all cycles with a start and the second point we want to go to.
    public List<Stack<Vector2>> recCycle(Dictionary<Vector2,List<Vector2>> adjList,List<Stack<Vector2>> resultat, Stack<Vector2> cycle, Vector2 startPoint, Vector2 secondPoint, Vector2 nextPoint, int maxRec)
    {
        List<Stack<Vector2>> test = new List<Stack<Vector2>>();
        foreach(var e in adjList[nextPoint])
        {
            if(startPoint == e && secondPoint != nextPoint)
            {
                resultat.Add(new Stack<Vector2>(cycle));
                return resultat;
            }
            
            if(maxRec < 4 && !cycle.Contains(e))
            {
                cycle.Push(e);
                test = recCycle(adjList,resultat,cycle,startPoint,secondPoint,e,maxRec++);
                cycle.Pop();
            }
        }
        return test;
    }

    public List<List<Vector2>> Cycle(Dictionary<Vector2,List<Vector2>> adjList)
    {
        List<List<Vector2>> cycles = new List<List<Vector2>>();

        foreach(var list in adjList)
        {
            foreach(var nextPoint in list.Value)
            {
                Stack<Vector2> stack = new Stack<Vector2>();
                List<Stack<Vector2>> result = new List<Stack<Vector2>>();
                stack.Push(list.Key);
                stack.Push(nextPoint);
                result = recCycle(adjList,result,stack,list.Key,nextPoint,nextPoint,0);

                // Take the minimum cycle
                int min = int.MaxValue;
                List<Vector2> minPoint = new List<Vector2>();
                foreach(var r in result)
                {
                    if(r.Count < min)
                    {
                        min = r.Count;
                        minPoint = new List<Vector2>(r);
                    }
                }

                if(minPoint.Count > 0)
                {
                    cycles.Add(minPoint);
                }
            }   
        }


        // Delete repeated cycles
        for(int k = 0 ; k < 2 ; k++)
        {
            for(int i = 0 ; i < cycles.Count - 1 ; i++)
            {
                for(int j = i+1 ; j < cycles.Count ; j++)
                {
                    bool delete = true;
                    foreach(var point in cycles[i])
                    {
                        if(!cycles[j].Contains(point))
                        {
                            delete = false;
                        }
                    }
                    if(delete)
                    {
                        cycles.Remove(cycles[j]);
                    }
                }
            }
        }

        return cycles;
    }

    public List<Vector2> getCycle(Dictionary<Vector2,List<Vector2>> adjList, Vector2 startPoint, Vector2 nextPoint)
    {
        bool found = false;
        List<Vector2> visitedPoints = new List<Vector2>();
        Vector2 firstPoint = startPoint;
        visitedPoints.Add(startPoint);
        int nbIteration = 0;
    
        while(!found)
        {
            visitedPoints.Add(nextPoint);

            for(int i = 0 ; i < adjList[nextPoint].Count ; i++)
            {
                if(adjList[nextPoint].Contains(firstPoint) && nbIteration != 0)
                {
                    visitedPoints.Add(firstPoint);
                    found = true;
                    break;
                }

                if(adjList[nextPoint][i] == startPoint)
                {
                    int j = i;

                    if(j == 0)
                    {
                        j = adjList[nextPoint].Count - 1;
                    }
                    else
                    {
                        j--;
                    }
                
                    startPoint = nextPoint;
                    nextPoint = adjList[nextPoint][j];
                
                    break;
                }
            }
            
            nbIteration++;
        }

        return visitedPoints;
    }

}
}