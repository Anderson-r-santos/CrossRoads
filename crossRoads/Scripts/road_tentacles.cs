using Godot;
using System;
using System.Collections.Generic;

public class road_tentacles : Spatial
{
    private AnimationPlayer [] animTenTacles = new AnimationPlayer[4];
    private AnimationNodeStateMachinePlayback stateMachine;
    private AnimationTree animTree;

   

    public override void _Ready()
    {
        Spatial allTentacles = GetNode<Spatial>("tentaclesChild");
        for(int i = 0 ; i < allTentacles.GetChildCount() ; i++)
        {
          Spatial currentTentacle = (Spatial)allTentacles.GetChild(i);
          animTenTacles[i] = currentTentacle.GetNode<AnimationPlayer>("./AnimationPlayer");
          GD.Print("name Tentacles " + currentTentacle.Name);

        }
    }

    private  async void playAsyncAnimation ()
    {
        
        
      foreach(AnimationPlayer animPlayerTentacles in animTenTacles)
      {
        await ToSignal(GetTree().CreateTimer(0.5f),"timeout");
        if(!animPlayerTentacles.IsPlaying()){
            animPlayerTentacles.Play("swing001",-1,(float)GD.RandRange(1.0,2.0));
        }
      }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
    //animTree.
  // animTenTacles.Play();
    playAsyncAnimation();

 }
}
