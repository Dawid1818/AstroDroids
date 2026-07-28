//Code for ShipCustomizationScreenGum
using AstroDroids.Components.Controls;
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
partial class ShipCustomizationScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::MonoGameGum.GueDeriving.ContainerRuntime();
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
    public ButtonStandard BodyBtn { get; protected set; }
    public ButtonStandard WeaponsBtn { get; protected set; }
    public ButtonStandard EnginesBtn { get; protected set; }
    public ButtonStandard CockpitBtn { get; protected set; }
    public ButtonStandard CockpitGlassBtn { get; protected set; }
    public ButtonStandard WingsBtn { get; protected set; }

    public ShipCustomizationScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public ShipCustomizationScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        ScorePanelBG = this.Visual?.GetGraphicalUiElementByName("ScorePanelBG") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        ScorePanelBG1 = this.Visual?.GetGraphicalUiElementByName("ScorePanelBG1") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        ScorePanelBG2 = this.Visual?.GetGraphicalUiElementByName("ScorePanelBG2") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        SatTrack = this.Visual?.GetGraphicalUiElementByName("SatTrack") as global::MonoGameGum.GueDeriving.SpriteRuntime;
        HueTrack = this.Visual?.GetGraphicalUiElementByName("HueTrack") as global::MonoGameGum.GueDeriving.SpriteRuntime;
        ValTrack = this.Visual?.GetGraphicalUiElementByName("ValTrack") as global::MonoGameGum.GueDeriving.SpriteRuntime;
        ShipIcon = this.Visual?.GetGraphicalUiElementByName("ShipIcon") as global::MonoGameGum.GueDeriving.SpriteRuntime;
        ContainerInstance = this.Visual?.GetGraphicalUiElementByName("ContainerInstance") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        ColoredRectangleInstance = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance1 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance1") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance2 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance2") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance3 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance3") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance4 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance4") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance5 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance5") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance6 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance6") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance7 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance7") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance8 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance8") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance9 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance9") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance10 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance10") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance11 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance11") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance12 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance12") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance13 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance13") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance14 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance14") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        ColoredRectangleInstance15 = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance15") as global::MonoGameGum.GueDeriving.ColoredRectangleRuntime;
        RSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ColorSlider>(this.Visual,"RSlider");
        RLabel = this.Visual?.GetGraphicalUiElementByName("RLabel") as global::MonoGameGum.GueDeriving.TextRuntime;
        GSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ColorSlider>(this.Visual,"GSlider");
        GLabel = this.Visual?.GetGraphicalUiElementByName("GLabel") as global::MonoGameGum.GueDeriving.TextRuntime;
        BSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ColorSlider>(this.Visual,"BSlider");
        BLabel = this.Visual?.GetGraphicalUiElementByName("BLabel") as global::MonoGameGum.GueDeriving.TextRuntime;
        ContainerInstance1 = this.Visual?.GetGraphicalUiElementByName("ContainerInstance1") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        BodyBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonStandard>(this.Visual,"BodyBtn");
        WeaponsBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonStandard>(this.Visual,"WeaponsBtn");
        EnginesBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonStandard>(this.Visual,"EnginesBtn");
        CockpitBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonStandard>(this.Visual,"CockpitBtn");
        CockpitGlassBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonStandard>(this.Visual,"CockpitGlassBtn");
        WingsBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonStandard>(this.Visual,"WingsBtn");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
