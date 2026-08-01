//Code for MainMenuScreenGum
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
    public TextRuntime TextInstance { get; protected set; }
    public ContainerRuntime ContainerInstance { get; protected set; }
    public ButtonGlow PlayBtn { get; protected set; }
    public ButtonGlow CustomizeBtn { get; protected set; }
    public ButtonGlow SettingsBtn { get; protected set; }
    public ButtonGlow LeaderboardBtn { get; protected set; }
    public ButtonGlow ExitBtn { get; protected set; }

    public MainMenuScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public MainMenuScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::Gum.GueDeriving.TextRuntime;
        ContainerInstance = this.Visual?.GetGraphicalUiElementByName("ContainerInstance") as global::Gum.GueDeriving.ContainerRuntime;
        PlayBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"PlayBtn");
        CustomizeBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"CustomizeBtn");
        SettingsBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"SettingsBtn");
        LeaderboardBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"LeaderboardBtn");
        ExitBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ExitBtn");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.CustomizeBtn.Text = GumService.Default.LocalizationService.Translate("T_Customize");
        this.ExitBtn.Text = GumService.Default.LocalizationService.Translate("T_Quit");
        this.LeaderboardBtn.Text = GumService.Default.LocalizationService.Translate("T_Leaderboard");
        this.PlayBtn.Text = GumService.Default.LocalizationService.Translate("T_Play");
        this.SettingsBtn.Text = GumService.Default.LocalizationService.Translate("T_Settings");
        this.TextInstance.Text = GumService.Default.LocalizationService.Translate("T_GameName");
    }
    partial void CustomInitialize();
}
