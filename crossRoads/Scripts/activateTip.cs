using Godot;
using System;

/// <summary>
/// usado por uma àrea que lança uma dica na tela por video ou messagem
/// </summary>
public class activateTip : Spatial
{
    [Export]
    private VideoStreamWebm videoTip;
    [Export]
    private String varMsgInLanguageFile; //o nome da string que aponta para uma celula no arquivo de tradução


    private Player scPlayer;
    public override void _Ready()
    {

        scPlayer =  GetTree().Root.GetNode<Player>("rootTree/Player");
        Area thisArea = GetNode<Area>("Area");

        thisArea.Connect("body_entered",this,"playerEnteredArea");

    }
    /// <summary>
    /// exibe uma dica de video quando o jogador entra na área,caso a messagem de dica for vazia
    /// </summary>
    /// <param name="body"></param>
    private void playerEnteredArea(Node body)
    {
        if(!varMsgInLanguageFile.Empty()){
           
            scPlayer.showTip(Tr(varMsgInLanguageFile));
       
       }else{
            
            scPlayer.showTip(videoTip);

        }
        deleteArea();
    }

    /// <summary>
    /// após exibir a dica na tela deleta a área de dica
    /// </summary>
    private void deleteArea()
    {
        QueueFree();
    }

}
