//Code for ShipCustomizationScreenGum
using AstroDroids.Components.Controls;
using AstroDroids.Components.Custom;
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
    public enum ColorPickerCategory
    {
        Out,
        In,
    }
    public enum HeaderCategory
    {
        Out,
        In,
    }

    ColorPickerCategory? _colorPickerCategoryState;
    public ColorPickerCategory? ColorPickerCategoryState
    {
        get => _colorPickerCategoryState;
        set
        {
            _colorPickerCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("ColorPickerCategory"))
                {
                    var category = Visual.Categories["ColorPickerCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "ColorPickerCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }

    HeaderCategory? _headerCategoryState;
    public HeaderCategory? HeaderCategoryState
    {
        get => _headerCategoryState;
        set
        {
            _headerCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("HeaderCategory"))
                {
                    var category = Visual.Categories["HeaderCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "HeaderCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public SpriteRuntime ShipIcon { get; protected set; }
    public ContainerRuntime ButtonContainer { get; protected set; }
    public ButtonGlow BodyBtn { get; protected set; }
    public ButtonGlow WeaponsBtn { get; protected set; }
    public ButtonGlow EnginesBtn { get; protected set; }
    public ButtonGlow CockpitBtn { get; protected set; }
    public ButtonGlow CockpitGlassBtn { get; protected set; }
    public ButtonGlow WingsBtn { get; protected set; }
    public ButtonGlow ReturnBtn { get; protected set; }
    public ContainerRuntime ColorPicker { get; protected set; }
    public NineSliceRuntime HBG { get; protected set; }
    public NineSliceRuntime SBG { get; protected set; }
    public NineSliceRuntime VBG { get; protected set; }
    public SpriteRuntime HueTrack { get; protected set; }
    public SpriteRuntime SatTrack { get; protected set; }
    public SpriteRuntime ValTrack { get; protected set; }
    public ColorSlider HSlider { get; protected set; }
    public TextRuntime HLabel { get; protected set; }
    public ColorSlider SSlider { get; protected set; }
    public TextRuntime SLabel { get; protected set; }
    public ColorSlider VSlider { get; protected set; }
    public TextRuntime VLabel { get; protected set; }
    public TextRuntime Header { get; protected set; }


    #region Animation Fields
    public AnimationRuntime Enter {get; protected set;}
    public AnimationRuntime Leave {get; protected set;}
    #endregion
    public ShipCustomizationScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public ShipCustomizationScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        ShipIcon = this.Visual?.GetGraphicalUiElementByName("ShipIcon") as global::Gum.GueDeriving.SpriteRuntime;
        ButtonContainer = this.Visual?.GetGraphicalUiElementByName("ButtonContainer") as global::Gum.GueDeriving.ContainerRuntime;
        BodyBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"BodyBtn");
        WeaponsBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"WeaponsBtn");
        EnginesBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"EnginesBtn");
        CockpitBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"CockpitBtn");
        CockpitGlassBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"CockpitGlassBtn");
        WingsBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"WingsBtn");
        ReturnBtn = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ButtonGlow>(this.Visual,"ReturnBtn");
        ColorPicker = this.Visual?.GetGraphicalUiElementByName("ColorPicker") as global::Gum.GueDeriving.ContainerRuntime;
        HBG = this.Visual?.GetGraphicalUiElementByName("HBG") as global::Gum.GueDeriving.NineSliceRuntime;
        SBG = this.Visual?.GetGraphicalUiElementByName("SBG") as global::Gum.GueDeriving.NineSliceRuntime;
        VBG = this.Visual?.GetGraphicalUiElementByName("VBG") as global::Gum.GueDeriving.NineSliceRuntime;
        HueTrack = this.Visual?.GetGraphicalUiElementByName("HueTrack") as global::Gum.GueDeriving.SpriteRuntime;
        SatTrack = this.Visual?.GetGraphicalUiElementByName("SatTrack") as global::Gum.GueDeriving.SpriteRuntime;
        ValTrack = this.Visual?.GetGraphicalUiElementByName("ValTrack") as global::Gum.GueDeriving.SpriteRuntime;
        HSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ColorSlider>(this.Visual,"HSlider");
        HLabel = this.Visual?.GetGraphicalUiElementByName("HLabel") as global::Gum.GueDeriving.TextRuntime;
        SSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ColorSlider>(this.Visual,"SSlider");
        SLabel = this.Visual?.GetGraphicalUiElementByName("SLabel") as global::Gum.GueDeriving.TextRuntime;
        VSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ColorSlider>(this.Visual,"VSlider");
        VLabel = this.Visual?.GetGraphicalUiElementByName("VLabel") as global::Gum.GueDeriving.TextRuntime;
        Header = this.Visual?.GetGraphicalUiElementByName("Header") as global::Gum.GueDeriving.TextRuntime;
        Enter = this.Visual.GetAnimation("Enter");
        Leave = this.Visual.GetAnimation("Leave");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.BodyBtn.Text = GumService.Default.LocalizationService.Translate("T_Body");
        this.CockpitBtn.Text = GumService.Default.LocalizationService.Translate("T_Cockpit");
        this.CockpitGlassBtn.Text = GumService.Default.LocalizationService.Translate("T_CockpitGlass");
        this.EnginesBtn.Text = GumService.Default.LocalizationService.Translate("T_Engines");
        this.Header.Text = GumService.Default.LocalizationService.Translate("T_Customize");
        this.ReturnBtn.Text = GumService.Default.LocalizationService.Translate("T_Return");
        this.WeaponsBtn.Text = GumService.Default.LocalizationService.Translate("T_Weapons");
        this.WingsBtn.Text = GumService.Default.LocalizationService.Translate("T_Wings");
    }
    partial void CustomInitialize();
}
