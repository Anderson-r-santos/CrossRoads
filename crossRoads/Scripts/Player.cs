using Godot;

public class Player : KinematicBody
{

    public Actions scActions;
    public bool isRunning = false;

    private Timer timerIncreaseTemperature;  // tempo que a temperatuda corporal decai
    private Timer timerDecreaseTemperature;
    public Timer timerUmbrellaRay; //tempo em que fica ativo o ray que detecta o inmigo
    private Timer timerRegenLife;
    private int amountDamageLeft = 4;
    private int lifeTotal;

    //movimentacao
    public float currentGravity;
    public float currentMoveSpeed;
    public float moveSpeed = 50f;
    public float runSpeed = 55f;
    public float fallGravity;
    public float flyGravity;
    public float gravity = 9.8f;
    public float defaultGravity = 9.8f;
    public int stamina = 4;
    public bool playerHasFloor = true;
    private Vector3 direction;

    private OmniLight lightUmbrella;

    private RayCast rayToGround;
    public RayCast rayCamera;
    private int bodyTemperature = 8;

    private cenario scSenario;

    private AudioStreamPlayer2D outRoad;
    private AudioStreamPlayer2D crackedGround;
    private AudioStreamPlayer2D lightBlink;

    private AnimationPlayer animationPlayer;
    private AnimationPlayer AnimationPlayerActions;
    private AnimationPlayer animationPlayerUI;

    public playerState scPlayerState;

    //briefCase
    private Label countDegrees;
    private MeshInstance verticalBarTemperature;
    private float sizeBarChunks;

    public int defaultAmountDegrees = 1;
    public int currentAmountDegrees;

    private Spatial tentaclesInsideBody;


    public override void _Ready()
    {

        GetNode<AnimationPlayer>("meshPlayer/AnimationPlayerTemperature").Play("RESET");   


        currentMoveSpeed = moveSpeed;
        defaultGravity = gravity * 5;
        currentGravity = defaultGravity;
        fallGravity = defaultGravity * 10;
        flyGravity = -(defaultGravity * 10);


        lifeTotal = amountDamageLeft;

        scActions = GetNode<Actions>("Actions");


        AnimationPlayerActions = GetNode<AnimationPlayer>("AnimationPlayerActions");
        animationPlayer = GetNode<AnimationPlayer>("meshPlayer/AnimationPlayer");
        animationPlayerUI = GetNode<AnimationPlayer>("Camera/PlayerUI/AnimationPlayerUI");

        countDegrees = GetNode<Label>("meshPlayer/Armature/Skeleton/BoneAttachBriefCase/Viewport/Label");

        verticalBarTemperature = GetNode<MeshInstance>("meshPlayer/Armature/Skeleton/BoneAttachBriefCase/barTemperatureIndicator");

        rayToGround = GetNode<RayCast>("rayToGround");
        //rayAttack = GetNode<RayCast>("meshPlayer/RayCast");
        lightUmbrella = GetNode<OmniLight>("umbrellaLight");
        timerIncreaseTemperature = GetNode<Timer>("TimerIncreaseTemperature");
        timerDecreaseTemperature = GetNode<Timer>("TimerDecreaseTemperature");
        timerUmbrellaRay = GetNode<Timer>("TimerUmbrellaRay");
        timerRegenLife = GetNode<Timer>("TimerRegenLife");

        scSenario = GetParent().GetNode<cenario>("cenarioInicial");
        outRoad = GetTree().Root.GetNode<AudioStreamPlayer2D>("rootTree/cenarioInicial/outRoad");
        crackedGround = GetTree().Root.GetNode<AudioStreamPlayer2D>("rootTree/cenarioInicial/crackedGround");
        lightBlink = GetNode<AudioStreamPlayer2D>("Sounds/lightBlink");

        tentaclesInsideBody = GetNode<Spatial>("tentaclesInsidePos");

        scPlayerState = GetNode<playerState>("playerState");
        rayCamera = GetNode<RayCast>("Camera/RayCast");

        sizeBarChunks = verticalBarTemperature.Scale.y / amountDamageLeft - 0.01f; // 0.1 so pra escala nao ser 0 e bugar
        currentAmountDegrees = defaultAmountDegrees;

        tentaclesInsideBody.Visible = false;
        showControlsUI();

    }


    private async void showControlsUI()
    {
        Control ControlsUI = GetNode<Control>("Camera/PlayerUI/ControlsUI");
        ControlsUI.Visible = true;
        await(ToSignal(GetTree().CreateTimer(4f),"timeout"));
        ControlsUI.QueueFree();

    }


    /// <summary>
    /// muda visualmente a qnt de graus atual da maleta
    /// </summary>
    /// <param name="amountDegrees"></param>
    /// <param name="increase"></param>
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
    /// <summary>
    /// faz a barra indicadora que esta na maleta mudar de cor do verde ao vermelho ao ficar na chuva
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// faz a barra indicadora que esta na maleta mudar de cor do vermelho ate o verde ao se proteger da chuva
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// ao fechar o guarda chuvas e se expor a chuva,a temperatura corporal do jogador abaixa
    /// </summary>
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
    /// <summary>
    /// ao abrir o guarda chuvas aumenta a temperatura do jogador
    /// </summary>
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

