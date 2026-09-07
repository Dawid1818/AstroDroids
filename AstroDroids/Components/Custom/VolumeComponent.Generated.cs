//Code for Custom/VolumeComponent (Container)
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
namespace AstroDroids.Components.Custom;
partial class VolumeComponent : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Custom/VolumeComponent") ?? throw new System.InvalidOperationException("Could not find an element named Custom/VolumeComponent - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new VolumeComponent(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(VolumeComponent)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Custom/VolumeComponent", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum SlideCategory
    {
        Idle,
        Right,
        Left,
    }

    SlideCategory? _slideCategoryState;
    public SlideCategory? SlideCategoryState
    {
        get => _slideCategoryState;
        set
        {
            _slideCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("SlideCategory"))
                {
                    var category = Visual.Categories["SlideCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "SlideCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public Slider VolumeSlider { get; protected set; }
    public TextRuntime LeftLabel { get; protected set; }
    public TextRuntime ValueLabel { get; protected set; }


    #region Animation Fields
    public AnimationRuntime SlideIn {get; protected set;}
    public AnimationRuntime SlideOut {get; protected set;}
    #endregion
    public string LeftLabelText
    {
        get => LeftLabel.Text;
        set => LeftLabel.Text = value;
    }

    public VolumeComponent(InteractiveGue visual) : base(visual)
    {
    }
    public VolumeComponent()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        VolumeSlider = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Slider>(this.Visual,"VolumeSlider");
        LeftLabel = this.Visual?.GetGraphicalUiElementByName("LeftLabel") as global::Gum.GueDeriving.TextRuntime;
        ValueLabel = this.Visual?.GetGraphicalUiElementByName("ValueLabel") as global::Gum.GueDeriving.TextRuntime;
        SlideIn = this.Visual.GetAnimation("SlideIn");
        SlideOut = this.Visual.GetAnimation("SlideOut");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.LeftLabel.Text = GumService.Default.LocalizationService.Translate("T_Music");
    }
    partial void CustomInitialize();
}
