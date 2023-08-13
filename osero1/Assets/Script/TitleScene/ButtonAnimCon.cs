using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimCon : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseEn()
    {
        this.anim.SetTrigger("modeButAnimS");
    }

    public void OnMouseEx()
    {
        this.anim.SetTrigger("modeButAnimE");
    }

}
