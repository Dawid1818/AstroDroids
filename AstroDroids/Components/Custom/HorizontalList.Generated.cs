//Code for Custom/HorizontalList (Container)
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
partial class HorizontalList : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Custom/HorizontalList") ?? throw new System.InvalidOperationException("Could not find an element named Custom/HorizontalList - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new HorizontalList(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(HorizontalList)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Custom/HorizontalList", () => 
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
    public NineSliceRuntime WeaponPanelBG { get; protected set; }
    public TextRuntime ItemLabel { get; protected set; }
    public Icon2 Icon2Instance { get; protected set; }
    public Icon2 Icon2Instance1 { get; protected set; }
    public ContainerRuntime WeaponPanel { get; protected set; }
    public TextRuntime LeftLabel { get; protected set; }


    #region Animation Fields
    public AnimationRuntime SlideIn {get; protected set;}
    public AnimationRuntime SlideOut {get; protected set;}
    #endregion
    public string LeftLabelText
    {
        get => LeftLabel.Text;
        set => LeftLabel.Text = value;
    }

    public HorizontalList(InteractiveGue visual) : base(visual)
    {
    }
    public HorizontalList()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        WeaponPanelBG = this.Visual?.GetGraphicalUiElementByName("WeaponPanelBG") as global::Gum.GueDeriving.NineSliceRuntime;
        ItemLabel = this.Visual?.GetGraphicalUiElementByName("ItemLabel") as global::Gum.GueDeriving.TextRuntime;
        Icon2Instance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Icon2>(this.Visual,"Icon2Instance");
        Icon2Instance1 = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Icon2>(this.Visual,"Icon2Instance1");
        WeaponPanel = this.Visual?.GetGraphicalUiElementByName("WeaponPanel") as global::Gum.GueDeriving.ContainerRuntime;
        LeftLabel = this.Visual?.GetGraphicalUiElementByName("LeftLabel") as global::Gum.GueDeriving.TextRuntime;
        SlideIn = this.Visual.GetAnimation("SlideIn");
        SlideOut = this.Visual.GetAnimation("SlideOut");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
    }
    partial void CustomInitialize();
}
