using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingController : MonoBehaviour
{
    public Animator animator;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public VisualEffect VFXGraph;
    public float dissolveRate = 0.05f;
    public float refreshRate = 0.1f;
    public float dieDelay = 0.25f;

    private bool alive = true;
    private Material[] dissolveMaterials;

    void Start()
    {
        if(VFXGraph != null)
        {
            VFXGraph.Stop();
            VFXGraph.gameObject.SetActive(false);
        }
        else
            Debug.Log("No VFX Graph assigned in the inspector.");

        if (skinnedMeshRenderer != null)
            dissolveMaterials = skinnedMeshRenderer.materials;
        else
            Debug.Log("No Skinned Mesh Renderer assigned in the inspector.");
    }
    private void Update()
    {
        if(alive)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(DissolveCo());
            }
        }

        if (!alive)
        {
            if (Input.GetKeyDown(KeyCode.R))
                Revive();
        }
    }

    IEnumerator DissolveCo ()
    {
        alive = false;

        if (animator != null)
            animator.SetTrigger("Die");
        else
        {
            Debug.Log("No Animator assigned in the inspector.");
            alive = true;
            yield break;
        }

        yield return new WaitForSeconds (dieDelay);
        
        if(VFXGraph != null)
        {
            VFXGraph.gameObject.SetActive(true);
            VFXGraph.Play();
        }
        else
            Debug.Log("No VFX Graph assigned in the inspector.");

        float counter = 0; 

        if(dissolveMaterials.Length > 0)
        {   
            while (dissolveMaterials[0].GetFloat("DissolveAmount_") < 1)
            {
                counter += dissolveRate;
                for(int i=0; i< dissolveMaterials.Length; i++)
                    dissolveMaterials[i].SetFloat("DissolveAmount_", counter);
                yield return new WaitForSeconds (refreshRate);
            }
        }
        else
        {
            Debug.Log("No Dissolving Material assigned to model.");
            yield break;
        }
    }

    public void Revive ()
    {
        if (animator != null)
        {
            animator.SetTrigger("Revive");
            alive = true;
        }
        else
            Debug.Log("No Animator assigned in the inspector.");

        if (dissolveMaterials.Length > 0)
        {
            for (int i = 0; i < dissolveMaterials.Length; i++)
                dissolveMaterials[i].SetFloat("DissolveAmount_", 0);
        }
    }
}
