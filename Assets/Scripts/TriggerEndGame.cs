using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEndGame : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Animator dog;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            doorAnimator.SetBool("CutScene", true);
            dog.SetBool("CutScene", true);
        }
        
    }
}
