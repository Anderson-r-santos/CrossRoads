using Godot;
using System;

public class language_game_UI : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
       
        TranslationServer.SetLocale("en_US"); 
        // TranslationServer.SetLocale("pt_BR"); 
        setupPauseMenuText();
    }
    
    private void assignButtonText(string [,] arrayWithPathAndText,bool isButton = true)
    {
        for(int y = 0 ; y < arrayWithPathAndText.GetLength(1) ; y++)
        {
            for(int x = 0 ; x < arrayWithPathAndText.GetLength(0) ; x++)
            {
                if(isButton)
                {
                    
                    GetNode<Button>(arrayWithPathAndText[x,0]).Text = Tr(arrayWithPathAndText[x,1]);
                }else{
                    GetNode<Label>(arrayWithPathAndText[x,0]).Text = Tr(arrayWithPathAndText[x,1]);
                }
            }  
        }
        //              y-0                           y-1
        //    x-0 ["VBoxContainer/Button1"   ,   "mainMenu1"]
        //    x-1 ["VBoxContainer/Button1"   ,   "mainMenu1"]
    }
    public  void setuptextMainMenu()
    {
        string[,] buttonText = {
                                    {"VBoxContainer/Button1","mainMenu1"},
                                    {"VBoxContainer/Button2","mainMenu1"},
                                    {"VBoxContainer/Button1","mainMenu1"},
                                };
        assignButtonText(buttonText);

    }
    private void setupPauseMenuText()
    {
        string[,] buttonsText = {
                                    {"../Player/pauseGame/pauseScene/VBoxContainer/ButtonResume","pauseMenu1"},
                                    {"../Player/pauseGame/pauseScene/VBoxContainer/ButtonExit","pauseMenu2"},
                                };
                                
        string[,] labelsText = {{"../Player/pauseGame/pauseScene/pauseLabel", "pauseMenu3"}};

        assignButtonText(buttonsText);
        assignButtonText(labelsText,false);

    }
    private void setupTipsText()
    {

    }
}
