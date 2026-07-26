//Code for GameScreenGum
using AstroDroids.Components.Controls;
using AstroDroids.Components.Elements;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using System.Linq;
namespace AstroDroids.Screens;
partial class GameScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("GameScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named GameScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new GameScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(GameScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("GameScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public ContainerRuntime BottomPanel { get; protected set; }
    public NineSliceRuntime BottomPanelBG { get; protected set; }
    public SpriteRuntime ShipIcon { get; protected set; }
    public SpriteRuntime PowerIcon { get; protected set; }
    public Label LivesLabel { get; protected set; }
    public Label PowerLabel { get; protected set; }
    public NineSliceRuntime ScorePanelBG { get; protected set; }
    public Label ScoreLabel { get; protected set; }
    public NineSliceRuntime BossPanelBG { get; protected set; }
    public PercentBar BossHPBar { get; protected set; }
    public ContainerRuntime ScorePanel { get; protected set; }
    public ContainerRuntime BossPanel { get; protected set; }
    public NineSliceRuntime WeaponPanelBG { get; protected set; }
    public SpriteRuntime WeaponPanelIcon { get; protected set; }
    public ContainerRuntime WeaponPanel { get; protected set; }

    public GameScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public GameScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        BottomPanel = this.Visual?.GetGraphicalUiElementByName("BottomPanel") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        BottomPanelBG = this.Visual?.GetGraphicalUiElementByName("BottomPanelBG") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        ShipIcon = this.Visual?.GetGraphicalUiElementByName("ShipIcon") as global::MonoGameGum.GueDeriving.SpriteRuntime;
        PowerIcon = this.Visual?.GetGraphicalUiElementByName("PowerIcon") as global::MonoGameGum.GueDeriving.SpriteRuntime;
        LivesLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"LivesLabel");
        PowerLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"PowerLabel");
        ScorePanelBG = this.Visual?.GetGraphicalUiElementByName("ScorePanelBG") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        ScoreLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"ScoreLabel");
        BossPanelBG = this.Visual?.GetGraphicalUiElementByName("BossPanelBG") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        BossHPBar = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<PercentBar>(this.Visual,"BossHPBar");
        ScorePanel = this.Visual?.GetGraphicalUiElementByName("ScorePanel") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        BossPanel = this.Visual?.GetGraphicalUiElementByName("BossPanel") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        WeaponPanelBG = this.Visual?.GetGraphicalUiElementByName("WeaponPanelBG") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        WeaponPanelIcon = this.Visual?.GetGraphicalUiElementByName("WeaponPanelIcon") as global::MonoGameGum.GueDeriving.SpriteRuntime;
        WeaponPanel = this.Visual?.GetGraphicalUiElementByName("WeaponPanel") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
