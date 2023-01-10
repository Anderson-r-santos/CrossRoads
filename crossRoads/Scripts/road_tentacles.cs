using Godot;

/// <summary>
/// um tipo diferen de imigo que causa morte instantâne  e nao pode ser atingido
/// </summary>
public class road_tentacles : Spatial
{
    private AnimationPlayer [] animTenTacles;
    private RayCast [] rayCastTentacles;
    private KinematicBody player;
    private Player scPlayer;
    private bool playerIsInsideTentacleArea = false;

    private enum stateTentacles{
      WAIT,ATTACK
    }
    private stateTentacles currentState;
    private AnimationPlayer currentTentacleAttack = null;


    public override void _Ready()
    {
        player = GetTree().Root.GetNode<KinematicBody>("rootTree/Player");
        scPlayer = player.GetNode<Player>(".");


        Spatial allTentacles = GetNode<Spatial>("tentaclesChild");
        animTenTacles = new AnimationPlayer[allTentacles.GetChildCount()];
        rayCastTentacles = new RayCast[allTentacles.GetChildCount()];
        for(int i = 0 ; i < allTentacles.GetChildCount() ; i++)
        {

          Spatial currentTentacle = (Spatial)allTentacles.GetChild(i);
          animTenTacles[i] = currentTentacle.GetNode<AnimationPlayer>("AnimationPlayer");
          rayCastTentacles[i] = currentTentacle.GetNode<RayCast>("Armature/Skeleton/BoneAttachment/RayCast");
          animTenTacles[i].Play("swing",-1,(float)GD.RandRange(2.0,3.0));
          animTenTacles[i].GetAnimation("swing").Loop = true;
          GD.Print("name Tentacles " + currentTentacle.Name);

        }

    }
    /// <summary>
    /// espera uma quantida de segundos aleatórios para executar a animação de "swing" dos tentaculos,para fazerem movimentos nao sincronizados
    /// </summary>
    /// <param name="animTentacles"></param>
    /// <returns></returns>
    private  async void wait (AnimationPlayer animTentacles)
    {

      animTentacles.Play("swing",0,(float)GD.RandRange(1.0,2.0));
      if(playerIsInsideTentacleArea){
        await ToSignal(GetTree().CreateTimer(2f),"timeout");
        currentState = stateTentacles.ATTACK;
      }
    }


    /// <summary>
    /// attacka o jogador ao entrar em sua áre
    /// </summary>
    /// <returns></returns>
    private async void attack()
    {

      for(int i = 0 ; i < animTenTacles.Length ; i++)
      {

        //await ToSignal(GetTree().CreateTimer(1.5f),"timeout");
        int randomIdxTentacle = (int)GD.RandRange(0,animTenTacles.Length);
        lookAtPlayer((Spatial)animTenTacles[i].GetParent());
        currentTentacleAttack = animTenTacles[i];
        animTenTacles[i].Play("attack",-1,2f);
        await ToSignal(GetTree().CreateTimer(1f),"timeout");
        wait(animTenTacles[i]);

      }

    
    }


    // private void aim()
    // {
    //   float shorterDistance = 1000;
    //   //Spatial shorterTentacle = null;

    //   foreach(AnimationPlayer animPlayerTentacles in animTenTacles)
    //   {
    //     Spatial tentacleNode = (Spatial)animPlayerTentacles.GetParent();
    //     float currentDistance = tentacleNode.Transform.origin.DistanceTo(player.Transform.origin);

    //     if(currentDistance <= shorterDistance)
    //     {
    //       shorterDistance = currentDistance;
    //       animPlayerTentacles.Play("aiming");
    //       lookAtPlayer(tentacleNode);

    //       currentState = stateTentacles.ATTACK;
    //     }
    //  }
    // }

    private void lookAtPlayer(Spatial tentacleNode)
    {
        tentacleNode.LookAt(player.Transform.origin,Vector3.Up);
        tentacleNode.RotateObjectLocal(Vector3.Up,3.14f);
    }



  private void playerEnteredInMyArea(Node body)
  {
    playerIsInsideTentacleArea = true;
    GD.Print("o body que entrou na area do tentacles foi " + body.Name);
    attack();
  }
  
  private void playerExitedInMyArea(Node body)
  {
    playerIsInsideTentacleArea = false;
  }


  /// <summary>
  /// cada tentaculo tem um raycast na ponta,esse método verifica se eles colidiram com o jogado
  /// </summary>
  /// <returns></returns>
  private bool verifyRayCollision()
  {
    bool hitPlayer = false;
    if(currentTentacleAttack != null && currentTentacleAttack.CurrentAnimation == "attack")
    {
      foreach(RayCast ray in rayCastTentacles)
      {
        if(ray.IsColliding())
        {
          GD.Print("rayCast collidiu com o body " + ray.GetCollider() != null ? ((KinematicBody)ray.GetCollider()).Name : "");
            hitPlayer = true;
            break;
        }
      }

    }
    return hitPlayer;
  }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
    if(playerIsInsideTentacleArea )
    {
      if(verifyRayCollision())
      {
        playerState.CurrentStatePlayer = playerState.STATE_PLAYER.RECEIVE_DAMAGE_TENTACLE_GROUND;
      }
    }

 }
}
