using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class StateInitializer : MonoBehaviour
{
    public String initalState;
    // Start is called before the first frame update
    void Start()
    {
        if (initalState != null) {
            Type stateClass = Type.GetType(initalState);
            GetComponent<StateMachine>().ChangeState((State)Activator.CreateInstance(stateClass));   
        }  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
