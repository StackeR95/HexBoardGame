using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class HandleInput : MonoBehaviour 
{

    public Text PlayerNameLabel;
    public InputField PvsPNameTextBox;
    public InputField PvsCNameTextBox;
    public static Toggle SwapRulePvsC;
    public static Toggle HumanFirst;
    public Toggle SRPvsC;
    public Toggle HFPVsC;
    public static string PlayerName;
    public static string Player1Name;
    public static string Player2Name;
    private int Count = 0;
    public static int CommunicationMode = -1;
    public static string GameMode="";
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
	// Update is called once per frame

    public void ClearString()
    {
        PlayerName = "";
        Player1Name = "";
        Player2Name = "";
        PvsPNameTextBox.text = "";
        PvsCNameTextBox.text = "";
        Count = 0;
        PlayerNameLabel.text = "Enter Player 1 Name :";
    }
    public void UpdatePvsC()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayerName = PvsCNameTextBox.text;
            SwapRulePvsC = SRPvsC;
            HumanFirst = HFPVsC;
            GameMode = "PvsC";
            SceneManager.LoadScene(1);
        }
    }
	public void UpdatePvsP () 
    {
        if (Count == 0)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                PlayerNameLabel.text = "Enter Player 2 Name :";
                Player1Name = PvsPNameTextBox.text;
                PvsPNameTextBox.text = "";
                Count++;
            }
        }
        
        else if(Count==1)
        {
            Player2Name = PvsPNameTextBox.text;
            Debug.Log("Tamam");
            GameMode = "PvsP";
            SceneManager.LoadScene(1);
            Count = 0;
        }
	}

    public void CommMode(int M)
    {
        CommunicationMode = M;
        GameMode = "CommMode";
        SceneManager.LoadScene(1);
    }

}
