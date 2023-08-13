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
        //CPU対戦の時自分の色を決める、CPUの色を保存する
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
