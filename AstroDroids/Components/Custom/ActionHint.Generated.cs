//Code for Custom/ActionHint (Container)
using AstroDroids.Components.Elements;
using Gum;
using Gum.Converters;
using Gum.DataTypes;
using Gum.GueDeriving;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary.Graphics;
using System.Linq;
namespace AstroDroids.Components.Custom;
partial class ActionHint : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::Gum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Custom/ActionHint") ?? throw new System.InvalidOperationException("Could not find an element named Custom/ActionHint - did you forget to load a Gum project?");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new ActionHint(visual);
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(ActionHint)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Custom/ActionHint", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public TextRuntime Label { get; protected set; }
    public Icon2 ActionIconShadow { get; protected set; }
    public Icon2 ActionIcon { get; protected set; }

    public Icon2.IconCategory? Icon
    {
        get => ActionIcon.IconCategoryState;
        set => ActionIcon.IconCategoryState = value;
    }

    public Icon2.IconCategory? IconShadow
    {
        get => ActionIconShadow.IconCategoryState;
        set => ActionIconShadow.IconCategoryState = value;
    }

    public string Text
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    public ActionHint(InteractiveGue visual) : base(visual)
    {
    }
    public ActionHint()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Label = this.Visual?.GetGraphicalUiElementByName("Label") as global::Gum.GueDeriving.TextRuntime;
        ActionIconShadow = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Icon2>(this.Visual,"ActionIconShadow");
        ActionIcon = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<Icon2>(this.Visual,"ActionIcon");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    public void ApplyLocalization()
    {
        this.Label.Text = GumService.Default.LocalizationService.Translate("T_Select");
    }
    partial void CustomInitialize();
}
