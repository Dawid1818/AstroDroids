//Code for GamemodeScreenGum
using AstroDroids.Components.Controls;
using Gum;
using Gum.Converters;
using Gum.DataTypes;
using Gum.GueDeriving;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary.Graphics;
using System.Linq;
namespace AstroDroids.Screens;
partial class GamemodeScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("GamemodeScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named GamemodeScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new GamemodeScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(GamemodeScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("GamemodeScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public ButtonGlow ReturnBtn { get; protected set; }
    public ButtonGlowGameMode TutorialBtn { get; protected set; }
    public ButtonGlowGameMode StoryBtn { get; protected set; }
    public ButtonGlowGameMode BossRushBtn { get; protected set; }

    public GamemodeScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public GamemodeScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        ReturnBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ReturnBtn");
        TutorialBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlowGameMode>(this.Visual,"TutorialBtn");
        StoryBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlowGameMode>(this.Visual,"StoryBtn");
        BossRushBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlowGameMode>(this.Visual,"BossRushBtn");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.BossRushBtn.Text = GumService.Default.LocalizationService.Translate("T_BossRush");
        this.ReturnBtn.Text = GumService.Default.LocalizationService.Translate("T_Return");
        this.StoryBtn.Text = GumService.Default.LocalizationService.Translate("T_StoryMode");
        this.TutorialBtn.Text = GumService.Default.LocalizationService.Translate("T_Tutorial");
    }
    partial void CustomInitialize();
}
