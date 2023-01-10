using Godot;
using System;

/// <summary>
/// usado ao jogador chegar no final do jogo
/// </summary>
public class EndGame : Spatial
{

    private void endGame(Node body)
    {
       //
       body.GetNode<Actions>("Actions").endGame(this);
    }

    public void changeToThanksScene()
    {
        GetTree().ChangeScene("res://Scenes/Thanks.tscn");
    }


}
