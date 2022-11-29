using Godot;
using System;

public class hand : RigidBody
{

    public override void _Ready()
    {
        
    }

    private void deleteThisInstance()
    {
        GetTree().Root.GetNode<cenario>("rootTree/cenarioInicial").deleteHand(this);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
