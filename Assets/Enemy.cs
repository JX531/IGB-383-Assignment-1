using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;


public class Enemy : NavigationAgent {

    //Player Reference
    Player player;
    int roam_goal = -1;
    System.Random random;
    //Movement Variables
    public float moveSpeed = 10.0f;
    public float minDistance = 0.1f;

    //FSM Variables
    public int newState = 0;
    private int currentState = 0;
    private int hideIndex = 25;


    // Use this for initialization
    void Start() {
        random = new System.Random(GetInstanceID());
        //Find waypoint graph
        graphNodes = GameObject.FindGameObjectWithTag("waypoint graph").GetComponent<WaypointGraph>();
        //Initial node index to move to
        currentPath.Add(currentNodeIndex);
        //Establish reference to player game object
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update () {

        Move();
        switch(newState){
            //roam
            case 0:
                Roam();
                break;

            //hide
            case 1:
                Hide();
                break;

            //attack
            case 2:
                Attack();
                break;
        }
    }

    //Move Enemy
    private void Move() {

        if (currentPath.Count > 0) {

            //Move towards next node in path
            transform.position = Vector3.MoveTowards(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position, moveSpeed * Time.deltaTime);

            //Increase path index
            if (Vector3.Distance(transform.position, graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position) <= minDistance) {

                if (currentPathIndex < currentPath.Count - 1)
                    currentPathIndex++;
            }

            currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>().index;   //Store current node index
        }
    }

    //FSM Behaviour - Roam - Randomly select nodes to travel to using Greedy Search Algorithm
    private void Roam() {
        print(roam_goal);
        
        if (currentPath[currentPathIndex] == roam_goal || roam_goal == -1){
            roam_goal = random.Next(0,graphNodes.graphNodes.Count());
        }

        if (currentPath[currentPath.Count -1] != roam_goal){
            currentPath = GreedySearch(currentPath[currentPathIndex], roam_goal);
            currentPathIndex = 0;
        }

        
    }
    
    //FSM Behaviour - Move towards hide location using A* Search Algorithm
    private void Hide() {
        print("Hiding");
        if (Vector3.Distance(transform.position, graphNodes.graphNodes[hideIndex].transform.position) > minDistance && currentPath[currentPath.Count -1] != hideIndex){
            currentPath = AStarSearch(currentPath[currentPathIndex], hideIndex);
            currentPathIndex = 0;
        }
    }

    //FSM Behaviour - Move towards node closest to player using A* Search Algorithm
    private void Attack() {
        print("Attacking");
        if (Vector3.Distance(transform.position, graphNodes.graphNodes[player.currentNodeIndex].transform.position) > minDistance && currentPath[currentPath.Count -1] != player.currentNodeIndex){
            currentPath = AStarSearch(currentPath[currentPathIndex], player.currentNodeIndex);
            currentPathIndex = 0;
        }
    }
}
