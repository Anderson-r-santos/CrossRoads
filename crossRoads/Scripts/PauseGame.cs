using Godot;
using System;

/// <summary>
/// pausa o jogo e exibe um menu
/// </summary>
public class PauseGame : Node
{
    private mainScene scMainScene;


    public override void _Ready()
    {
       scMainScene = GetTree().Root.GetNode<mainScene>("rootTree");
       GetNode<Button>("pauseScene/VBoxContainer/ButtonResume").Connect("pressed",this,"setResumeGame");
       GetNode<Button>("pauseScene/VBoxContainer/ButtonExit").Connect("pressed",this,"setExitGame");
    }
    public void setPauseGame()
    {
        GD.Print("tetando pausar o jogo");
        scMainScene.pauseGame();
        GetNode<Control>("pauseScene").Visible = true;
    }
    private void setResumeGame()
    {
        scMainScene.resumeGame();
        GetNode<Control>("pauseScene").Visible = false;

    }
    private void setExitGame()
    {
        scMainScene.exitGame();
    }
}
