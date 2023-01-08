using Godot;
using System;

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

    private void playerEnteredArea(Node body)
    {
        if(!varMsgInLanguageFile.Empty()){
           
            scPlayer.showTip(Tr(varMsgInLanguageFile));
       
       }else{
            
            scPlayer.showTip(videoTip);

        }
        deleteArea();
    }
    private void deleteArea()
    {
        QueueFree();
    }

}
