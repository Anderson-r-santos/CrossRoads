using Godot;

public class Player : KinematicBody
{

    public Actions scActions;
    private bool isRunning = false;


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

    private AudioStreamPlayer2D outRoad;
    private AudioStreamPlayer2D crackedGround;
    private AudioStreamPlayer2D lightBlink;

    private AnimationPlayer animationPlayer;
    private AnimationPlayer AnimationPlayerActions;
    private AnimationPlayer animationPlayerUI;

    public playerState scPlayerState;


    private Label countDegrees;
    private MeshInstance verticalBarTemperature;
    private float sizeBarChunks;

    public int defaultAmountDegrees = 1;
    public int currentAmountDegrees;

    private Spatial tentaclesInsideBody;


    public override void _Ready()
    {
        lifeTotal = amountDamageLeft;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        scActions = GetNode<Actions>("Actions");


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

        tentaclesInsideBody = GetNode<Spatial>("tentaclesInsidePos");

        scPlayerState = GetNode<playerState>("playerState");
        rayUmbrella = GetNode<RayCast>("Camera/RayCast");

        sizeBarChunks = verticalBarTemperature.Scale.y / amountDamageLeft - 0.01f; // 0.1 so pra escala nao ser 0 e bugar
        currentAmountDegrees = defaultAmountDegrees;

        tentaclesInsideBody.Visible = false;

    }

    private void playMsgWarning()
    {
        AnimationPlayerActions.Play("damageReceiver");
    }

    private void setCountTemperature(int amountDegrees, bool increase = true)
    {
        string splitCountText = countDegrees.Text.Substring(0, countDegrees.Text.IndexOf(" "));
        int countText = splitCountText.ToInt();
        if (increase)
        {
            countText += amountDegrees;
        }
        else
        {
            countText -= amountDegrees;
        }

        countDegrees.Text = countText.ToString() + " ° c";

    }
    private Color fromGreenToRed()
    {
        Material material = (Material)verticalBarTemperature.Get("material/0");
        Color colorEmission = (Color)material.Get("emission");
        Color color8 = Color.Color8((byte)colorEmission.r8, (byte)colorEmission.g8, (byte)colorEmission.b8);


        //cor vai de 0 a 1, e nao 255
        int fixedSize = 255 / lifeTotal;
        // GD.Print("fixedSize " + fixedSize);

        colorEmission.r8 += fixedSize + fixedSize;
        colorEmission.g8 -= fixedSize;
        colorEmission.b8 = 0;

        material.Set("emission", colorEmission);

        return colorEmission;
    }
    private Color fromRedToGreen()
    {
        Material material = (Material)verticalBarTemperature.Get("material/0");
        Color colorEmission = (Color)material.Get("emission");
        Color color8 = Color.Color8((byte)colorEmission.r8, (byte)colorEmission.g8, (byte)colorEmission.b8);


        //cor vai de 0 a 1, e nao 255
        int fixedSize = 255 / lifeTotal;
        // GD.Print("fixedSize " + fixedSize);

        colorEmission.r8 -= fixedSize + fixedSize;
        colorEmission.g8 += fixedSize;
        colorEmission.b8 = 0;

        material.Set("emission", colorEmission);

        return colorEmission;
    }
    public void decreaseTemperature()
    {

        setCountTemperature(currentAmountDegrees, false);
        currentAmountDegrees += 1;

        if (bodyTemperature > -9)
        {
            bodyTemperature -= currentAmountDegrees;
            fromGreenToRed();

        }
        else if (amountDamageLeft >= 0)
        {

            damageReceived("temperature");

            Vector3 scaleBarTemperature = verticalBarTemperature.Scale;
            if (scaleBarTemperature.y > 0.1f)
            {
                scaleBarTemperature.y -= sizeBarChunks;
                verticalBarTemperature.Scale = scaleBarTemperature;
            }
        }
    }
    public void increaseTemperature()
    {
        if (bodyTemperature < 8)
        {
            fromRedToGreen();
            bodyTemperature += currentAmountDegrees;
            setCountTemperature(currentAmountDegrees);
            currentAmountDegrees += 1;

            if (bodyTemperature > -9)
            {
                Vector3 scaleBarTemperature = verticalBarTemperature.Scale;
                if (amountDamageLeft < 4)
                {
                    amountDamageLeft += 1;
                }
                if (scaleBarTemperature.y < 1)
                {
                    scaleBarTemperature.y += sizeBarChunks;
                    verticalBarTemperature.Scale = scaleBarTemperature;

                }
            }
        }

    }
    public void verifyRayUmbrella()
    {
        
        if (rayUmbrella.IsColliding())
        {
            Node node = (Node)rayUmbrella.GetCollider();
            if(node.Name == "enemyCollisor")
            {

            Enemy scEnemy = node.GetParent().GetNode<Enemy>("../.");

            scEnemy.takeDamage();
            }


        }

    }


