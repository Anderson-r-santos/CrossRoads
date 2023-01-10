using Godot;
using System;

/// <summary>
/// responsável pela tradução da interface,menus,de acordo com o padrão da godot(arquivo .csv)
/// </summary>
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
    

    /// <summary>
    /// coloca o texto traduzido em um botão ou label
    /// </summary>
    /// <param name="arrayWithPathAndText">uma matriz com [caminho para o botão/label , o texto do botão/label]</param>
    /// <param name="isButton">verifica se é um botão,caso contrário,assume que é uma label</param>
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

    /// <summary>
    /// passa uma matriz contendo o texto traduzido do menu principal
    /// </summary>
    public  void setuptextMainMenu()
    {
        string[,] buttonText = {
                                    {"VBoxContainer/Button1","mainMenu1"},
                                    {"VBoxContainer/Button2","mainMenu1"},
                                    {"VBoxContainer/Button1","mainMenu1"},
                                };
        assignButtonText(buttonText);

    }

    /// <summary>
    /// passa o texto traduzido do menu de pause
    /// </summary>
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

}
