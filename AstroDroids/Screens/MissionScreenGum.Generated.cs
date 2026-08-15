//Code for MissionScreenGum
using AstroDroids.Components.Controls;
using AstroDroids.Components.Custom;
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
partial class MissionScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("MissionScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named MissionScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new MissionScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(MissionScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("MissionScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public ButtonGlow ReturnBtn { get; protected set; }
    public ButtonGlow PlayBtn { get; protected set; }
    public LevelCard Level1Card { get; protected set; }
    public LevelCard Level2Card { get; protected set; }
    public LevelCard Level3Card { get; protected set; }
    public LevelCard Level4Card { get; protected set; }
    public LevelCard Level5Card { get; protected set; }
    public ButtonIconGlow NextLevelBtn { get; protected set; }
    public ButtonIconGlow PrevLevelBtn { get; protected set; }

    public MissionScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public MissionScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        ReturnBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ReturnBtn");
        PlayBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"PlayBtn");
        Level1Card = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<LevelCard>(this.Visual,"Level1Card");
        Level2Card = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<LevelCard>(this.Visual,"Level2Card");
        Level3Card = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<LevelCard>(this.Visual,"Level3Card");
        Level4Card = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<LevelCard>(this.Visual,"Level4Card");
        Level5Card = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<LevelCard>(this.Visual,"Level5Card");
        NextLevelBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonIconGlow>(this.Visual,"NextLevelBtn");
        PrevLevelBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonIconGlow>(this.Visual,"PrevLevelBtn");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.PlayBtn.Text = GumService.Default.LocalizationService.Translate("T_Play");
        this.ReturnBtn.Text = GumService.Default.LocalizationService.Translate("T_Return");
    }
    partial void CustomInitialize();
}
