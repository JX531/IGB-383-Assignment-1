using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;


public class Enemy : NavigationAgent {

    //Player Reference
    Player player;
    public int roam_goal = -1; // target location to roam to
    System.Random random;
    //Movement Variables
    private float moveSpeed = 10.0f;
    private float rotationSpeed = 10.0f;
    public float minDistance = 0.1f;

    //FSM Variables
    public int newState = -1;
    public int currentState = 0;
    private int hideIndex;

    public int[,] table; // FSM table

    // Use this for initialization
    void Start() {
        random = new System.Random(GetInstanceID());
        //Find waypoint graph
        graphNodes = GameObject.FindGameObjectWithTag("waypoint graph").GetComponent<WaypointGraph>();
        //Initial node index to move to
        currentPath.Add(currentNodeIndex);
        //Establish reference to player game object
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //FSM and initial values
        //0 = roam, 1 = hide, 2 = attack
        if (gameObject.name == "Guard"){
            table = new int[,] {{2,2,0}, //Currently roaming and player seen by self / player seen by Dog / player not seen
                                {2,2,0}, //Currently hiding and ...
                                {2,2,0}};//Currently attacking and ...
        }

        if (gameObject.name == "Sentry"){
            currentState = 1;
            hideIndex = 3;
            table = new int[,] {{2,2,1}, //Currently roaming and player seen by self / player seen by Dog / player not seen
                                {2,2,1}, //Currently hiding and ...
                                {2,2,1}};//Currently attacking and ...
        }

        if (gameObject.name == "Target"){
            hideIndex = 54;
            table = new int[,] {{1,1,0}, //Currently roaming and player seen by self / player seen by Dog / player not seen
                                {1,1,0}, //Currently hiding and ...
                                {1,1,0}};//Currently attacking and ...
        }

        if (gameObject.name == "Dog"){
            currentState = 1;
            hideIndex = 17;
            table = new int[,] {{0,0,1}, //Currently roaming and player seen by self / player seen by Dog / player not seen
                                {0,0,1}, //Currently hiding and ...
                                {0,0,1}};//Currently attacking and ...
        }
        
        
    }

    // Update is called once per frame
    void Update () {

        Move();
        switch(currentState){
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

        //get the position of the next node
        Vector3 targetPosition = graphNodes.graphNodes[currentPath[currentPathIndex]].transform.position;

        //get direction
        Vector3 direction = (targetPosition - transform.position).normalized;

        //rotate
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        //move
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Increase path index when the enemy reaches the node
        if (Vector3.Distance(transform.position, targetPosition) <= minDistance) {
            if (currentPathIndex < currentPath.Count - 1)
                currentPathIndex++;
        }

        // Store current node index
        currentNodeIndex = graphNodes.graphNodes[currentPath[currentPathIndex]].GetComponent<LinkedNodes>().index;
    }
}

    //FSM Behaviour - Roam - Randomly select nodes to travel to using Greedy Search Algorithm
    private void Roam() {
        
        if (currentPath[currentPathIndex] == roam_goal || roam_goal == -1){ // select a new roam goal
            roam_goal = random.Next(0,graphNodes.graphNodes.Count());
        }

        if (currentPath[currentPath.Count -1] != roam_goal){ //if not done roaming to roam goal
            currentPath.Clear(); //reset all pathing
            closedList.Clear();
            currentPathIndex = 0;

            currentPath.Add(currentNodeIndex); //add current location and roam goal to current path
            currentPath.Add(roam_goal);
            
            //Greedy Search
            currentPath = GreedySearchRecursive(currentPath[currentPathIndex], roam_goal, currentPath); 

            //Reverse path and remove final (i.e. initial) position
            currentPath.Reverse();
			currentPath.RemoveAt (currentPath.Count-1);
        }
        moveSpeed = 10;
        
    }
    
    //FSM Behaviour - Move towards hide location using A* Search Algorithm
    private void Hide() {
        //print("Hiding");
        if (Vector3.Distance(transform.position, graphNodes.graphNodes[hideIndex].transform.position) > minDistance && currentPath[currentPath.Count -1] != hideIndex){
            currentPath = AStarSearch(currentPath[currentPathIndex], hideIndex);
            currentPathIndex = 0;
        }
        moveSpeed = 15;
    }

    //FSM Behaviour - Move towards node closest to player using A* Search Algorithm
    private void Attack() {
        //print("Attacking");
        if (Vector3.Distance(transform.position, graphNodes.graphNodes[player.currentNodeIndex].transform.position) > minDistance && currentPath[currentPath.Count -1] != player.currentNodeIndex){
            currentPath = AStarSearch(currentPath[currentPathIndex], player.currentNodeIndex);
            currentPathIndex = 0;
        }
        moveSpeed = 15;
    }
}
