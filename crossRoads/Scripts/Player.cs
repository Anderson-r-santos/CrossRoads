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
    private int lifeTotal;

    
    private OmniLight lightUmbrella;

    private RayCast rayToGround;
    public RayCast rayUmbrella;
    private int bodyTemperature = 8;

    private cenario scSenario;
    private Area attackArea;

    private AudioStreamPlayer2D outRoad;
    private AudioStreamPlayer2D crackedGround;
    private AudioStreamPlayer2D lightBlink;

    private AnimationPlayer animationPlayer;
    private AnimationPlayer AnimationPlayerActions;
    private AnimationPlayer animationPlayerUI;

    private string whoAttack;

    public playerState scPlayerState;


    public string WhoAttack
    {
        get{return whoAttack;}
        set{whoAttack = value;}
    }


    public StaticBody currentRoadPlayerOnTop = null;

    private Label countDegrees;
    private MeshInstance verticalBarTemperature;
    private float sizeBarChunks;

    public int defaultAmountDegrees = 1;
    public int currentAmountDegrees;


    public override void _Ready()
    {
        lifeTotal = amountDamageLeft;
        //Input.SetMouseMode(mode:Input.MouseMode.Captured);
        scActions = GetNode<Actions>("Actions");
        attackArea = GetNode<Area>("areaAttack");
        timerHit = GetNode<Timer>("TimerHit");
        timerHit.Connect("timeout",this,"hitReceived");

        AnimationPlayerActions = GetNode<AnimationPlayer>("AnimationPlayerActions");
        animationPlayer = GetNode<AnimationPlayer>("meshPlayer/AnimationPlayer");
        animationPlayerUI = GetNode<AnimationPlayer>("Camera/AnimationPlayerUI");

        countDegrees = GetNode<Label>("meshPlayer/Armature/Skeleton/BoneAttachBriefCase/CountDegrees/Viewport/Label");

        verticalBarTemperature = GetNode<MeshInstance>("meshPlayer/Armature/Skeleton/BoneAttachBriefCase/barTemperatureIndicator");

        rayToGround = GetNode<RayCast>("rayToGround");
        //rayAttack = GetNode<RayCast>("meshPlayer/RayCast");
        lightUmbrella = GetNode<OmniLight>("umbrellaLight");
        timerIncreaseTemperature = GetNode<Timer>("TimerIncreaseTemperature");
        timerDecreaseTemperature = GetNode<Timer>("TimerDecreaseTemperature");
        timerUmbrellaRay = GetNode<Timer>("TimerUmbrellaRay");

        scSenario = GetParent().GetNode<cenario>("cenarioInicial");
        outRoad = GetTree().Root.GetNode<AudioStreamPlayer2D>("rootTree/cenarioInicial/outRoad");
        crackedGround = GetTree().Root.GetNode<AudioStreamPlayer2D>("rootTree/cenarioInicial/crackedGround");
        lightBlink = GetNode<AudioStreamPlayer2D>("Sounds/lightBlink");

        scPlayerState = GetNode<playerState>("playerState");
        rayUmbrella = GetNode<RayCast>("Camera/RayCast");
 
        sizeBarChunks = verticalBarTemperature.Scale.y / amountDamageLeft - 0.01f; // 0.1 so pra escala nao ser 0 e bugar
        currentAmountDegrees = defaultAmountDegrees;

    }

    private void playMsgWarning()
    {
        AnimationPlayerActions.Play("damageReceiver");
    }
    public void hitReceived()
    {
        canReceiverDamage = true;
    }
    private void setCountTemperature(int amountDegrees,bool increase = true)
    {
        string splitCountText = countDegrees.Text.Substring(0,countDegrees.Text.IndexOf(" "));
        int countText = splitCountText.ToInt();
        if(increase)
        {
            countText += amountDegrees;
        }else{
            countText -= amountDegrees;
        }
        
        countDegrees.Text = countText.ToString() + " ° c";
         
    }
    private Color fromGreenToRed()
    {
        Material material = (Material)verticalBarTemperature.Get("material/0");
        Color colorEmission = (Color)material.Get("emission");
        Color color8 = Color.Color8((byte)colorEmission.r8,(byte)colorEmission.g8,(byte)colorEmission.b8);


        //cor vai de 0 a 1, e nao 255
        int fixedSize = 255 / lifeTotal;
       // GD.Print("fixedSize " + fixedSize);

        colorEmission.r8 += fixedSize + fixedSize;
        colorEmission.g8 -= fixedSize;
        colorEmission.b8 = 0;

        material.Set("emission",colorEmission);

        return colorEmission;
    }
    private Color fromRedToGreen()
    {
        Material material = (Material)verticalBarTemperature.Get("material/0");
        Color colorEmission = (Color)material.Get("emission");
        Color color8 = Color.Color8((byte)colorEmission.r8,(byte)colorEmission.g8,(byte)colorEmission.b8);


        //cor vai de 0 a 1, e nao 255
        int fixedSize = 255 / lifeTotal;
       // GD.Print("fixedSize " + fixedSize);

        colorEmission.r8 -= fixedSize + fixedSize;
        colorEmission.g8 += fixedSize;
        colorEmission.b8 = 0;

        material.Set("emission",colorEmission);

        return colorEmission;
    }
    public void decreaseTemperature()
    {

        setCountTemperature(currentAmountDegrees,false);
        currentAmountDegrees += 1;

        if(bodyTemperature > -9)
        {
            bodyTemperature -= currentAmountDegrees;
            fromGreenToRed();
          
        }
        else if(amountDamageLeft >= 0)
            {
                
                damageReceived("temperature");
               
                Vector3 scaleBarTemperature = verticalBarTemperature.Scale; 
                if(scaleBarTemperature.y > 0.1f){
                    scaleBarTemperature.y -= sizeBarChunks;
                    verticalBarTemperature.Scale = scaleBarTemperature;
                }
            }
    }
    public void increaseTemperature()
    {
        if(bodyTemperature < 8)
        {
            fromRedToGreen();
            bodyTemperature += currentAmountDegrees;
            setCountTemperature(currentAmountDegrees);
            currentAmountDegrees += 1;
            
            if(bodyTemperature > -9){
                Vector3 scaleBarTemperature = verticalBarTemperature.Scale; 
                if(amountDamageLeft < 4)
                {
                    amountDamageLeft +=1;
                }
                if(scaleBarTemperature.y < 1){
                    scaleBarTemperature.y += sizeBarChunks;
                    verticalBarTemperature.Scale = scaleBarTemperature;

                }
            }
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
            Enemy scEnemy = node.FindParent("Enemy").GetNode<Enemy>(".");
 
            scEnemy.takeDamage();
  
          
        }

    }
    private void enemyEnteredAttackArea(Area area)
    {
        if(area.Name == "enemyCollisor")
        {
           Enemy scEnemy = area.FindParent("Enemy").GetNode<Enemy>(".");
           scEnemy.enemyInsideBodyPlayer();
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
       AnimationPlayerActions.Stop();
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
            amountDamageLeft -= damage;
            canReceiverDamage = false;

            animationPlayerUI.Play("damageReceiver");

        }else
        {
           scActions.die();
        }
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
 

