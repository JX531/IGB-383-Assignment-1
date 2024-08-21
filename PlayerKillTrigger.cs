using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKillTrigger : MonoBehaviour
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

    public GameObject food;
    void OnTriggerEnter(Collider other){
        if (other.tag == "Food"){
            //end game
            food.SetActive(false);
        }
    }
}