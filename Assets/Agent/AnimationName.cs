using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationName : MonoBehaviour
{
    public TextMesh animName;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Fetch the current Animation clip information for the base layer
        // AnimatorClipInfo[] clipInfo = this.animator.GetCurrentAnimatorClipInfo(0);

        // //Access the Animation clip name
        // string clipName = clipInfo[0].clip.name;

        // animName.text = clipName;
    }
}
