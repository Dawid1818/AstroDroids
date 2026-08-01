//Code for MainMenuScreenGum
using AstroDroids.Components.Controls;
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
partial class MainMenuScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("MainMenuScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named MainMenuScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new MainMenuScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(MainMenuScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("MainMenuScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum Animations
    {
        PlayArrival,
        Arrived,
        Out,
    }

    Animations? _animationsState;
    public Animations? AnimationsState
    {
        get => _animationsState;
        set
        {
            _animationsState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("Animations"))
                {
                    var category = Visual.Categories["Animations"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "Animations");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public TextRuntime LogoLabel { get; protected set; }
    public ContainerRuntime ButtonContainer { get; protected set; }
    public ButtonGlow PlayBtn { get; protected set; }
    public ButtonGlow CustomizeBtn { get; protected set; }
    public ButtonGlow SettingsBtn { get; protected set; }
    public ButtonGlow LeaderboardBtn { get; protected set; }
    public ButtonGlow CreditsBtn { get; protected set; }
    public ButtonGlow ExitBtn { get; protected set; }


    #region Animation Fields
    public AnimationRuntime Enter {get; protected set;}
    public AnimationRuntime Leave {get; protected set;}
    public AnimationRuntime EnterSimple {get; protected set;}
    #endregion
    public MainMenuScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public MainMenuScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        LogoLabel = this.Visual?.GetGraphicalUiElementByName("LogoLabel") as global::Gum.GueDeriving.TextRuntime;
        ButtonContainer = this.Visual?.GetGraphicalUiElementByName("ButtonContainer") as global::Gum.GueDeriving.ContainerRuntime;
        PlayBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"PlayBtn");
        CustomizeBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"CustomizeBtn");
        SettingsBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"SettingsBtn");
        LeaderboardBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"LeaderboardBtn");
        CreditsBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"CreditsBtn");
        ExitBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ExitBtn");
        Enter = this.Visual.GetAnimation("Enter");
        Leave = this.Visual.GetAnimation("Leave");
        EnterSimple = this.Visual.GetAnimation("EnterSimple");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.CreditsBtn.Text = GumService.Default.LocalizationService.Translate("T_Credits");
        this.CustomizeBtn.Text = GumService.Default.LocalizationService.Translate("T_Customize");
        this.ExitBtn.Text = GumService.Default.LocalizationService.Translate("T_Quit");
        this.LeaderboardBtn.Text = GumService.Default.LocalizationService.Translate("T_Leaderboard");
        this.LogoLabel.Text = GumService.Default.LocalizationService.Translate("T_GameName");
        this.PlayBtn.Text = GumService.Default.LocalizationService.Translate("T_Play");
        this.SettingsBtn.Text = GumService.Default.LocalizationService.Translate("T_Settings");
    }
    partial void CustomInitialize();
}