    /// <summary>
    /// verifica se tem algum inimigo na mira
    /// </summary>
    public void verifyRayCamera()
    {

        if (rayCamera.IsColliding())
        {
            Node node = (Node)rayCamera.GetCollider();
            if (node.Name == "enemyCollisor")
            {

                Enemy scEnemy = node.GetParent().GetNode<Enemy>("../.");

                scEnemy.takeDamage();
            }


        }

    }
    /// <summary>
    /// ao chegar em uma area exibe um video dando dicas
    /// </summary>
    /// <param name="video"></param>
    public void showTip(VideoStreamWebm video,string titleVideo)
    {
        GetNode<Tips>("Camera/PlayerUI/TipVideo").showTipsVideo(video,titleVideo);
    }

    /// <summary>
    /// ao chegar em uma area exibe uma mensagem na parte inferiro da tela
    /// </summary>
    /// <param name="message"></param>
    public void showTip(string message)
    {
        GetNode<Tips>("Camera/PlayerUI/TipVideo").showTipsText(message);
    }


    /// <summary>
    /// qnd o inimigo estiver perto,a luz do guarda chuva reage
    /// </summary>
    public void enemyClose()
    {
        AnimationPlayerActions.Play("lightBlink", -1, 2);
        lightBlink.Play();
    }
    /// <summary>
    /// idetifica se o player saiu da pista
    /// </summary>
    /// <param name="hitPoint">o ponto que o jogador esta,para fazer surgir as mãos da terra</param>
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
    /// <summary>
    /// chamada apos o jogador sair fora da pista para a parte de terra e dps retornar para a pista
    /// </summary>
    private void playerReturnToRoad()
    {
        crackedGround.Stop();
        outRoad.Stop();
        // AnimationPlayerActions.Stop();
        scSenario.playerIsNotInGround();
        AnimationPlayerActions.Stop();
    }
    /// <summary>
    /// todas vez que o jogador receber dano do inimigo,terreno,temperatura 
    /// </summary>
    /// <param name="animation">animação de dano que o jogador recebeu</param>
    /// <param name="damage">quantidade de dano causado pelo invocador</param>
    /// <param name="animSpeed">velocidade da animação</param>
    public void damageReceived(string animation, int damage = 1, float animSpeed = 1)
    {
        if (!scActions.playerDie)
        {
            if (animation != "temperature")
            {

                animationPlayer.Play(animation, -1, animSpeed);

            }

            if (amountDamageLeft - damage > 0)
            {
                amountDamageLeft -= damage;
                animationPlayerUI.Play("damageReceiver");
                timerRegenLife.Start();

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

    }

    /// <summary>
    /// faz os tentaculos aparecer em posições diferentes,e executa as animações de formas NÂO sincronizadas
    /// </summary>
    /// <returns></returns>
    public async void ariseTentaclesInsideBody()
    {
        if(!scActions.playerDie)
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

    }

    /// <summary>
    /// 
    /// </summary>
    private void restartScene()
    {

        GetTree().ReloadCurrentScene();
         
    }

    /// <summary>
    /// depois do jogador tomar dano,aguarda 4 seg e regenera a vida total
    /// </summary>
    private void regenLife()
    {
        if(amountDamageLeft < lifeTotal)
        {
            amountDamageLeft = lifeTotal;
        }
    }
    /// <summary>
    ///verifica se as teclas de movimentação estao sendo pressionada
    /// </summary>
    /// <param name="isRunning">se o jogador esta no stado de correr</param>
    /// <returns></returns>
    private bool verifyInputs(ref bool isRunning)
    {
        bool isInputMovimentPressed = false;

        if (Input.IsActionPressed("moveFront"))
        {
            direction += Transform.basis.z;
            isInputMovimentPressed = true;

        }
        else if (Input.IsActionPressed("moveBack"))
        {
            direction -= Transform.basis.z;
            isInputMovimentPressed = true;
        }

        if (Input.IsActionPressed("moveLeft"))
        {
            direction += Transform.basis.x;
            isInputMovimentPressed = true;

        }
        else if (Input.IsActionPressed("moveRight"))
        {
            direction -= Transform.basis.x;
            isInputMovimentPressed = true;

        }
        if (Input.IsActionPressed("run"))
        {
            isRunning = true;
            playerState.CurrentStatePlayer = playerState.STATE_PLAYER.RUN;
        }
        if (Input.IsKeyPressed((int)KeyList.Escape))
        {

            GetNode<PauseGame>("pauseGame").setPauseGame();
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
            playerHasFloor = true;
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
            playerHasFloor = false;
        }

        if (playerState.CurrentStatePlayer != playerState.STATE_PLAYER.RUN)
        {
            if (stamina < 4)
            {
                scActions.recoverStamina();

            }
        }
    }


    public override void _PhysicsProcess(float delta)
    {

        if (!scActions.playerDie && !scActions.isWaitTime)
        {
            isRunning = false;

            if (verifyInputs(ref isRunning))
            {
                if (!isRunning && playerState.CurrentStatePlayer != playerState.STATE_PLAYER.ATTACK)
                {
                    playerState.CurrentStatePlayer = playerState.STATE_PLAYER.WALK;

                }
                else if (!playerHasFloor)
                {
                    currentMoveSpeed = moveSpeed;
                }

            }
            direction.y = 0;
            direction.z *= currentMoveSpeed * delta;
            direction.x *= currentMoveSpeed * delta;

            direction.y -= currentGravity * delta;
            MoveAndSlide(direction, Vector3.Up);

        }

    }
}


