using Godot;

public class Actions : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    float rotationVelocity = 0.01f;
    bool canChangeStamina = true;
    private bool isRunning = false;
    public bool attackMode = false;
    public bool canAttack = true;
    public bool playerDie = false;
    public bool isWaitTime = false;
    private float sizeStaminaBar;
    private Panel staminaIndicator;

    private AnimationPlayer animPlayer;

    private KinematicBody player;

    private Player scPlayer;
    private playerState scPlayerState;
    private Timer timerAttackON;
    private Timer timerReloadScene;
    
    public AudioStreamPlayer2D audioPlayer;
    private AudioStreamPlayer2D audioAttack;

    private PackedScene attackEffect = GD.Load<PackedScene>("res://Prefabs/attackEffect.tscn");


    private Particles splashRainParticles;

    private Camera playerCamera;
    private Vector2 mousePosition;
    private float sizeRay = 50;

    public override void _Ready()
    {
        animPlayer = GetParent().GetNode<AnimationPlayer>("meshPlayer/AnimationPlayer");

        player = GetParent().GetNode<KinematicBody>(".");

        //Node nodePlayerState = player.GetNode<playerState>(".");
        scPlayerState = GetParent().GetNode<playerState>("playerState");
        scPlayer = GetParent().GetNode<Player>(".");

        timerReloadScene = GetParent().GetNode<Timer>("TimerRestartScene");

        audioPlayer = GetParent().GetNode<AudioStreamPlayer2D>("Sounds/stepSound");
        audioAttack = GetParent().GetNode<AudioStreamPlayer2D>("Sounds/attack");

        splashRainParticles = GetNode<Particles>("../splash");


        playerCamera = GetParent().GetNode<Camera>("Camera");

        staminaIndicator = scPlayer.GetNode<Panel>("Camera/PlayerUI/VBoxContainer/staminaIndicator");
        sizeStaminaBar = staminaIndicator.MarginRight - staminaIndicator.MarginLeft;

    }
    

    public async void attack1()
    {
        if(canAttack)
        {
            animPlayer.Play("attack2",-1,-4,true);
            canAttack = false;

            scPlayer.verifyRayUmbrella();
            audioAttack.Play();
            Spatial attackEffectNode = (Spatial)attackEffect.Instance();
            GetTree().Root.GetNode<Spatial>("rootTree").AddChild(attackEffectNode);
    
            Vector3 playerPos = scPlayer.GetNode<Position3D>("meshPlayer/attackPos").GlobalTransform.origin;
             attackEffectNode.Translate(playerPos);
            attackEffectNode.Rotation = scPlayer.Rotation;
            AnimationPlayer actionAnimPlayer = attackEffectNode.GetNode<AnimationPlayer>("AnimationPlayer");
            actionAnimPlayer.Play("attackEffect");
            await ToSignal(GetTree().CreateTimer(1f),"timeout");
            canAttack = true;
            attackEffectNode.QueueFree();
        }

    }

    public void walk(float moveSpeed,float animSpeed = 1)
    {
        if (scPlayer.playerHasFloor)
        {
            animPlayer.Play("walk",-1,animSpeed);
            scPlayer.currentMoveSpeed = moveSpeed;
            splashRainParticles.Visible = true;

            if(!audioPlayer.Playing)
            {
                audioPlayer.Play();
            }
        }
         
    }

    public void stop()
    {
        audioPlayer.Stop();

        if(playerState.currentStateUmbrella == playerState.STATE_UMBRELLA.DOWN_UMBRELLA){
            animPlayer.Play("stop_CloseUmbrella");
        }else{
             animPlayer.Play("stop");
        }
        
    }
    public void die(bool dieByTentacle = false)
    {
        if(!playerDie){
            GD.Print("O JOGADOR ESTA MORTO!!!!");
    
            if(!dieByTentacle){
                animPlayer.Play("Die");

            }else{
                animPlayer.Play("dieInTentacles");
            }
            timerReloadScene.Start();
            playerState.CurrentStatePlayer = playerState.STATE_PLAYER.DIE;
        }
        playerDie = true;

    }


    public void hitByTentacleGround()
    {
        scPlayer.damageReceived("dieInTentacles",5);
    }
    public async void fly()
    {
        scPlayer.currentGravity = scPlayer.flyGravity;
        splashRainParticles.Visible = false;
        await ToSignal(GetTree().CreateTimer(5f),"timeout");
        if(!scPlayer.playerHasFloor)
        {
            playerState.CurrentStatePlayer = playerState.STATE_PLAYER.FALL;

        }
    }
    public void fall()
    {
        scPlayer.currentGravity = scPlayer.fallGravity;
        animPlayer.Play("falling");
        GD.Print("caindo........");
    }
    
    public async void waitTime(float time)
    {
        isWaitTime = true;
        await ToSignal(GetTree().CreateTimer(time),"timeout");
        GD.Print("waittime :" + isWaitTime);
        isWaitTime = false;
        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.NONE;
    }

    private void rotatePlayer(Vector2 mousePosition)
    {
        //Vector3 playerRotation = new Vector3(0,mousePosition.x,0);
        player.RotateObjectLocal(Vector3.Up,-(mousePosition.x * rotationVelocity));
    }
    
    private void rotateCamera(Vector2 mousePosition)
    {

       if(mousePosition.y > 0  && playerCamera.Rotation.x > -1.05f)
       {
        playerCamera.RotateObjectLocal(Vector3.Right, -(mousePosition.y* rotationVelocity));
       
       }else if( mousePosition.y < 0 && playerCamera.Rotation.x < 0.8f ){
            playerCamera.RotateObjectLocal(Vector3.Right,-(mousePosition.y* rotationVelocity));
       }
    }
    
    public async void recoverStamina()
    {

        const int defaultStamina = 4;

        if(canChangeStamina){
            for(int i =0; i< defaultStamina; i++){  

                if(playerState.CurrentStatePlayer == playerState.STATE_PLAYER.RUN)
                {
                        break;
                }
                canChangeStamina =false;
                GD.Print("size bar Stamina " + sizeStaminaBar);

                staminaIndicator.MarginLeft -= (sizeStaminaBar / defaultStamina) /2;
                staminaIndicator.MarginRight += (sizeStaminaBar / defaultStamina) /2;
                GD.Print("margin Left " + staminaIndicator.MarginLeft);
                GD.Print("margin Right " + staminaIndicator.MarginRight);
                scPlayer.stamina ++;
                await ToSignal(GetTree().CreateTimer(1f),"timeout");
                canChangeStamina = true;
            }
        }

    }
    public async void loseStamina()
    {
        const int defaultStamina = 4;

        if(canChangeStamina){
            for(int i =0; i< defaultStamina; i++){  

                if(playerState.CurrentStatePlayer != playerState.STATE_PLAYER.RUN)
                {
                        break;
                }
                canChangeStamina =false;
                GD.Print("size bar Stamina " + sizeStaminaBar);

                staminaIndicator.MarginLeft += (sizeStaminaBar / defaultStamina) /2;
                staminaIndicator.MarginRight -= (sizeStaminaBar / defaultStamina) /2;
                GD.Print("margin Left " + staminaIndicator.MarginLeft);
                GD.Print("margin Right " + staminaIndicator.MarginRight);
                scPlayer.stamina --;
                await ToSignal(GetTree().CreateTimer(2f),"timeout");
                canChangeStamina = true;
            }
        }

       
    }
    
    public override void _Input(InputEvent inputEvent)
    {
        if(inputEvent is InputEventMouseButton mouseEvent  && mouseEvent.Pressed)
       {
          if(mouseEvent.ButtonIndex == (int)ButtonList.Left)
          {
              if(playerState.currentStateUmbrella == playerState.STATE_UMBRELLA.DOWN_UMBRELLA){
                  playerState.CurrentStatePlayer = playerState.STATE_PLAYER.ATTACK; 
              }

             
          }else if(mouseEvent.ButtonIndex == (int)ButtonList.Right)
          {
            if(playerState.currentStateUmbrella == playerState.STATE_UMBRELLA.UP_UMBRELLA) 
            {
               playerState.currentStateUmbrella = playerState.STATE_UMBRELLA.DOWN_UMBRELLA;
           
            }else if(playerState.currentStateUmbrella == playerState.STATE_UMBRELLA.DOWN_UMBRELLA) 
            {
               playerState.currentStateUmbrella = playerState.STATE_UMBRELLA.UP_UMBRELLA;
            } 

          }
        }else if(inputEvent is InputEventMouseButton mouseEventAny)
        {
            if(mouseEventAny.ButtonIndex == (int)ButtonList.Right)
            {
                playerState.CurrentStatePlayer = playerState.STATE_PLAYER.STOP;
            }
        }
        if(inputEvent is InputEventMouseMotion mouseMotion)
        {
            rotatePlayer(mouseMotion.Relative);
            rotateCamera(mouseMotion.Relative);
        }
    }

}
