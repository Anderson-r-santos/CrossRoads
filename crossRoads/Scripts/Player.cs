using Godot;
using System;

public class Player : KinematicBody
{
    
    public Actions scActions;
    public bool canReceiverDamage = true;
    private bool isRunning = false;

    private Timer timerHit; // tempo para poder atacar dnv
    private Timer timerIncreaseTemperature;  // tempo que a temperatuda corporal decai
    private Timer timerDecreaseTemperature;
    public Timer timerUmbrellaRay; //tempo em que fica ativo o ray que detecta o inmigo
    private int amountDamageLeft = 4;

    
    private OmniLight lightUmbrella;

    private RayCast rayToGround;
    public RayCast rayUmbrella;
    private int bodyTemperature = 8;

    private TextureRect temperatureIndicator;
    private cenario scSenario;
    private Area attackArea;

    private AudioStreamPlayer2D outRoad;
    private AudioStreamPlayer2D crackedGround;
    private AudioStreamPlayer2D lightBlink;

    private AnimationPlayer animationPlayer;
    private AnimationPlayer AnimationPlayerActions;

    private string whoAttack;

    public playerState scPlayerState;


    public string WhoAttack
    {
        get{return whoAttack;}
        set{whoAttack = value;}
    }


    public StaticBody currentRoadPlayerOnTop = null;

    public override void _Ready()
    {
        //Input.SetMouseMode(mode:Input.MouseMode.Captured);
        scActions = GetNode<Actions>("Actions");
        attackArea = GetNode<Area>("areaAttack");
        timerHit = GetNode<Timer>("TimerHit");
        timerHit.Connect("timeout",this,"hitReceived");

        AnimationPlayerActions = GetNode<AnimationPlayer>("AnimationPlayerActions");
        animationPlayer = GetNode<AnimationPlayer>("meshPlayer/AnimationPlayer");
        rayToGround = GetNode<RayCast>("rayToGround");
        //rayAttack = GetNode<RayCast>("meshPlayer/RayCast");
        lightUmbrella = GetNode<OmniLight>("umbrellaLight");
        timerIncreaseTemperature = GetNode<Timer>("TimerIncreaseTemperature");
        timerDecreaseTemperature = GetNode<Timer>("TimerDecreaseTemperature");
        timerUmbrellaRay = GetNode<Timer>("TimerUmbrellaRay");
        temperatureIndicator = GetNode<TextureRect>("Camera/Control/ninePatchRect/TextureRect2");
        scSenario = GetParent().GetNode<cenario>("cenarioInicial");
        outRoad = GetTree().Root.GetNode<AudioStreamPlayer2D>("rootTree/cenarioInicial/outRoad");
        crackedGround = GetTree().Root.GetNode<AudioStreamPlayer2D>("rootTree/cenarioInicial/crackedGround");
        lightBlink = GetNode<AudioStreamPlayer2D>("Sounds/lightBlink");

        scPlayerState = GetNode<playerState>("playerState");
        rayUmbrella = GetNode<RayCast>("Camera/RayCast");

    }

    private void playMsgWarning()
    {
        AnimationPlayerActions.Play("msgDeAviso");
    }
    public void hitReceived()
    {
        canReceiverDamage = true;
    }
    public void decreaseTemperature()
    {
        if(bodyTemperature > 0)
        {
            bodyTemperature -= 1;
            temperatureIndicator.RectScale = new Vector2(temperatureIndicator.RectScale.x,temperatureIndicator.RectScale.y - 0.2f);
        }
        else{
            damageReceived("temperature");
        }
    }
    public void increaseTemperature()
    {
        if(bodyTemperature < 8)
        {
            bodyTemperature +=1;
            temperatureIndicator.RectScale = new Vector2(0.2f,temperatureIndicator.RectScale.y + 0.02f);
        }
    }
    private void endTimerRayUmbrellaEnable()
    {
        rayUmbrella.Enabled = false;
    }
    void verifyRayUmbrella()
    {
        Node node = (Node) rayUmbrella.GetCollider();
        if(node != null && node.Name == "enemyCollisor")
        {
            Enemy enemy = (Enemy)node.GetParent().GetParent().GetNode<Enemy>(".");
 
            enemy.takeDamage(ref rayUmbrella);
  
          
        }

    }
    private void enemyEnteredAttackArea(Area area)
    {
        if(area.Name == "enemyCollisor")
        {
           Enemy scEnemy = area.GetParent().GetParent().GetNode<Enemy>(".");
           scEnemy.areaAttackEntered();
        }
    }

