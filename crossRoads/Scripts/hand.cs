using Godot;
using System;

/// <summary>
/// mão que surge no terreno quando o jogador sai da pista
/// </summary>
public class hand : Spatial
{

    public override void _Ready()
    {
        
    }
    /// <summary>
    /// deleta essa instância de mão,depois de terminar o tempo definido pelo nó contido em sua cena
    /// </summary>
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
