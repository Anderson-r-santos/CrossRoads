using Godot;
using System;

/// <summary>
/// responsável pela tradução da interface,menus,de acordo com o padrão da godot(arquivo .csv)
/// </summary>
public class language_game_UI : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    [Export]
    string [] pathToTextElement;
    
    [Export]
    string [] textToTranslate;
    public override void _Ready()
    {
        setTextInTextField();
    }
    
    public void setLanguage(string language)
    {
        TranslationServer.SetLocale(language);
        GetNode<Control>("../../languageMenu").Visible = false;
        setTextInTextField();

    }

    private void setTextInTextField()
    {
       for(int i = 0 ; i < textToTranslate.Length ; i ++)
       {
        Button button = GetNode(pathToTextElement[i]) as Button;
        Label label = GetNode(pathToTextElement[i]) as Label;
        if(button != null)
        {
            button.Text = Tr(textToTranslate[i]);
        }else if(label != null){
           label.Text = Tr(textToTranslate[i]);
        }
       }
    }


}
