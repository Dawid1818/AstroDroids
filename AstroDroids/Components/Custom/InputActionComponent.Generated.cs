//Code for Custom/InputActionComponent (Container)
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
partial class InputActionComponent : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Custom/InputActionComponent") ?? throw new System.InvalidOperationException("Could not find an element named Custom/InputActionComponent - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new InputActionComponent(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(InputActionComponent)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Custom/InputActionComponent", () => 
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
    public enum InputActionCategory
    {
        Enabled,
        Disabled,
        Highlighted,
        Pushed,
        HighlightedFocused,
        Focused,
        DisabledFocused,
        FocusedGlow,
        FocusedGlownt,
        FocusedActive,
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

    InputActionCategory? _inputActionCategoryState;
    public InputActionCategory? InputActionCategoryState
    {
        get => _inputActionCategoryState;
        set
        {
            _inputActionCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("InputActionCategory"))
                {
                    var category = Visual.Categories["InputActionCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "InputActionCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
    }
    public NineSliceRuntime Background { get; protected set; }
    public NineSliceRuntime FocusedIndicator { get; protected set; }
    public TextRuntime LeftLabel { get; protected set; }
    public TextRuntime ItemLabel { get; protected set; }


    #region Animation Fields
    public AnimationRuntime SlideIn {get; protected set;}
    public AnimationRuntime SlideOut {get; protected set;}
    public AnimationRuntime GlowFocused {get; protected set;}
    #endregion
    public string LeftLabelText
    {
        get => LeftLabel.Text;
        set => LeftLabel.Text = value;
    }

    public InputActionComponent(InteractiveGue visual) : base(visual)
    {
    }
    public InputActionComponent()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Background = this.Visual?.GetGraphicalUiElementByName("Background") as global::Gum.GueDeriving.NineSliceRuntime;
        FocusedIndicator = this.Visual?.GetGraphicalUiElementByName("FocusedIndicator") as global::Gum.GueDeriving.NineSliceRuntime;
        LeftLabel = this.Visual?.GetGraphicalUiElementByName("LeftLabel") as global::Gum.GueDeriving.TextRuntime;
        ItemLabel = this.Visual?.GetGraphicalUiElementByName("ItemLabel") as global::Gum.GueDeriving.TextRuntime;
        SlideIn = this.Visual.GetAnimation("SlideIn");
        SlideOut = this.Visual.GetAnimation("SlideOut");
        GlowFocused = this.Visual.GetAnimation("GlowFocused");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
    }
    partial void CustomInitialize();
}
