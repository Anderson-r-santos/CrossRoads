using Godot;
using System;

public class mainScene : Spatial
{
 
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
    public void pauseGame()
    {
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
    public void resumeGame()
    {
        GetTree().Paused = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
    public void exitGame()
    {
        GetTree().Quit();
    }
}
