using Godot;
using System;

public class playerState : Node
{

    public static bool playerHasFloor = true;
    



    public enum STATE_UMBRELLA
    {
         UP_UMBRELLA,DOWN_UMBRELLA
    }
    public enum STATE_PLAYER
    {
       WALK,STOP,ATTACK,DIE,RECEIVE_DAMAGE_BASIC_ENEMY,RECEIVE_DAMAGE_GROUND_ENEMY,FLY,FALL,PUSHED_OUT
    }

    private Particles umbrellaParticles;
    private Light lightUmbrella;
    private Player scPlayer;
    private static Actions scActions;


     private AnimationPlayer animPlayer;

    private static STATE_PLAYER currentStatePlayer;
    public static STATE_PLAYER CurrentStatePlayer{
        get{return currentStatePlayer;}
        set{
            if(currentStatePlayer != value && currentStatePlayer != STATE_PLAYER.DIE)
            {

                currentStatePlayer = value;

                if(currentStatePlayer != STATE_PLAYER.ATTACK &&  value == STATE_PLAYER.ATTACK)
                {
                    currentStatePlayer = value;

                }
            }
        }     

    }
    public static STATE_UMBRELLA currentStateUmbrella;
    public override void _Ready()
    {
        scPlayer = GetParent().GetNode<Player>(".");
        scActions = GetParent().GetNode<Actions>("Actions");
        umbrellaParticles = GetParent().GetNode<Particles>("umbrellaParticles");
        lightUmbrella = GetParent().GetNode<Light>("umbrellaLight");
        animPlayer = GetParent().GetNode<AnimationPlayer>("meshPlayer/AnimationPlayer");
        currentStatePlayer = STATE_PLAYER.STOP;
        currentStateUmbrella = STATE_UMBRELLA.UP_UMBRELLA;
    }


    private void endTimerIncreaseTemperature()
    {
        if(currentStateUmbrella == STATE_UMBRELLA.UP_UMBRELLA)
        {
            scPlayer.increaseTemperature();
        }
    }
    private void endTimerDecreaseTemperature()
    {
        if(currentStateUmbrella == STATE_UMBRELLA.DOWN_UMBRELLA)
        {
            scPlayer.decreaseTemperature();
        }
    }
    private void umbrellaUp()
    {

        umbrellaParticles.Emitting = true;
        lightUmbrella.Visible = true;
        
    }
    private void umbrellaDown()
    {

        umbrellaParticles.Emitting = false;
        lightUmbrella.Visible = false;
  
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
     if(currentStateUmbrella == STATE_UMBRELLA.UP_UMBRELLA)
     {
       // animPlayer.PlaybackSpeed = 1;
        umbrellaUp();
        if(currentStatePlayer == STATE_PLAYER.WALK)
        {
            if(playerHasFloor)
            {
                scActions.moviment();
            }
            animPlayer.Play("walk");

        }else if(currentStatePlayer == STATE_PLAYER.STOP)
        {
            scActions.audioPlayer.Stop();
            animPlayer.Play("stop");

        }else if(currentStatePlayer == STATE_PLAYER.RECEIVE_DAMAGE_BASIC_ENEMY)
        {

            scPlayer.damageReceived("enemy");

        }else if(currentStatePlayer == STATE_PLAYER.RECEIVE_DAMAGE_GROUND_ENEMY)
        {

            scPlayer.damageReceived("hand",2);

        }else if(currentStatePlayer == STATE_PLAYER.FLY)
        {
            scActions.currentGravity = scActions.flyGravity;
            

        }else if(currentStatePlayer == STATE_PLAYER.PUSHED_OUT)
        {
            scActions.pushOutPlayer(delta);

        }
        else if(currentStatePlayer == STATE_PLAYER.FALL)
        {
            scActions.currentGravity = scActions.fallGravity;
            GD.Print("caindo........");
        }

     }else if(currentStateUmbrella == STATE_UMBRELLA.DOWN_UMBRELLA)
            {
                
                umbrellaDown();
                if(currentStatePlayer == STATE_PLAYER.WALK)
                {
                    animPlayer.Play("walk_closeUmbrella");

                }else if(currentStatePlayer == STATE_PLAYER.STOP)
                {
                    scActions.stop();
                }else if(currentStatePlayer == STATE_PLAYER.ATTACK)
                {
                    scActions.attack1();
                    if(!animPlayer.IsPlaying()){
                        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.STOP;
                    }
                }
            }

 }

}
