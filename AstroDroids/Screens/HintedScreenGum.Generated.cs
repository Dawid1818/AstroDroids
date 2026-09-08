//Code for HintedScreenGum
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
partial class HintedScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("HintedScreenGum") ?? throw new System.InvalidOperationException("Could not find an element named HintedScreenGum - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new HintedScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(HintedScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("HintedScreenGum", () => 
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
    public ContainerRuntime HostPane { get; protected set; }
    public ContainerRuntime ActionHintsPanel { get; protected set; }
    public ActionHint ActionHintInstance { get; protected set; }
    public ActionHint ActionHintInstance2 { get; protected set; }
    public ActionHint ActionHintInstance1 { get; protected set; }
    public TextRuntime LogoLabel { get; protected set; }


    #region Animation Fields
    public AnimationRuntime Enter {get; protected set;}
    public AnimationRuntime Leave {get; protected set;}
    #endregion
    public HintedScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public HintedScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        HostPane = this.Visual?.GetGraphicalUiElementByName("HostPane") as global::Gum.GueDeriving.ContainerRuntime;
        ActionHintsPanel = this.Visual?.GetGraphicalUiElementByName("ActionHintsPanel") as global::Gum.GueDeriving.ContainerRuntime;
        ActionHintInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ActionHint>(this.Visual,"ActionHintInstance");
        ActionHintInstance2 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ActionHint>(this.Visual,"ActionHintInstance2");
        ActionHintInstance1 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<ActionHint>(this.Visual,"ActionHintInstance1");
        LogoLabel = this.Visual?.GetGraphicalUiElementByName("LogoLabel") as global::Gum.GueDeriving.TextRuntime;
        Enter = this.Visual.GetAnimation("Enter");
        Leave = this.Visual.GetAnimation("Leave");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.ActionHintInstance.Text = GumService.Default.LocalizationService.Translate("T_Navigate");
        this.ActionHintInstance1.Text = GumService.Default.LocalizationService.Translate("T_Return");
        this.LogoLabel.Text = GumService.Default.LocalizationService.Translate("T_GameName");
    }
    partial void CustomInitialize();
}
