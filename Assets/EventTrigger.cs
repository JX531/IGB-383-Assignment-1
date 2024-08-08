﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{   
    public enum TriggerState{
        Patrol, Hide, Attack
    }

    public TriggerState enter;
    public TriggerState exit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "Player"){
            switch(enter){
                case TriggerState.Patrol:
                ChangeEnemyStates(0);
                break;

                case TriggerState.Hide:
                ChangeEnemyStates(1);
                break;

                case TriggerState.Attack:
                ChangeEnemyStates(2);
                break;
            }
        }
    }

    void OnTriggerExit(Collider other){
        if (other.tag == "Player"){
            switch (exit){
                case TriggerState.Patrol:
                ChangeEnemyStates(0);
                break;

                case TriggerState.Hide:
                ChangeEnemyStates(1);
                break;

                case TriggerState.Attack:
                ChangeEnemyStates(2);
                break;
            }
        }
    }

    public Enemy[] enemies;

    private void ChangeEnemyStates(int state){
        foreach(Enemy enemy in enemies){
            enemy.newState = state;
        }
    }

    
}
