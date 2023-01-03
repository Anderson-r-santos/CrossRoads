using Godot;

public class Actions : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    float rotationVelocity = 0.01f;

    private float currentGravity;
    private float fallGravity;
    private float flyGravity;
    private const float moveSpeed = 50f;
    private const float runSpeed = 60f;
    private float currentMoveSpeed;

    private bool isRunning = false;
    public float gravity = 9.8f;
    public float defaultGravity = 9.8f;
    public bool attackMode = false;
    public bool canAttack = true;
    private bool playerDie = false;


    public Vector3 direction;
    private AnimationPlayer animPlayer;

    private KinematicBody player;

    private Player scPlayer;
    private playerState scPlayerState;
    private Timer timerAttackON;
    private Timer timerReloadScene;
    
    public AudioStreamPlayer2D audioPlayer;
    private AudioStreamPlayer2D audioAttack;

    private Particles attackParticles;
    private Particles attackParticles2;
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
        currentMoveSpeed = moveSpeed;
        timerReloadScene = GetParent().GetNode<Timer>("TimerRestartScene");

        audioPlayer = GetParent().GetNode<AudioStreamPlayer2D>("Sounds/stepSound");
        audioAttack = GetParent().GetNode<AudioStreamPlayer2D>("Sounds/attack");

        attackParticles = GetParent().GetNode<Particles>("meshPlayer/Armature/Skeleton/BoneAttachment/Position3D/attack_Particles");
        attackParticles2 = GetParent().GetNode<Particles>("meshPlayer/Armature/Skeleton/BoneAttachment/Position3D/attack_Particles2");
        splashRainParticles = GetNode<Particles>("../splash");


        playerCamera = GetParent().GetNode<Camera>("Camera");
        fallGravity = defaultGravity * 40;
        flyGravity = -(defaultGravity * 50);
        currentGravity = defaultGravity * 20;

    }
    

    public async void attack1()
    {
        if(canAttack)
        {
            animPlayer.Play("attack2",-1,-4,true);
            canAttack = false;

            scPlayer.verifyRayUmbrella();
            audioAttack.Play();
            attackParticles.Emitting = true;
            attackParticles2.Emitting = true;
            await ToSignal(GetTree().CreateTimer(1f),"timeout");
            canAttack = true;
        }

    }

    public void walk()
    {

        if(isRunning)
        {
            animPlayer.PlaybackSpeed = 1.5f;
            currentMoveSpeed = runSpeed;
        }else
        {
            animPlayer.PlaybackSpeed = 1;
            currentMoveSpeed = moveSpeed;
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
    public void moviment()
    {
        splashRainParticles.Visible = true;
        if(!audioPlayer.Playing)
        {
            audioPlayer.Play();
        }


        if(Input.IsActionPressed("run"))
        {
            isRunning = true;
        }else
        {
            isRunning = false;
        }


    }
       public void pushOutPlayer(float delta)
    {
        GD.Print("empurrando");
       direction += scPlayer.Transform.basis.z*5;

       
    }
    public void hitByTentacleGround()
    {
        scPlayer.damageReceived("dieInTentacles",5);
    }
    public async void fly()
    {
        currentGravity = flyGravity;
        splashRainParticles.Visible = false;
        await ToSignal(GetTree().CreateTimer(5f),"timeout");
        if(!playerState.playerHasFloor)
        {
            playerState.CurrentStatePlayer = playerState.STATE_PLAYER.FALL;

        }
    }
    public void fall()
    {
        currentGravity = fallGravity;
        animPlayer.Play("falling");
        GD.Print("caindo........");
    }
    public async void changeToStoppedPlayer()
    {
        float timeAnim = animPlayer.GetAnimation("tentacleInsideDamage").Length;
        await ToSignal(GetTree().CreateTimer(timeAnim),"timeout");
        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.STOP;
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
    public override void _PhysicsProcess(float delta)
    {
        if(!playerDie)
        {
            direction.y = 0;
            direction.z *= currentMoveSpeed * delta;
            direction.x *= currentMoveSpeed * delta;

            direction.y -= currentGravity * delta;
            player.MoveAndSlide(direction,Vector3.Up);
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
