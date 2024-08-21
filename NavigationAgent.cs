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

        while (openList.Count >0){ //while theres nodes available to explore
            int currentNode = bestOpenListFScore(start,goal); //find node with lowest heuristics for A* and move to it
            if (currentNode == goal){ //reached goal
                return ReconstructPath(cameFrom, currentNode); //return reconstructed path to goal
            }

            openList.Remove(currentNode); //pop current node from open list
            closedList.Add(currentNode); //add current node to closed list

            for (int i = 0; i< graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex.Length; i++){ //for each node connected to current node
                int thisNeightbourNode = graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i]; //select neighbour node
                if (!closedList.Contains(thisNeightbourNode)){ //if neighbour node not explored before
                    float tentativeGScore = Heuristic(start, currentNode) + Heuristic(currentNode, thisNeightbourNode); //tentative Gscore

                    if (!openList.Contains(thisNeightbourNode) || tentativeGScore < gScore){ //if not in open list or has better tentative Gscore
                        openList.Add(thisNeightbourNode); //add to open list
                    }

                    if (!cameFrom.ContainsKey(thisNeightbourNode)){ //if not in camefrom
                        cameFrom.Add(thisNeightbourNode, currentNode); //add
                    }

                    gScore = tentativeGScore; //update gscore with new score
                }
            }
        }


        return null; //dead end
    }
    
    //reconstructs path to current location from start based on cameFrom
    public List<int> ReconstructPath(Dictionary<int, int> cameFrom, int current){ 
        List<int> finalPath = new List<int>();

        finalPath.Add(current);

        while (cameFrom.ContainsKey(current)){
            current = cameFrom[current];
            finalPath.Add(current);
        }
        
        finalPath.Reverse();
        
        
        return finalPath;
    }

    //find best node from open list for A*
    public int bestOpenListFScore(int start, int goal){
        int bestIndex = 0;

        //iterate through open list
        for (int i=0; i<openList.Count; i++){
            //find node with lowest combined heuristics
            if ((Heuristic(openList[i],start) + Heuristic(openList[i], goal)) < (Heuristic(openList[bestIndex], start) + Heuristic(openList[bestIndex], goal))){
                bestIndex = i;
            }
        }

        //return it
        int bestNode = openList[bestIndex];
        return bestNode;
    }

    //heuristic
    public float Heuristic(int a, int b){
        return Vector3.Distance(graphNodes.graphNodes[a].transform.position, graphNodes.graphNodes[b].transform.position);
    }

    //Greedy Search Recursive
    public List<int> GreedySearchRecursive(int currentNode, int goal, List<int> path) {
        
        //reached goal, return current path taken
        if (currentNode == goal) {
            return path;
        }
        
        //add current node to closed list
        closedList.Add(currentNode);

        //init new list of current neighbours
        List<int> neighbourList = new List<int>();

        //for each neighbour node
        for (int i = 0; i< graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex.Length; i++){
            int thisNeightbourNode = graphNodes.graphNodes[currentNode].GetComponent<LinkedNodes>().linkedNodesIndex[i]; //select neighbour node
            if (!closedList.Contains(thisNeightbourNode)) { //if neighbour not visited before

                if (!neighbourList.Contains(thisNeightbourNode)) { //add to current neighbour list if not added
                    neighbourList.Add(thisNeightbourNode);
                }

                if (!cameFrom.ContainsKey(thisNeightbourNode)) { //add to came from if not added
                    cameFrom.Add(thisNeightbourNode, currentNode);
                }
            }

        }

        while (neighbourList.Count >0){ //while theres neighbours
            int nextNode = Greedy_bestOpenListFScore(neighbourList,goal); //select best neighbour to visit and move to it
            neighbourList.Remove(nextNode); //remove neighbour from neighbourlist
            List<int> finalpath = GreedySearchRecursive(nextNode,goal, path); //recursion

            //if not dead end
            if( finalpath != null){
                finalpath.Add(currentNode); //add currentnode to final path
                return finalpath; //return final path
            }
        }     
        
        //dead end
        return null;
    }

    //find best node from open list for Greedy
    public int Greedy_bestOpenListFScore(List<int> neighbourList, int goal) {
        int bestIndex = 0;

        //iterate through neighbours
        for (int i = 0; i < neighbourList.Count; i++) {
            //find neighbour with lowest heuristic
            if (Heuristic(neighbourList[i], goal) < Heuristic(neighbourList[bestIndex], goal)) {
                bestIndex = i;
            }
        }

        //return it
        int bestNode = neighbourList[bestIndex];
        return bestNode;
    }
}
