//Code for CurveEditorScreenGum
using GumRuntime;
using System.Linq;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using AstroDroids.Components.Controls;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

namespace AstroDroids.Screens;
partial class CurveEditorScreenGum : global::Gum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new global::Gum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new global::MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("CurveEditorScreenGum");
#if DEBUG
if(element == null) throw new System.InvalidOperationException("Could not find an element named CurveEditorScreenGum - did you forget to load a Gum project?");
#endif
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new CurveEditorScreenGum(visual);
            visual.Width = 0;
            visual.WidthUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = global::Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        global::Gum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(CurveEditorScreenGum)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("CurveEditorScreenGum", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public WindowStandard WindowStandardInstance { get; protected set; }
    public TextRuntime StartPointText { get; protected set; }
    public TextRuntime KeyPoint1Text { get; protected set; }
    public TextRuntime EndPointText { get; protected set; }
    public TextRuntime KeyPoint2Text { get; protected set; }

    public CurveEditorScreenGum(InteractiveGue visual) : base(visual)
    {
    }
    public CurveEditorScreenGum()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        WindowStandardInstance = global::Gum.Forms.GraphicalUiElementFormsExtensions.TryGetFrameworkElementByName<WindowStandard>(this.Visual,"WindowStandardInstance");
        StartPointText = this.Visual?.GetGraphicalUiElementByName("StartPointText") as global::MonoGameGum.GueDeriving.TextRuntime;
        KeyPoint1Text = this.Visual?.GetGraphicalUiElementByName("KeyPoint1Text") as global::MonoGameGum.GueDeriving.TextRuntime;
        EndPointText = this.Visual?.GetGraphicalUiElementByName("EndPointText") as global::MonoGameGum.GueDeriving.TextRuntime;
        KeyPoint2Text = this.Visual?.GetGraphicalUiElementByName("KeyPoint2Text") as global::MonoGameGum.GueDeriving.TextRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
