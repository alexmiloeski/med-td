using UnityEngine;

public class AnimatedHead : MonoBehaviour
{
    public RuntimeAnimatorController[] ac;
    //public RuntimeAnimatorController ac2;
    //public RuntimeAnimatorController ac3;
    
    internal void SetAnimatorController(int i)
    {
        if (i > ac.Length)
        {
            Debug.Log("i = " + i + " is greater than ac.Length");
            return;
        }

        Debug.Log("SetAnimatorController: " + i);

        Animator animator = gameObject.GetComponent<Animator>();
        //animator.runtimeAnimatorController = Resources.Load("path_to_your_controller") as RuntimeAnimatorController;
        switch (i)
        {
            default:
            case 1:
                {
                    if (ac[0] != null)
                    {
                        Debug.Log("Loading ac[0]");
                        animator.runtimeAnimatorController = ac[0];
                    }
                }
                break;
            case 2:
                {
                    if (ac[1] != null)
                    {
                        Debug.Log("Loading ac[1]");
                        animator.runtimeAnimatorController = ac[1];
                    }
                }
                break;
            case 3:
                {
                    if (ac[2] != null)
                    {
                        Debug.Log("Loading ac[2]");
                        animator.runtimeAnimatorController = ac[2];
                    }
                }
                break;
            case 4:
                {
                    if (ac[3] != null)
                    {
                        Debug.Log("Loading ac[3]");
                        animator.runtimeAnimatorController = ac[3];
                    }
                }
                break;
        }
    }
}
