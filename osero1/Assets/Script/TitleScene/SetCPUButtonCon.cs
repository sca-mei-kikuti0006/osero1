using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCPUButtonCon : MonoBehaviour
{
    [SerializeField] private GameObject cpuLevelCanvas;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PutColorSetButton() {
        //CPU‘Îí‚Ì©•ª‚ÌF‚ğŒˆ‚ß‚éACPU‚ÌF‚ğ•Û‘¶‚·‚é
        string color = this.gameObject.name;
        if (color == "ButtonB")
        {
            MainCon.cpuColor = MainCon.turnBW.White;
        }
        else if (color == "ButtonW")
        {
            MainCon.cpuColor = MainCon.turnBW.Black;
        }

        transform.parent.gameObject.gameObject.SetActive(false);
        Instantiate(cpuLevelCanvas);
    }

   
}
