﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyscript : MonoBehaviour
{
    public collectible c;
    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<collectible>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
            if (other.gameObject.tag.Equals("Player") && c.active)
            {

                Talk.id = 5;
                CharacterControllerScript.key = true;
                Debug.Log("collectible");
                //Destroy(gameObject);
                c.active = false;
            }

        
    }
}
