using Godot;
using System;

public class Enemy : Spatial
{
    private Player scPlayer;
    private KinematicBody targetPlayer = null;
    private const float moveSpeed = 10f;
    public bool playerInsideArea = false;
    private PathFollow pathPatrol;
    private Path path;
    private RayCast ray;
    private Area enemyPatrolArea;
    private MeshInstance meshEnemy;
    private AnimationPlayer enemyAnimPlayer;
    private Particles damageParticles;
    private int lifeEnemy = 3;
    private float defautPathOffset;
    

    private Vector3 closePoint;

    public enum EnemyState{
        FOLLOWPLAYER,PATROL
    }
    private EnemyState currentState;

    public EnemyState EnemyState_G_S
    {
        get{return currentState;}
        set{
            currentState = value;
        }
    }
   
    public override void _Ready()
    {
        scPlayer = GetTree().Root.GetNode<Player>("rootTree/Player");
        pathPatrol = GetParent().GetNode<PathFollow>(".");
        path = GetParent().GetParent().GetNode<Path>(".");
        currentState = EnemyState.PATROL;
        ray = GetNode<RayCast>("mesh/RayCast");
        enemyPatrolArea = GetNode<Area>("../../../Area");
        enemyPatrolArea.Connect("body_entered",this,"playerEnteredArea");
        enemyPatrolArea.Connect("body_exited",this,"playerExitedArea");
        meshEnemy = GetNode<MeshInstance>("mesh");

        enemyAnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        damageParticles = GetNode<Particles>("mesh/ParticlesDamage");
        defautPathOffset = pathPatrol.Offset;
    }



    public void takeDamage()
    {
        LookAt(scPlayer.Transform.origin,Vector3.Up);
        GD.Print("inimigo acertado");
        if(lifeEnemy > 0)
        {
            enemyAnimPlayer.Play("blink",-1,3f);
            damageParticles.Emitting = true;
            lifeEnemy--;
        }else{
            QueueFree();
        }
      

    }
  

    private void playerExitedArea(Node player)
    {
        targetPlayer = null;
        currentState = EnemyState.PATROL;
        MeshInstance meshEnemy = GetNode<MeshInstance>("mesh");
        meshEnemy.Translate(Vector3.Zero);
        playerInsideArea = false;
    }

    private void followPlayer(float delta)
    {
      
        if(targetPlayer != null && playerState.currentStateUmbrella == playerState.STATE_UMBRELLA.UP_UMBRELLA)
        {
            Vector3 playerCenter = targetPlayer.GlobalTransform.origin;
            playerCenter.y +=4;
            meshEnemy.LookAt(playerCenter,Vector3.Up);
            meshEnemy.Translate(Vector3.Forward * delta * moveSpeed);

            if(ray.IsColliding())
            {
               KinematicBody player = (KinematicBody) ray.GetCollider();
               Player scPlayer =  player.GetNode<Player>(".");
               scPlayer.damageReceived("tentacleInsideDamage");
               
               playerState.CurrentStatePlayer = playerState.STATE_PLAYER.WAIT_TIME;
               scPlayer.ariseTentaclesInsideBody();
               QueueFree();
           }
  
        }else{
            //pathPatrol.Offset = defautPathOffset;
             currentState = EnemyState.PATROL;
        }
    }

    private void patrol(float delta)
    {

        if(playerInsideArea){
            currentState = EnemyState.FOLLOWPLAYER;
            
        }

            pathPatrol.Offset += delta * moveSpeed;
        


    }

        //quando o player entra no campo de visao do inimigo
    public void playerEnteredArea(Node player)
    {
        if(player.Name == "Player")
        {
            playerInsideArea = true;
            scPlayer.enemyClose();
            GD.Print("body enter area " + player.Name);
            targetPlayer = (KinematicBody) player;
            currentState = EnemyState.FOLLOWPLAYER;
        
        }

    }

    private void deleteEnemy()
    {
        QueueFree();
    }

    public override void _Process(float delta)
    {

        switch(currentState)
        {
            case EnemyState.FOLLOWPLAYER:
                followPlayer(delta);
            break;

             case EnemyState.PATROL:
                patrol(delta);
            break;

        }
    }
}
