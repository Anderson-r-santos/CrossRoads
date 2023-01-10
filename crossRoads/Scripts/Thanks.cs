using Godot;
using System;

public class Thanks : Control
{
    public override void _Ready()
    {
        timeToChangeScene();
    }

    private async void timeToChangeScene()
    {
        await ToSignal(GetTree().CreateTimer(5f),"timeout");
        changeToCreditsScene();
    }
    private void changeToCreditsScene()
    {
        GetTree().ChangeScene("res://Scenes/Credits.tscn");
    }


}
