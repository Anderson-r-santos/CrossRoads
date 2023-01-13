using Godot;
using System;


/// <summary>
/// verifica o estado do jogador e do guarda chuvas
/// </summary>
public class playerState : Node
{

  
    private bool umbrellaIsUp = true;



    public enum STATE_UMBRELLA
    {
        UP_UMBRELLA, DOWN_UMBRELLA
    }
    public enum STATE_PLAYER
    {
       NONE,WALK, STOP, RUN, ATTACK, DIE, RECEIVE_DAMAGE_HAND_GROUND, RECEIVE_DAMAGE_TENTACLE_GROUND, FLY, FALL,WAIT_TIME
    }

    private Particles umbrellaParticles;
    private Light lightUmbrella;
    private Player scPlayer;
    private static Actions scActions;


    private AnimationPlayer animPlayer;
    private AnimationPlayer animPlayerTemperature;

    public static bool isEndGame = false;

    private static STATE_PLAYER currentStatePlayer;
    public static STATE_PLAYER CurrentStatePlayer
    
    {
        get { return currentStatePlayer; }
        set
        {
            if(currentStatePlayer != STATE_PLAYER.DIE){
                if( currentStatePlayer != STATE_PLAYER.WAIT_TIME || value == STATE_PLAYER.NONE){
                
                    if (currentStatePlayer != value)
                    {
                        currentStatePlayer = value;
                    }
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
        animPlayerTemperature = GetParent().GetNode<AnimationPlayer>("meshPlayer/AnimationPlayerTemperature");
        currentStatePlayer = STATE_PLAYER.STOP;
        currentStateUmbrella = STATE_UMBRELLA.UP_UMBRELLA;
    }

    /// <summary>
    /// começa a subir a temperatura depois de o guarda chuvas ser aberto
    /// </summary>
    private void endTimerIncreaseTemperature()
    {
        if (currentStateUmbrella == STATE_UMBRELLA.UP_UMBRELLA)
        {
            scPlayer.increaseTemperature();
        }
    }

    /// <summary>
    /// decai a  temperatua quando o guarda chuvas está fechado
    /// </summary>
    private void endTimerDecreaseTemperature()
    {
        if (currentStateUmbrella == STATE_UMBRELLA.DOWN_UMBRELLA)
        {
            scPlayer.decreaseTemperature();
        }
    }


    /// <summary>
    /// chamado ao abrir o guarda chuvas
    /// </summary>
    private void umbrellaUp()
    {
        if (!umbrellaIsUp)
        {
            umbrellaParticles.Emitting = true;
            lightUmbrella.Visible = true;
            animPlayerTemperature.PlayBackwards("countBackToBrief");
            scPlayer.currentAmountDegrees = scPlayer.defaultAmountDegrees;
            umbrellaIsUp = true;
        }

    }

    /// <summary>
    /// chamado ao fechar o guarda chuvas
    /// </summary>
    private void umbrellaDown()
    {

        if (umbrellaIsUp)
        {
            umbrellaParticles.Emitting = false;
            lightUmbrella.Visible = false;
            animPlayerTemperature.Play("countFaceToPlayer");
            scPlayer.currentAmountDegrees = scPlayer.defaultAmountDegrees;
            umbrellaIsUp = false;
        }
    }

    public override void _Process(float delta)
    {
      //  GD.Print("current state : " + currentStatePlayer);
        if (currentStatePlayer != STATE_PLAYER.DIE)
        {
            if (currentStateUmbrella == STATE_UMBRELLA.UP_UMBRELLA)
            {
                umbrellaUp();
               if (currentStatePlayer == STATE_PLAYER.WALK)
                {
     
                    scActions.walk(scPlayer.moveSpeed);

                }
                else if (currentStatePlayer == STATE_PLAYER.STOP)
                {
                    scActions.stop();

                }
                else if (currentStatePlayer == STATE_PLAYER.RUN)
                {
                    if(scPlayer.stamina > 0){
                        scActions.walk(scPlayer.runSpeed,false,1.5f);
                        scActions.loseStamina();
                    }else{
                        scActions.walk(scPlayer.moveSpeed);
                    }


                }
                
                else if (currentStatePlayer == STATE_PLAYER.RECEIVE_DAMAGE_HAND_GROUND)
                {
                    scPlayer.damageReceived("damageEnemyGround", 3);
                }
                else if (currentStatePlayer == STATE_PLAYER.RECEIVE_DAMAGE_TENTACLE_GROUND)
                {
                    scActions.hitByTentacleGround();
                }
                else if (currentStatePlayer == STATE_PLAYER.FLY)
                {
                    scActions.fly();
                }                
                else if (currentStatePlayer == STATE_PLAYER.FALL)
                {
                    scActions.fall();
                }else if(currentStatePlayer == STATE_PLAYER.WAIT_TIME)
                {
                    float timeAnim = animPlayer.GetAnimation("tentacleInsideDamage").Length;
                    scActions.waitTime(timeAnim);
                }

            }
            else if (currentStateUmbrella == STATE_UMBRELLA.DOWN_UMBRELLA)
            {

                umbrellaDown();
                if (currentStatePlayer == STATE_PLAYER.WALK)
                {
                    scActions.walk(scPlayer.moveSpeed,true);

                }else if(currentStatePlayer == STATE_PLAYER.RUN)
                {
                    scActions.walk(scPlayer.runSpeed,true,1.5f);
                }
                else if (currentStatePlayer == STATE_PLAYER.STOP)
                {
                    scActions.stop();
                }
                else if (currentStatePlayer == STATE_PLAYER.ATTACK)
                {
                    if(!scPlayer.isRunning)
                    {
                        scActions.attack1();
                        if (!animPlayer.IsPlaying())
                        {
                            playerState.CurrentStatePlayer = playerState.STATE_PLAYER.STOP;
                        }
                    }
   
                }
            }

        }
    }

}
