﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LootDrop : MonoBehaviour
{
    public GameObject lootObject;
    public VisualEffect lootVFX;
    public int experienceAmount = 10;

    void Start()
    {
        lootVFX.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            lootVFX.Stop();
            lootObject.SetActive(false);


            PlayerExperience playerExperience = other.GetComponent<PlayerExperience>();
            if (playerExperience != null)
            {
                playerExperience.GainExperience(experienceAmount);
                // Optionally play a sound or particle effect here

                // Destroy the orb after it's collected
                Destroy(gameObject);
            }
        }

    }
}
