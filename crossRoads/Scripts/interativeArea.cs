using Godot;
using System;

public class interativeArea : Area
{
    private Timer timerToFall;
    private bool playerIsInsideArea = false;
    private bool playerEnteredArea = false;
   // private RayCast rayOfPlayer;


    public override void _Ready()
    {
        timerToFall = GetNode<Timer>("TimerToFall");
        //rayOfPlayer = GetTree().Root.GetNode<RayCast>("rootTree/Player/rayToGround");

    }

    private void endtimerFly()
    {

        //playerState.CurrentStatePlayer = playerState.STATE_PLAYER.FALL;
        
    }

    private void areaEntered(Node body)
    {
        if(body.Name == "Player")
        {
            GD.Print("player entrou em uma area interativa");

                playerEnteredArea = true;

        }
    }
    private void areaExited(Node body)
    {

        if(body.Name == "Player")
        {
            if(!playerState.playerHasFloor)
            {
                timerToFall.Start();
            }
           playerEnteredArea = false;
           playerState.CurrentStatePlayer = playerState.STATE_PLAYER.FALL;
           GD.Print("player saiu da area");
        }
    }

    public override void _Process(float delta)
    {
        if(playerEnteredArea)
        {

            // if(playerState.CurrentStatePlayer != playerState.STATE_PLAYER.PUSHED_OUT)
            // {
                 playerState.CurrentStatePlayer = playerState.STATE_PLAYER.FLY;
            // }
            
        } 
    }
}
