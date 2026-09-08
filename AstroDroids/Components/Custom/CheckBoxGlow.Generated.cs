//Code for Custom/CheckBoxGlow (Container)
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
namespace AstroDroids.Components.Custom;
partial class CheckBoxGlow : global::Gum.Forms.Controls.CheckBox
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Custom/CheckBoxGlow") ?? throw new System.InvalidOperationException("Could not find an element named Custom/CheckBoxGlow - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new CheckBoxGlow(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(CheckBoxGlow)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Custom/CheckBoxGlow", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public enum CheckBoxCategory
    {
        EnabledOn,
        EnabledOff,
        EnabledIndeterminate,
        DisabledOn,
        DisabledOff,
        DisabledIndeterminate,
        HighlightedOn,
        HighlightedOff,
        HighlightedIndeterminate,
        PushedOn,
        PushedOff,
        PushedIndeterminate,
        FocusedOn,
        FocusedOff,
        FocusedIndeterminate,
        HighlightedFocusedOn,
        HighlightedFocusedOff,
        HighlightedFocusedIndeterminate,
        DisabledFocusedOn,
        DisabledFocusedOff,
        DisabledFocusedIndeterminate,
        FocusedGlow,
        FocusedGlownt,
    }
    public enum SlideCategory
    {
        Idle,
        Right,
        Left,
    }

    CheckBoxCategory? _checkBoxCategoryState;
    public CheckBoxCategory? CheckBoxCategoryState
    {
        get => _checkBoxCategoryState;
        set
        {
            _checkBoxCategoryState = value;
            if(value != null)
            {
                if(Visual.Categories.ContainsKey("CheckBoxCategory"))
                {
                    var category = Visual.Categories["CheckBoxCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Visual.Tag).Categories.FirstOrDefault(item => item.Name == "CheckBoxCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.Visual.ApplyState(state);
                }
            }
        }
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
    public NineSliceRuntime CheckboxBackground { get; protected set; }
    public TextRuntime TextInstance { get; protected set; }
    public NineSliceRuntime FocusedIndicator1 { get; protected set; }
    public Icon Check { get; protected set; }
    public NineSliceRuntime FocusedIndicator { get; protected set; }


    #region Animation Fields
    public AnimationRuntime SlideIn {get; protected set;}
    public AnimationRuntime SlideOut {get; protected set;}
    public AnimationRuntime GlowFocused {get; protected set;}
    #endregion

    public CheckBoxGlow(InteractiveGue visual) : base(visual)
    {
    }
    public CheckBoxGlow()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        CheckboxBackground = this.Visual?.GetGraphicalUiElementByName("CheckboxBackground") as global::Gum.GueDeriving.NineSliceRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as global::Gum.GueDeriving.TextRuntime;
        FocusedIndicator1 = this.Visual?.GetGraphicalUiElementByName("FocusedIndicator1") as global::Gum.GueDeriving.NineSliceRuntime;
        Check = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Icon>(this.Visual,"Check");
        FocusedIndicator = this.Visual?.GetGraphicalUiElementByName("FocusedIndicator") as global::Gum.GueDeriving.NineSliceRuntime;
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
