using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillTrigger : MonoBehaviour
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

    public GameObject player;
    void OnTriggerEnter(Collider other){
        if (other.tag == "Player"){
            //end game
            player.SetActive(false); //Kills player and ends game
            print("Game Over");
        }
    }
}