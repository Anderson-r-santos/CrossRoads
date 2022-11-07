using Godot;
using System;

public class EndGame : Spatial
{

    private void endGame(Node body)
    {
        GetTree().ChangeScene("res://Prefabs/End.tscn");
    }

    public override void _Ready()
    {
        
    }


}
