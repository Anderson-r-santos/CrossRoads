using Godot;

public class Actions : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    float rotationVelocity = 0.01f;

    public float currentGravity;
    public float fallGravity;
    public float flyGravity;
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
        timerAttackON = GetParent().GetNode<Timer>("TimerAttackON"); 
        timerAttackON.Connect("timeout",this,"setCanAttack");
        timerReloadScene = GetParent().GetNode<Timer>("TimerRestartScene");

        audioPlayer = GetParent().GetNode<AudioStreamPlayer2D>("Sounds/stepSound");
        audioAttack = GetParent().GetNode<AudioStreamPlayer2D>("Sounds/attack");

        attackParticles = GetParent().GetNode<Particles>("meshPlayer/Armature/Skeleton/BoneAttachment/Position3D/attack_Particles");
         
        playerCamera = GetParent().GetNode<Camera>("Camera");
        fallGravity = defaultGravity * 40;
        flyGravity = -(defaultGravity * 50);
        currentGravity = defaultGravity * 20;
    }
    
    private void setCanAttack()
    {
        canAttack = true;

    }

    public void attack1()
    {
        if(canAttack)
        {
            animPlayer.Play("attack2",-1,-4,true);
            canAttack = false;
            timerAttackON.Start();

            //Vector3 from = playerCamera.ProjectRayOrigin(mousePosition);
            //Vector3 to = from + playerCamera.ProjectRayNormal(mousePosition) * sizeRay;
            scPlayer.rayUmbrella.Enabled = true;
            scPlayer.timerUmbrellaRay.Start();
            audioAttack.Play();
            attackParticles.Emitting = true;

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
        animPlayer.Play("stop_CloseUmbrella");

        audioPlayer.Stop();
        
        if(!audioAttack.Playing)
        {
            audioAttack.Stop();
        }
        
    }
    public void die()
    {
        if(!playerDie){
            GD.Print("O JOGADOR ESTA MORTO!!!!");
    
            animPlayer.Play("Die");
            timerReloadScene.Start();
            playerState.CurrentStatePlayer = playerState.STATE_PLAYER.DIE;
        }
        playerDie = true;

    }
    public void moviment()
    {
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
    private void rotatePlayer(Vector2 mousePosition)
    {
        //Vector3 playerRotation = new Vector3(0,mousePosition.x,0);
        player.RotateObjectLocal(Vector3.Up,-(mousePosition.x * rotationVelocity));
    }
    private void rotateCamera(Vector2 mousePosition)
    {
        playerCamera.RotateObjectLocal(Vector3.Right,-(mousePosition.y* rotationVelocity));
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
