//Code for GameScreenGum
using AstroDroids.Components.Controls;
using AstroDroids.Components.Elements;
using Gum;
using Gum.Converters;
using Gum.DataTypes;
using Gum.GueDeriving;
using Gum.Managers;
using Gum.StateAnimation.Runtime;
using Gum.Wireframe;
using GumRuntime;
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
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
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
    public enum MissionStatus
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

    MissionStatus? _missionStatusState;
    public MissionStatus? MissionStatusState
    {
        get => _missionStatusState;
        set
        {
            _missionStatusState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("MissionStatus"))
                {
                    var category = Visual.Categories["MissionStatus"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "MissionStatus");
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
    public ContainerRuntime MissionStatusContainer { get; protected set; }
    public ContainerRuntime PauseMenu { get; protected set; }
    public NineSliceRuntime PauseMenuBG { get; protected set; }
    public TextRuntime PausedLabel { get; protected set; }
    public ContainerRuntime ButtonContainer { get; protected set; }
    public ButtonGlow ResumeBtn { get; protected set; }
    public ButtonGlow SettingsBtn { get; protected set; }
    public ButtonGlow QuitBtn { get; protected set; }
    public TextRuntime MissionStatusLabel { get; protected set; }


    #region Animation Fields
    public AnimationRuntime Show {get; protected set;}
    public AnimationRuntime Hide {get; protected set;}
    public AnimationRuntime ShowMissionStatus {get; protected set;}
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
        BottomPanel = this.Visual?.GetGraphicalUiElementByName("BottomPanel") as global::Gum.GueDeriving.ContainerRuntime;
        BottomPanelBG = this.Visual?.GetGraphicalUiElementByName("BottomPanelBG") as global::Gum.GueDeriving.NineSliceRuntime;
        ShipIcon = this.Visual?.GetGraphicalUiElementByName("ShipIcon") as global::Gum.GueDeriving.SpriteRuntime;
        PowerIcon = this.Visual?.GetGraphicalUiElementByName("PowerIcon") as global::Gum.GueDeriving.SpriteRuntime;
        LivesLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"LivesLabel");
        PowerLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"PowerLabel");
        ScorePanelBG = this.Visual?.GetGraphicalUiElementByName("ScorePanelBG") as global::Gum.GueDeriving.NineSliceRuntime;
        ScoreLabel = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Label>(this.Visual,"ScoreLabel");
        BossPanelBG = this.Visual?.GetGraphicalUiElementByName("BossPanelBG") as global::Gum.GueDeriving.NineSliceRuntime;
        BossHPBar = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<PercentBar>(this.Visual,"BossHPBar");
        ScorePanel = this.Visual?.GetGraphicalUiElementByName("ScorePanel") as global::Gum.GueDeriving.ContainerRuntime;
        BossPanel = this.Visual?.GetGraphicalUiElementByName("BossPanel") as global::Gum.GueDeriving.ContainerRuntime;
        WeaponPanelBG = this.Visual?.GetGraphicalUiElementByName("WeaponPanelBG") as global::Gum.GueDeriving.NineSliceRuntime;
        WeaponPanelIcon = this.Visual?.GetGraphicalUiElementByName("WeaponPanelIcon") as global::Gum.GueDeriving.SpriteRuntime;
        WeaponPanel = this.Visual?.GetGraphicalUiElementByName("WeaponPanel") as global::Gum.GueDeriving.ContainerRuntime;
        BossWarning = this.Visual?.GetGraphicalUiElementByName("BossWarning") as global::Gum.GueDeriving.ContainerRuntime;
        BossWarningBG = this.Visual?.GetGraphicalUiElementByName("BossWarningBG") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        BossWarningTopLines = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<CautionLinesWrapped>(this.Visual,"BossWarningTopLines");
        BossWarningBottomLines = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<CautionLinesWrapped>(this.Visual,"BossWarningBottomLines");
        BossWarningTextClip = this.Visual?.GetGraphicalUiElementByName("BossWarningTextClip") as global::Gum.GueDeriving.ContainerRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::Gum.GueDeriving.TextRuntime;
        MissionStatusContainer = this.Visual?.GetGraphicalUiElementByName("MissionStatusContainer") as global::Gum.GueDeriving.ContainerRuntime;
        PauseMenu = this.Visual?.GetGraphicalUiElementByName("PauseMenu") as global::Gum.GueDeriving.ContainerRuntime;
        PauseMenuBG = this.Visual?.GetGraphicalUiElementByName("PauseMenuBG") as global::Gum.GueDeriving.NineSliceRuntime;
        PausedLabel = this.Visual?.GetGraphicalUiElementByName("PausedLabel") as global::Gum.GueDeriving.TextRuntime;
        ButtonContainer = this.Visual?.GetGraphicalUiElementByName("ButtonContainer") as global::Gum.GueDeriving.ContainerRuntime;
        ResumeBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ResumeBtn");
        SettingsBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"SettingsBtn");
        QuitBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"QuitBtn");
        MissionStatusLabel = this.Visual?.GetGraphicalUiElementByName("MissionStatusLabel") as global::Gum.GueDeriving.TextRuntime;
        Show = this.Visual.GetAnimation("Show");
        Hide = this.Visual.GetAnimation("Hide");
        ShowMissionStatus = this.Visual.GetAnimation("ShowMissionStatus");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.MissionStatusLabel.Text = GumService.Default.LocalizationService.Translate("T_MissionComplete");
        this.PausedLabel.Text = GumService.Default.LocalizationService.Translate("T_Paused");
        this.QuitBtn.Text = GumService.Default.LocalizationService.Translate("T_SaveandQuit");
        this.ResumeBtn.Text = GumService.Default.LocalizationService.Translate("T_Resume");
        this.SettingsBtn.Text = GumService.Default.LocalizationService.Translate("T_Settings");
        this.TextInstance.Text = GumService.Default.LocalizationService.Translate("T_BossWarning");
    }
    partial void CustomInitialize();
}
