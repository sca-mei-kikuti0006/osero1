using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CpuLevelButtonCOn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PutLevelButton()
    {
        //CPU‘Îí‚Ì©•ª‚ÌF‚ğŒˆ‚ß‚éACPU‚ÌF‚ğ•Û‘¶‚·‚é
        string level = this.gameObject.name;
        if (level == "ButtonEasy")
        {
            MainCon.cpuLevel = MainCon.level.Easy;
        }
        else if (level == "ButtonNormal")
        {
            MainCon.cpuLevel = MainCon.level.Normal;
        }
        else if (level == "ButtonHard")
        {
            MainCon.cpuLevel = MainCon.level.Hard;
        }

        SceneManager.LoadScene("MainScene");
    }
}
