//Code for ShipCustomizationScreenGum
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
partial class ShipCustomizationScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("ShipCustomizationScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named ShipCustomizationScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new ShipCustomizationScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(ShipCustomizationScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("ShipCustomizationScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public NineSliceRuntime ScorePanelBG { get; protected set; }
    public NineSliceRuntime ScorePanelBG1 { get; protected set; }
    public NineSliceRuntime ScorePanelBG2 { get; protected set; }
    public SpriteRuntime SatTrack { get; protected set; }
    public SpriteRuntime HueTrack { get; protected set; }
    public SpriteRuntime ValTrack { get; protected set; }
    public SpriteRuntime ShipIcon { get; protected set; }
    public ContainerRuntime ContainerInstance { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance1 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance2 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance3 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance4 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance5 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance6 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance7 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance8 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance9 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance10 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance11 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance12 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance13 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance14 { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance15 { get; protected set; }
    public ColorSlider RSlider { get; protected set; }
    public TextRuntime RLabel { get; protected set; }
    public ColorSlider GSlider { get; protected set; }
    public TextRuntime GLabel { get; protected set; }
    public ColorSlider BSlider { get; protected set; }
    public TextRuntime BLabel { get; protected set; }
    public ContainerRuntime ContainerInstance1 { get; protected set; }
    public ButtonGlow BodyBtn { get; protected set; }
    public ButtonGlow WeaponsBtn { get; protected set; }
    public ButtonGlow EnginesBtn { get; protected set; }
    public ButtonGlow CockpitBtn { get; protected set; }
    public ButtonGlow CockpitGlassBtn { get; protected set; }
    public ButtonGlow WingsBtn { get; protected set; }
    public ButtonGlow ReturnBtn { get; protected set; }

    public ShipCustomizationScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public ShipCustomizationScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        ScorePanelBG = this.Visual?.GetGraphicalUiElementByName("ScorePanelBG") as global::Gum.GueDeriving.NineSliceRuntime;
        ScorePanelBG1 = this.Visual?.GetGraphicalUiElementByName("ScorePanelBG1") as global::Gum.GueDeriving.NineSliceRuntime;
        ScorePanelBG2 = this.Visual?.GetGraphicalUiElementByName("ScorePanelBG2") as global::Gum.GueDeriving.NineSliceRuntime;
        SatTrack = this.Visual?.GetGraphicalUiElementByName("SatTrack") as global::Gum.GueDeriving.SpriteRuntime;
        HueTrack = this.Visual?.GetGraphicalUiElementByName("HueTrack") as global::Gum.GueDeriving.SpriteRuntime;
        ValTrack = this.Visual?.GetGraphicalUiElementByName("ValTrack") as global::Gum.GueDeriving.SpriteRuntime;
        ShipIcon = this.Visual?.GetGraphicalUiElementByName("ShipIcon") as global::Gum.GueDeriving.SpriteRuntime;
        ContainerInstance = this.Visual?.GetGraphicalUiElementByName("ContainerInstance") as global::Gum.GueDeriving.ContainerRuntime;
        ColoredRectangleInstance = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance1 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance1") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance2 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance2") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance3 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance3") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance4 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance4") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance5 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance5") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance6 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance6") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance7 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance7") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance8 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance8") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance9 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance9") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance10 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance10") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance11 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance11") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance12 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance12") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance13 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance13") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance14 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance14") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance15 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance15") as global::Gum.GueDeriving.ColoredRectangleRuntime;
        RSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ColorSlider>(this.Visual,"RSlider");
        RLabel = this.Visual?.GetGraphicalUiElementByName("RLabel") as global::Gum.GueDeriving.TextRuntime;
        GSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ColorSlider>(this.Visual,"GSlider");
        GLabel = this.Visual?.GetGraphicalUiElementByName("GLabel") as global::Gum.GueDeriving.TextRuntime;
        BSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ColorSlider>(this.Visual,"BSlider");
        BLabel = this.Visual?.GetGraphicalUiElementByName("BLabel") as global::Gum.GueDeriving.TextRuntime;
        ContainerInstance1 = this.Visual?.GetGraphicalUiElementByName("ContainerInstance1") as global::Gum.GueDeriving.ContainerRuntime;
        BodyBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"BodyBtn");
        WeaponsBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"WeaponsBtn");
        EnginesBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"EnginesBtn");
        CockpitBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"CockpitBtn");
        CockpitGlassBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"CockpitGlassBtn");
        WingsBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"WingsBtn");
        ReturnBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ReturnBtn");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.BodyBtn.Text = GumService.Default.LocalizationService.Translate("T_Body");
        this.CockpitBtn.Text = GumService.Default.LocalizationService.Translate("T_Cockpit");
        this.CockpitGlassBtn.Text = GumService.Default.LocalizationService.Translate("T_CockpitGlass");
        this.EnginesBtn.Text = GumService.Default.LocalizationService.Translate("T_Engines");
        this.ReturnBtn.Text = GumService.Default.LocalizationService.Translate("T_Return");
        this.WeaponsBtn.Text = GumService.Default.LocalizationService.Translate("T_Weapons");
        this.WingsBtn.Text = GumService.Default.LocalizationService.Translate("T_Wings");
    }
    partial void CustomInitialize();
}
