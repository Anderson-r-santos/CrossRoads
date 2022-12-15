using Godot;
using System;


public class road_tentacles : Spatial
{
    private AnimationPlayer [] animTenTacles;
    private AnimationNodeStateMachinePlayback stateMachine;
    private AnimationTree animTree;
    private KinematicBody player;


    private enum stateTentacles{
      WAIT,AIM,ATTACK
    }
    private stateTentacles currentState;

    public override void _Ready()
    {
        player = GetTree().Root.GetNode<KinematicBody>("rootTree/Player");
        Spatial allTentacles = GetNode<Spatial>("tentaclesChild");
        animTenTacles = new AnimationPlayer[allTentacles.GetChildCount()];
        for(int i = 0 ; i < allTentacles.GetChildCount() ; i++)
        {

          Spatial currentTentacle = (Spatial)allTentacles.GetChild(i);
          animTenTacles[i] = currentTentacle.GetNode<AnimationPlayer>("AnimationPlayer");
          animTenTacles[i].Play("swing");
          GD.Print("name Tentacles " + currentTentacle.Name);

        }

        currentState = stateTentacles.AIM;
    }

    private  async void wait ()
    {
        
 
      foreach(AnimationPlayer animPlayerTentacles in animTenTacles)
      {
        await ToSignal(GetTree().CreateTimer(0.5f),"timeout");
        if(!animPlayerTentacles.IsPlaying()){
            animPlayerTentacles.Play("swing",-1,(float)GD.RandRange(1.0,2.0));
        }
      }
    }


    
    private void attack(AnimationPlayer animPlayer)
    {
      animPlayer.Play("attack");
    }



    private void aim()
    {
      float shorterDistance = 1000;
      //Spatial shorterTentacle = null;

      foreach(AnimationPlayer animPlayerTentacles in animTenTacles)
      {
        Spatial tentacleNode = (Spatial)animPlayerTentacles.GetParent();
        float currentDistance = tentacleNode.Transform.origin.DistanceTo(player.Transform.origin);

        if(currentDistance <= shorterDistance)
        {
          shorterDistance = currentDistance;
          animPlayerTentacles.Play("aiming");
          tentacleNode.LookAt(player.Transform.origin,Vector3.Up);
          tentacleNode.RotateObjectLocal(Vector3.Up,3.14f);
          //attack(animPlayerTentacles);
        }
     }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
       switch(currentState)
      {
        case stateTentacles.WAIT:
            wait();
        break; 

        case stateTentacles.AIM:
          aim();
        break;
        case stateTentacles.ATTACK:
          //attack();
        break;


      }

 }
}
