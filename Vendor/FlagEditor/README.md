# Flag Editor #
<https://github.com/timdetering/FlagEditor>

A simple flags editor. It can be used to edit flags in a property grid.

## Introduction ##
This editor displays a drop`down control holding a `CheckedListBox` with all the values of an enumeration.

## Using the `FlagsEditor` ##
You just have to put the `Editor` attributes on an enumeration to link it with the `FlagsEditor`. A `Description` attribute can also be added to each value. It will be shown as a tooltip on the `CheckedListBox` items.

Here is a sample.

    [Flags]
    [Editor(typeof(STUP.ComponentModel.Design.FlagsEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public enum EnumerationTest
    {
        [Description("Description for the first tested value.")]
        firstValue = 1,

        [Description("Description for the second tested value.")]
        secondValue = 2,

        [Description("Description for the third tested value.")]
        thirdValue = 4
    }

## How does it work? ##
The `Editor` is just one class that inherits `UITypeEditor`. The behavior of `Editor` is controlled by two functions.

### GetEditStyle ###

This function is used for controlling the appearance of the small button in the property grid. In this sample a dropdown arrow will be shown.

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
    {
        return UITypeEditorEditStyle.DropDown;
    }

### EditValue ###

This function is called when the user clicks on the small button.

    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
    {
        if (context != null && context.Instance != null && provider != null)
        {
            // Get an instance of the IWindowsFormsEditorService. 
            edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null) 
            {
                // Create a CheckedListBox
                clb = new CheckedListBox();

                ...
    
                // Show our CheckedListbox as a DropDownControl. 
                // This methods returns only when the dropdowncontrol is closed
                edSvc.DropDownControl(clb);
    
                // return the right enum value corresponding to the result
                return ...
            }
        }

        return value;
    }

The `DropDownControl` can be closed by calling the `edSvc.CloseDropDown()` function.

## TODO ##
 * [ ] Fix code styling in README file.
 * [ ] Upgrade project files to Visual Studio 2017.
 * [ ] Complete `AssemblyInfo` fields.
 * [ ] Verify and fix unit tests.

## References ##
 * <https://www.codeproject.com/articles/3058/a-flag-editor>