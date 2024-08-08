using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class NavigationAgent : MonoBehaviour {

    //Navigation Variables
    public WaypointGraph graphNodes;
    public List<int> openList = new List<int>();
    public List<int> closedList = new List<int>();
    public List<int> currentPath = new List<int>();
    public List<int> greedyPaintList = new List<int>();
    public int currentPathIndex = 0;
    public int currentNodeIndex = 0;

    public Dictionary<int,int> cameFrom = new Dictionary<int, int>();


    // Use this for initialization
    void Start () {
        //Find waypoint graph
        graphNodes = GameObject.FindGameObjectWithTag("waypoint graph").GetComponent<WaypointGraph>();

        //Initial node index to move to
        currentPath.Add(currentNodeIndex);
    }

    //A-Star Search
    public List<int> AStarSearch(int start, int goal) {
        
        //Code here
        
        openList.Clear();
        closedList.Clear();
        cameFrom.Clear();

        //Begin
        openList.Add(start);
        float gScore = 0;
        float fScore = gScore + Heuristic(start,goal);

        while (openList.Count >0){
            int currentNode = bestOpenListFScore(start,goal);
            if (currentNode == goal){
                
                return ReconstructPath(cameFrom, currentNode);
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            for (int i = 0; i< graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex.Length; i++){
                int thisNeightbourNode = graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i];
                if (!closedList.Contains(thisNeightbourNode)){
                    float tentativeGScore = Heuristic(start, currentNode) + Heuristic(currentNode, thisNeightbourNode);

                    if (!openList.Contains(thisNeightbourNode) || tentativeGScore < gScore){
                        openList.Add(thisNeightbourNode); 
                    }

                    if (!cameFrom.ContainsKey(thisNeightbourNode)){
                        cameFrom.Add(thisNeightbourNode, currentNode);
                    }

                    gScore = tentativeGScore;
                    fScore = Heuristic(start, thisNeightbourNode) + Heuristic(thisNeightbourNode, goal);
                }
            }
        }


        return null;
    }

    public List<int> ReconstructPath(Dictionary<int, int> CF, int current){
        List<int> finalPath = new List<int>();

        finalPath.Add(current);

        while (CF.ContainsKey(current)){
            current = CF[current];
            finalPath.Add(current);
        }

        finalPath.Reverse();
        return finalPath;
    }

    public int bestOpenListFScore(int start, int goal){
        int bestIndex = 0;

        for (int i=0; i<openList.Count; i++){
            if ((Heuristic(openList[i],start) + Heuristic(openList[i], goal)) < (Heuristic(openList[bestIndex], start) + Heuristic(openList[bestIndex], goal))){
                bestIndex = i;
            }
        }

        int bestNode = openList[bestIndex];
        return bestNode;
    }

    public float Heuristic(int a, int b){
        return Vector3.Distance(graphNodes.graphNodes[a].transform.position, graphNodes.graphNodes[b].transform.position);
    }

    //Greedy Search
    public List<int> GreedySearch(int start, int goal) {

        //Code here

        openList.Clear();
        closedList.Clear();
        cameFrom.Clear();

        //Begin
        openList.Add(start);

        while (openList.Count >0){;
            int currentNode = bestOpenListFScore(start,goal);

            if (currentNode == goal){               
                return ReconstructPath(cameFrom, currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            for (int i = 0; i< graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex.Length; i++){
                int thisNeightbourNode = graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i];
                if (!closedList.Contains(thisNeightbourNode)){

                    if (!openList.Contains(thisNeightbourNode)) {
                    openList.Add(thisNeightbourNode);
                    }

                    if (!cameFrom.ContainsKey(thisNeightbourNode)) {
                    cameFrom.Add(thisNeightbourNode, currentNode);
                    }

                }
            }

        }
        return null;
        
    }
}
