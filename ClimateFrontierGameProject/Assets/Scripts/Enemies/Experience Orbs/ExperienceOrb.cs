using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ExperienceOrb : MonoBehaviour
{
    public GameObject lootObject;
    public VisualEffect lootVFX;
    public int experienceAmount = 10;

    void OnEnable()
    {
        lootVFX.Play();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the player's experience component and add experience
            PlayerExperience playerExperience = other.GetComponent<PlayerExperience>();
            if (playerExperience != null)
            {
                playerExperience.GainExperience(experienceAmount);

                // Return the orb to the pool instead of destroying it
                ExperienceOrbPool.Instance.ReturnOrb(this.gameObject);
            }
        }
    }


    public void ResetOrb()
    {
        lootVFX.Stop();
    }

}
