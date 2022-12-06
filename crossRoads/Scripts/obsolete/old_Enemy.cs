using Godot;
using System;

[Obsolete]
public class old_Enemy : Spatial
{
    private float moveSpeed = 15f;
    private Spatial player;

    private playerState scPlayerState;
    private float delta;

    private bool canReAttack = false;
    public bool playerInRangeToAttack = false;
    private int life = 4;

    private Particles particlesDamage;
    private AnimationPlayer animationPlayer;
    private RayCast rayToAttack;
    private Timer timerToNextAttack;    //tempo que o inimigo vai em linha reta dps de atacar

    private AudioStreamPlayer3D soundEnemyScream;

    public enum STATE_ENEMY
    {
        NONE,MOVE,ATTACK,SPAWN,FLYCIRCLE
    }
    public STATE_ENEMY currentStateEnemy = STATE_ENEMY.NONE;

    public Spatial currentSlotThisEnemy = null;

    private old_EnemyHandler enemyHandler;
    public old_EnemyHandler EnemyHandlerPropertie
    {
        get{return enemyHandler;}
        set{enemyHandler = value;}
    }

    public override void _Ready()
    {
        player = GetTree().Root.GetNode<Spatial>("rootTree/Player/targetToEnemy");
        scPlayerState = GetTree().Root.GetNode<playerState>("rootTree/Player/playerState");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        rayToAttack = GetNode<RayCast>("mesh/RayCast");

        timerToNextAttack = GetNode<Timer>("Timer");
        timerToNextAttack.Connect("timeout",this,"areaAttackExited");
        particlesDamage = GetNode<Particles>("ParticlesDamage");

        soundEnemyScream = GetNode<AudioStreamPlayer3D>("enemyScream");
    }

 public override void _Process(float delta)
 {
    this.delta = delta;

    switch(currentStateEnemy)
    {
        case STATE_ENEMY.MOVE:
            if(enemyHandler.playerInsideArea && playerState.currentStateUmbrella == playerState.STATE_UMBRELLA.UP_UMBRELLA){
                Vector3 playerPos = player.GlobalTransform.origin;
                move(playerPos,moveSpeed);
            }
        break;
        case STATE_ENEMY.ATTACK:
            attackMode();
        break;
        case STATE_ENEMY.SPAWN:
            if(currentSlotThisEnemy != null)
            {

                move(currentSlotThisEnemy.GlobalTransform.origin,moveSpeed * 2.5f);

            }

        break;
        case STATE_ENEMY.FLYCIRCLE:

        break;

    }

 }

 //qnd o imigo ataca o jogador entrando na area em volta do jogador
 public void areaAttackEntered()
 {
    // GD.Print("entrou uma area aki com o nome de " + area.Name);
    if(currentStateEnemy == STATE_ENEMY.MOVE){
        currentStateEnemy = STATE_ENEMY.ATTACK;
        timerToNextAttack.Start();
        canReAttack = false;
    }
    //scPlayer.enemyClose();
 } 

 //qnd o inimigo sai de dentro da area envolta do player dps de um ataque do inimigo
 public void areaAttackExited()
 {
    playerInRangeToAttack = false;
    canReAttack = true;
 }

 private void attackMode()
 {
  
  //if(rayToAttack.IsColliding()){
    KinematicBody node = (KinematicBody) rayToAttack.GetCollider();
    if(node !=null && node.Name == "Player")
    {
        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.RECEIVE_DAMAGE_BASIC_ENEMY;
    }
    animationPlayer.PlaybackSpeed = 2;
    animationPlayer.Play("blink");
    Translate(Vector3.Forward * delta * moveSpeed*4);
 // }


 }

 public void takeDamage( ref RayCast  rayUmbrella)
 {
     if(life > 0)
     {
        GD.Print("INIMIGO LEVOU DANOO");
        life -=1;
        rayUmbrella.Enabled = false;
        animationPlayer.Play("damageTake");
        particlesDamage.Restart();
        particlesDamage.Emitting = true;
        soundEnemyScream.Play();
     }else
     {
         die();
     }
 }
 private void die()
 {
     GD.Print("INIMIGO Mooooorto");
     QueueFree();
 }
 public Spatial verifySlot()
 {

     Spatial currentSlot = null;
    for(int i=0;i < enemyHandler.slotList.Count;i++)
    {
        old_EnemyHandler.Slot currentListItem = enemyHandler.slotList[i];
        old_EnemyHandler.Slot newListItem = new old_EnemyHandler.Slot();

        if(currentListItem.isEmpty == true)
        {

            
            currentListItem.isEmpty = false;
            newListItem.isEmpty = false;
            newListItem.spatialPosition = currentListItem.spatialPosition;
            enemyHandler.slotList[i] = newListItem;
            //enemyHandler.floatCircle.AddChild(this);
            //currentSlotThisEnemy = currentListItem.spatialPosition;
            return currentListItem.spatialPosition;

        }
    }
    return currentSlot;
 }

 private void move(Vector3 positionToFollow, float speed)
 {
    
    LookAt(positionToFollow,Vector3.Up);
    Translate(Vector3.Forward * delta * speed);
    animationPlayer.Stop();
    
 }
}

