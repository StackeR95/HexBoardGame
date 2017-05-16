using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {

    public Animator MainAnim;
    public Animator NewGameAnim;
    public Animator PlayerVsPlayerAnim;
    public Animator PlayerVsComputerAnim;
    public Animator CompVsComp;
    public Animator SettingsAnim;
    public Animator PlayerVsAgentAnim;
    public GameObject PvsPPanel;
    public GameObject PvsCPanel;
    public HandleInput PVsP;
    public HandleInput PVsC;
    public Dropdown BoardThemeDropDown;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
     
     DirectoryInfo dir = new DirectoryInfo(Application.dataPath+"//Board Themes");
    // Debug.Log(dir);
     FileInfo[] info = dir.GetFiles("*.jpg.*");
     BoardThemeDropDown.ClearOptions();
     string newText ;
     int j = 0;
     Dropdown.OptionData newItem;
            for (int i = 0; i < info.Length;i=i+2 )   
            {
                newText = info[i].ToString().Remove(0,info[i].ToString().LastIndexOf("\\")+1);
                newItem = new Dropdown.OptionData(newText);
                BoardThemeDropDown.options.Insert(j, newItem);
                j++;
            }
    }
    public void AnimationController(int Scene)
    {
        if (Scene == 0) // From Main Menu To New Game Pannel and Vice 
        {
            MainAnim.SetTrigger("MainMenuOutPanelTrigger");
            NewGameAnim.SetTrigger("NewGamePanelINTrigger");
        }
        if(Scene==1) //From NewGamePanel to Player Vs Player  and Vice
        {

            PVsP.ClearString();
            NewGameAnim.SetTrigger("NewGamePanelINTrigger"); 
            PlayerVsPlayerAnim.SetTrigger("PVsPPanelIN");


        }
        if (Scene == 2) //From NewGamePanel to Player Vs Computer  and Vice
        {
            PVsC.ClearString();
            PlayerVsComputerAnim.SetTrigger("PVsCTrigger");
            NewGameAnim.SetTrigger("NewGamePanelINTrigger");
            
        }
        if (Scene == 3)//From NewGamePanel to Settings and Vice
        {
            MainAnim.SetTrigger("MainMenuOutPanelTrigger");
            SettingsAnim.SetTrigger("SettingsPanelTrigger");
        }
        if(Scene==4) //From NewGamePanel To CvsC and Vice
        {
            NewGameAnim.SetTrigger("NewGamePanelINTrigger");
            CompVsComp.SetTrigger("CvsCTrigger");    
        }
        if(Scene==5)
        {
            CompVsComp.SetTrigger("CvsCTrigger");
            PlayerVsAgentAnim.SetTrigger("PlayerVsAgentTrigger");
        }
        }


}