    public void enemyClose()
    {
        AnimationPlayerActions.Play("lightBlink", -1, 2);
        lightBlink.Play();
    }

    private void playerIsOutRoad(Vector3 hitPoint)
    {
        AnimationPlayerActions.Play("tremer", -1, 10);

        scSenario.startTimer(hitPoint);

        if (!crackedGround.Playing)
        {
            crackedGround.Play();
        }
        if (!outRoad.Playing)
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

    public void damageReceived(string animation, int damage = 1, float animSpeed = 1)
    {
        if (animation != "temperature")
        {

            animationPlayer.Play(animation, -1, animSpeed);

        }

        if (amountDamageLeft > 0)
        {
            amountDamageLeft -= damage;
            animationPlayerUI.Play("damageReceiver");

        }
        else
        {
            if (animation == "dieInTentacles")
            {
                scActions.die(true);
            }
            else
            {

                scActions.die();

            }
        }

    }

    public async void ariseTentaclesInsideBody()
    {
        tentaclesInsideBody.Visible = true;
        AnimationPlayer[] animTentacles = new AnimationPlayer[tentaclesInsideBody.GetChildCount()];
        for (int i = 0; i < tentaclesInsideBody.GetChildCount(); i++)
        {
            animTentacles[i] = tentaclesInsideBody.GetChild(i).GetNode<AnimationPlayer>("tentacles/AnimationPlayer");
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");


            animTentacles[i].Play("fadeOutSwing", -1, 15f);
            //animTentacles[i].GetAnimation("fadeOutSwing").Loop = true;
        }
        
    }

    private void restartScene()
    {
        GetTree().ReloadCurrentScene();
    }

    private bool verifyInputs()
    {
        bool isInputMovimentPressed = false;


        if (Input.IsActionPressed("moveFront"))
        {
            scActions.direction += Transform.basis.z;
            isInputMovimentPressed = true;

        }
        else if (Input.IsActionPressed("moveBack"))
        {
            scActions.direction -= Transform.basis.z;
            isInputMovimentPressed = true;
        }

        if (Input.IsActionPressed("moveLeft"))
        {
            scActions.direction += Transform.basis.x;
            isInputMovimentPressed = true;

        }
        else if (Input.IsActionPressed("moveRight"))
        {
            scActions.direction -= Transform.basis.x;
            isInputMovimentPressed = true;

        }
        if (Input.IsActionJustReleased("moveFront") || Input.IsActionJustReleased("moveBack") || Input.IsActionJustReleased("moveLeft") || Input.IsActionJustReleased("moveRight"))
        {
            isInputMovimentPressed = false;
            playerState.CurrentStatePlayer = playerState.STATE_PLAYER.STOP;
        }

        return isInputMovimentPressed;
    }

    public override void _Process(float delta)
    {
  

        Node nodeGround = (Node)rayToGround.GetCollider();

        if (nodeGround != null)

        {
            playerState.playerHasFloor = true;
           
            Vector3 hitPoint = rayToGround.GetCollisionPoint();

             if (((StaticBody)nodeGround).CollisionLayer == 32)
            {
                playerIsOutRoad(hitPoint);

            }
            else
            {

                playerReturnToRoad();
            }

        }
        else
        {
            playerState.playerHasFloor = false;
        }

        if (verifyInputs() && playerState.playerHasFloor == true)
        {
            playerState.CurrentStatePlayer = playerState.STATE_PLAYER.WALK;
        }

    }

}


