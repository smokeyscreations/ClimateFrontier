using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerSpellController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public VisualEffect levelUp;

    private bool levelingUp;

    // Update is called once per frame
    void Update()
    {
        if(_animator != null)
        {
            // Check if the "1" key is pressed
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Trigger the spell cast animation
                _animator.SetTrigger("CastSpell1");
            }

            // Check if the "2" key is pressed
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // Trigger the spell cast animation
                _animator.SetTrigger("PowerUp");

                if(levelUp != null)
                    levelUp.Play();

                levelingUp = true;
                StartCoroutine(ResetBool(levelingUp, 0.5f));


            }
        }
        
    }

    IEnumerator ResetBool (bool boolToReset, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        levelingUp = !levelingUp;
    }
}
