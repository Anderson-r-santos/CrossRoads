using Godot;
using System;

public class secretes : Spatial
{
    private void collectOneSecret(Node body)
    {
        GetTree().Root.GetNode<Actions>("rootTree/Player/Actions").getCollectible();
        QueueFree();
    }
}
