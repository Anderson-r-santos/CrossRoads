using Godot;
using System;

/// <summary>
/// mão que surge no terreno quando o jogador sai da pista
/// </summary>
public class hand : Spatial
{
        private RayCast ray;
    public override void _Ready()
    {
        ray = GetNode<RayCast>("Armature/Skeleton/BoneAttachment/KinematicBody/RayCast");
    }
    /// <summary>
    /// deleta essa instância de mão,depois de terminar o tempo definido pelo nó contido em sua cena
    /// </summary>
    private void deleteThisInstance()
    {
        GetTree().Root.GetNode<cenario>("rootTree/cenarioInicial").deleteHand(this);
    }

 public override void _Process(float delta)
 {

    if(ray != null)
    {
        if(ray.IsColliding())
        
        {
           
            KinematicBody player = (KinematicBody)ray.GetCollider();
            if(player.Name == "Player"){
                 GD.Print("O RAY da mao esta colidindo");
             //   ray = null;
                playerState.CurrentStatePlayer = playerState.STATE_PLAYER.RECEIVE_DAMAGE_HAND_GROUND;
            }
        }
    }
 }
}

