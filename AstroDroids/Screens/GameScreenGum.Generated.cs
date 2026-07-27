//Code for GameScreenGum
using AstroDroids.Components.Controls;
using AstroDroids.Components.Elements;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.StateAnimation.Runtime;
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
    public enum HideShow
    {
        Hidden,
        Shown,
    }

    HideShow? _hideShowState;
    public HideShow? HideShowState
    {
        get => _hideShowState;
        set
        {
            _hideShowState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("HideShow"))
                {
                    var category = Visual.Categories["HideShow"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "HideShow");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
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
    public ContainerRuntime BossWarning { get; protected set; }
    public ColoredRectangleRuntime BossWarningBG { get; protected set; }
    public CautionLinesWrapped BossWarningTopLines { get; protected set; }
    public CautionLinesWrapped BossWarningBottomLines { get; protected set; }
    public ContainerRuntime BossWarningTextClip { get; protected set; }
    public TextRuntime TextInstance { get; protected set; }


    #region Animation Fields
    public AnimationRuntime Show {get; protected set;}
    public AnimationRuntime Hide {get; protected set;}
    #endregion
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
        BossWarning = this.Visual?.GetGraphicalUiElementByName("BossWarning") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        BossWarningBG = this.Visual?.GetGraphicalUiElementByName("BossWarningBG") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        BossWarningTopLines = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<CautionLinesWrapped>(this.Visual,"BossWarningTopLines");
        BossWarningBottomLines = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<CautionLinesWrapped>(this.Visual,"BossWarningBottomLines");
        BossWarningTextClip = this.Visual?.GetGraphicalUiElementByName("BossWarningTextClip") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::MonoGameGum.GueDeriving.TextRuntime;
        Show = this.Visual.GetAnimation("Show");
        Hide = this.Visual.GetAnimation("Hide");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
