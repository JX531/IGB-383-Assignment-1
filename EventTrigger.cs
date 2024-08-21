using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{   
    public enum TriggerState{
        Patrol, Hide, Attack
    }

    public TriggerState enter;
    public TriggerState exit;
    private static bool isCoroutine = false; //To ensure only 1 coroutine runs at a time

    private static int interactions = 0; //Keeps track of how many NPCs are currently seeing player

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "Player"){ //An NPC sees player
            interactions++; //increase whenever player enters vision of any NPC
            if (self.name != "Dog"){ //Not Dog, so signals 1 (seen by self)
                signal(0);
            }
            else{
                signal(1); //Seen by Dog, so signals 2 (seen by Dog), which alerts all NPCs
            }
        }
    }

    void OnTriggerExit(Collider other){
        if (other.tag == "Player"){//An NPC lost vision of player
            interactions--; //decrease whenever player leaves vision of any NPC
            if (!isCoroutine && interactions <= 0){ //coroutine currently not running and no NPCs see player
                StartCoroutine(Delayed_LoseAggro(4)); //start counter to make NPCs lose aggro
            }
        }
    }

    private IEnumerator Delayed_LoseAggro(int delay){ //NPC lose aggro after delay seconds
        isCoroutine = true; //coroutine currently running
        yield return new WaitForSeconds(delay); //wait for delay seconds
        if (interactions <= 0){ //double check if any NPCs currently see player
            signal(2); //signal 2 to make all enemies lose aggro
        }
        isCoroutine = false; //coroutine ends
    }

    public Enemy[] enemies; //list of all enemies
    public Enemy self; //self

    //All enemies are affected when : Signal is from Dog, otherwise only affects self OR Signal = 2
    private void signal(int signal){
        if (self.name != "Dog" && signal != 2){ // if the signal is not from Dog and signal is not lost vision of player
            self.newState = self.table[self.currentState,signal]; //modify self state based on table
            //print(self.newState);
            self.currentState = self.newState;
        }
        else{
            foreach(Enemy enemy in enemies){ // signal is from Dog OR signal = 2
                enemy.newState = enemy.table[enemy.currentState,signal]; //all enemies are affected
                enemy.currentState = enemy.newState;
            }
        }
    }

    
}
