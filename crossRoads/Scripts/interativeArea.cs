using Godot;
using System;

/// <summary>
/// área que interage com o guarda chuvas,pegar impulso para voar por exemplo
/// </summary>
public class interativeArea : Area
{
    private bool playerIsInsideArea = false;
    private bool playerEnteredArea = false;
   // private RayCast rayOfPlayer;


    public override void _Ready()
    {


    }


    /// <summary>
    /// indica que o jogador entrou na área de interação
    /// </summary>
    /// <param name="body"></param>
    private void areaEntered(Node body)
    {
        if(body.Name == "Player")
        {
            GD.Print("player entrou em uma area interativa");

            playerEnteredArea = true;

        }
    }


    /// <summary>
    /// indica que o jogador saiu da área de interação
    /// </summary>
    /// <param name="body"></param>
    private void areaExited(Node body)
    {

        if(body.Name == "Player")
        {
           playerEnteredArea = false;
           playerState.CurrentStatePlayer = playerState.STATE_PLAYER.FALL;
           GD.Print("player saiu da area");
        }
    }

    public override void _Process(float delta)
    {
        if(playerEnteredArea)
        {

            playerState.CurrentStatePlayer = playerState.STATE_PLAYER.FLY;
            
        } 
    }
}
