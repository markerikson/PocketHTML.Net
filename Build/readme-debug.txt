PocketHTML.Net version 1.2 (beta)
ISquared Software
http://www.isquaredsoftware.com

This is a debug version of PocketHTML.  It should run about the same as the release version,
but will output debug information to \Temp\pockethtml-*date*-log.txt

Changes in version 1.2:

Version 1.2 Release Candidate

    * ADD: Portrait / Landscape compatibility!
    * ADD: Tag editor
    * CHG: Reworked the Find/Replace dialog architecture
    * BUG: Fixed doctype tags, <dl> tag, and comment tag
    * BUG: Fixed bug in file filters used by the standard OpenFileDialog
    * BUG: Now uses a UTF8Encoding that doesn't print a BOM

Version 1.2 Beta 7

    * ADD: VGA compatibility!
    * ADD: HTML preview now loads images
    * ADD: Option to set the default text encoding
    * CHG: Modified options dialog

Version 1.2 Beta 6:

    * BUG: PocketHTML would crash on startup.

Version 1.2 betas 4 and 5: not released publicly

Version 1.2 beta 3:

    * BUG: Files with a charset other than the default were not being interpreted properly
    * CHG: Additional check code added to aid in debugging. 

Version 1.2 beta 2:

    * BUG: Opening a file while the current file was unsaved would replace the current file 
               without a warning
    * BUG: Tags with default attributes were being inserted with extra quotation marks
    * BUG: Indenting text while scrolled down caused a jump to the top of the textbox
    * BUG: File > Close wasn't working
    * BUG: Comment tag was broken

Version 1.2 beta 1:

    * ADD: Tag context menu can be bound to a hardware button
    * ADD: Copy/Cut/Paste added to context menu
    * ADD: Recent files list added to File menu
    * ADD: Indent / Unindent selected text functions
    * ADD: User-added HTML templates - just save your template file to the 
               PocketHTML\Templates directory
    * CHG: Tags file is now XML-based, for easier modification
    * CHG: Tag context menu is now generated from menu.xml (which means new items can be 
               added to the menu just by editing menu.xml)
    * CHG: File dialogs start with last used directory
    * CHG: Additional file types added to the dialogs (.asp, .php, etc)
    * CHG: Save dialog defaults to current file's type
    * CHG: TAB key now sent to textbox instead of changing focus
    * CHG: TAB / SHIFT-TAB keys now indent/unindent text
    * CHG: Find/Replace dialogs are shown with selected text
    * CHG: Tag insertion now uses tabs instead of spaces for indentation
    * BUG: Warn when possibly overwriting a saved file
    * BUG: Replacing text doesn't set modified flag
    * BUG: Saving a saved file brings up the "Save As" dialog again
    * BUG: Text wrapping not doing anything
    * BUG: Find and replace dialogs should be exclusive
    * BUG: "XHTML tags" option was broken
