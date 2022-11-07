using Godot;
using System;

public class interativeArea : Area
{
    private Timer timerToFall;
    bool playerInsideArea = false;


    public override void _Ready()
    {
        timerToFall = GetNode<Timer>("TimerToFall");  

    }

    private void endtimerFly()
    {

        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.FALL;
        
    }

    private void areaEntered(Node body)
    {
        if(body.Name == "Player")
        {
            GD.Print("player entrou em uma area interativa");

                playerInsideArea = true;

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
           playerInsideArea = false;
           //playerState.CurrentStatePlayer = playerState.STATE_PLAYER.FALL;
           GD.Print("player saiu da area");
        }
    }

    public override void _Process(float delta)
    {
        if(playerInsideArea)
        {
            if(playerState.CurrentStatePlayer != playerState.STATE_PLAYER.PUSHED_OUT)
            {
                playerState.CurrentStatePlayer = playerState.STATE_PLAYER.FLY;
            }
            
        } 
    }
}
