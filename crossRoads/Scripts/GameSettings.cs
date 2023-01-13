using Godot;
using System;

/// <summary>
/// mostra algumas opções de configurações para o jogo,como resolução,tela cheia...
/// </summary>
public class GameSettings : Control
{
    private OptionButton resolutionOptions;
    private CheckBox fullScreenOption;

    public override void _Ready()
    {
        resolutionOptions = GetNode<OptionButton>("VBoxContainer/OptionButton");
        fullScreenOption = GetNode<CheckBox>("VBoxContainer/CheckBox");
    }

    private void setFullScreen(bool fullScreenMode)
    {
        Viewport mainViewport = GetTree().Root;
        mainViewport.SizeOverrideStretch = true;
        if(fullScreenMode)
        {
            OS.WindowFullscreen = true;
        }
        else{
            OS.WindowFullscreen = false ;
        }

    }
    private void changeResolution(int id)
    {
        Viewport mainViewport = GetTree().Root;
        mainViewport.SizeOverrideStretch = true;
        
        
        switch(id)
        {
            case 0:
                mainViewport.Size = new Vector2(1920,1080);
            break;
            case 1:
                mainViewport.Size = new Vector2(1280,768);
            break;
            case 2:
                mainViewport.Size = new Vector2(1024,768);
            break;
        }
    }
}
