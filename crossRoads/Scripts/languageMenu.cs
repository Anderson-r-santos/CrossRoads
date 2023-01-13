using Godot;
using System;

public class languageMenu : Control
{

    public override void _Ready()
    {

   
    }
    private void setEnglishLanguage()
    {
        Control mainMenu =  GetNode<Control>("../menu");
        mainMenu.Visible = true;
        mainMenu.GetNode<language_game_UI>("Translate").setLanguage("en_US");
        QueueFree();
        //GetTree().ChangeScene("res://Scenes/mainMenu.tscn");
       
    }
      private void setPortugueseLanguage()
    {
        Control mainMenu =  GetNode<Control>("../menu");
        mainMenu.Visible = true;
        mainMenu.GetNode<language_game_UI>("Translate").setLanguage("pt_BR");

         QueueFree();
         //GetTree().ChangeScene("res://Scenes/mainMenu.tscn"); 
    }
}
