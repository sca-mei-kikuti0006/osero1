using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCon : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject titleCanvas;

    [SerializeField] private GameObject modeCanvas;
    private Animator modeCanAnim;

    [SerializeField] private GameObject colorSetCanvasCpu;

    [SerializeField] private GameObject colorSetCanvas2P;


    // Start is called before the first frame update
    void Start()
    {
        modeCanvas.gameObject.SetActive(false);
        modeCanAnim = modeCanvas.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        mainCamera.transform.Rotate(new Vector3(0, Time.deltaTime, 0));
    }

    public void PutTitleButton()
    {
        titleCanvas.gameObject.SetActive(false);
        modeCanvas.gameObject.SetActive(true);
        modeCanAnim.SetTrigger("canvasAnim");
    }

    //CPUëŒêÌÇ©2êlëŒêÌÇ©åàÇﬂÇÈ
    public void PutModeButton1()
    {
        modeCanvas.gameObject.SetActive(false);
        Instantiate(colorSetCanvasCpu);
    }
    public void PutModeButton2()
    {
        modeCanvas.gameObject.SetActive(false);
        Instantiate(colorSetCanvas2P);
    }

}
