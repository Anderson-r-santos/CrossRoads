using Godot;
using System;

public class languageMenu : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
    private void setEnglishLanguage()
    {
        TranslationServer.SetLocale("en_US");
        GetTree().ChangeScene("res://Scenes/mainMenu.tscn");
        
    }
      private void setPortugueseLanguage()
    {
         TranslationServer.SetLocale("pt_BR");
         GetTree().ChangeScene("res://Scenes/mainMenu.tscn"); 
    }
}