    private void enemyExitedAttackArea(Area area)
    {
        if(area.Name == "enemyCollisor")
        {
           Enemy scEnemy = area.GetParent().GetParent().GetNode<Enemy>(".");
           scEnemy.areaAttackExited();
        }
    }
    public void enemyClose()
    {
        AnimationPlayerActions.Play("lightBlink",-1,2);
        lightBlink.Play();
    }
    
    private void playerIsOutRoad(Vector3 hitPoint)
    {

        AnimationPlayerActions.Play("tremer",-1,10);
        scSenario.startTimer(hitPoint);

        if(!crackedGround.Playing)
        {
            crackedGround.Play();
        }
        if(!outRoad.Playing)
        {
            outRoad.Play();
        }
        
    }
    private void playerReturnToRoad()
    {
        crackedGround.Stop();
        outRoad.Stop();
       // AnimationPlayerActions.Stop();
        scSenario.playerIsNotInGround();
    }

    public void damageReceived(string whoAttack,int damage =1)
    {
        
        if(whoAttack == "enemy")
        {
            if(!canReceiverDamage)
            {
                return;
            }
            animationPlayer.Play("damageEnemyBasic",-1,6);
        }
        
        if( whoAttack == "hand")
        {
            if(!canReceiverDamage)
            {
                return;
            }
            animationPlayer.Play("damageEnemyGround");
        }
      
        if(amountDamageLeft > 0)
        {
            amountDamageLeft -=damage;
            canReceiverDamage = false;
            AnimationPlayerActions.Stop();
            AnimationPlayerActions.Play("damageReceiver");

        }else
        {
           scActions.die();
        }
        animationPlayer.PlaybackSpeed = 6;
        timerHit.Start();
            }

    private void restartScene()
    {
        GetTree().ReloadCurrentScene();
    }

    private bool verifyInputs()
    {
        bool isInputMovimentPressed = true;


    if(Input.IsActionPressed("moveFront"))
    {
       scActions.direction += Transform.basis.z;
        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.WALK;

 

    }else if(Input.IsActionPressed("moveBack"))
    {
        scActions.direction -= Transform.basis.z;
        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.WALK;

    }
    
    if(Input.IsActionPressed("moveLeft"))
    {
        scActions.direction +=  Transform.basis.x;
        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.WALK;

        
    }else if(Input.IsActionPressed("moveRight"))
    {
         scActions.direction -= Transform.basis.x;
        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.WALK;

    }
    if(Input.IsActionJustReleased("moveFront") || Input.IsActionJustReleased("moveBack") || Input.IsActionJustReleased("moveLeft") || Input.IsActionJustReleased("moveRight"))
    {
        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.STOP;
        isInputMovimentPressed = false;
    }
 
        return isInputMovimentPressed;
    }
 
 public override void _Process(float delta)
 {

     if(verifyInputs())
     {
        scActions.walk();
     }
     if(rayUmbrella.Enabled)
     {
         verifyRayUmbrella();
     }
   
     Node nodeGround = (Node) rayToGround.GetCollider();
       
     if(nodeGround != null)

     {
         playerState.playerHasFloor = true;
         //   GD.Print ("player collidindo com " + nodeGround.Name);
         Vector3 hitPoint = rayToGround.GetCollisionPoint();


        if(nodeGround.Name == "interativeArea")
        {
            playerState.CurrentStatePlayer = playerState.STATE_PLAYER.PUSHED_OUT;
            //GD.Print("atingiu o top da area e sera empurrado pra fora");
        }
        else if(nodeGround.GetParent().Name == "groundColision")
         {
             playerIsOutRoad(hitPoint);
             currentRoadPlayerOnTop = null;
         }else{
             currentRoadPlayerOnTop = (StaticBody) nodeGround;
             playerReturnToRoad();
         }

     }else{
         playerState.playerHasFloor = false;
     }
     

 }
 
 }
 

