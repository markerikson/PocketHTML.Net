PocketHTML.Net version 1.2 (beta)
ISquared Software
http://www.isquaredsoftware.com

This is a debug version of PocketHTML.  It should run about the same as the release version,
but will output debug information to \Temp\pockethtml-*date*-log.txt

Changes in version 1.2:

- ADD:	Tag context menu can be bound to a hardware button
- ADD:	Copy/Cut/Paste added to context menu
- ADD:	Recent files list added to File menu
- ADD:	Indent / Unindent selected text functions
- ADD:	User-added HTML templates - just save your template file 
		to the PocketHTML\Templates directory

- CHG:	Tags file is now XML-based, for easier modification
- CHG:	Tag context menu is now generated from menu.xml (which means
		new items can be added to the menu just by editing menu.xml)
- CHG:	File dialogs start with last used directory
- CHG:	Additional file types added to the dialogs (.asp, .php, etc)
- CHG:	Save dialog defaults to current file's type
- CHG:	TAB key now sent to textbox instead of changing focus
- CHG:	TAB / SHIFT-TAB indent/unindent text
- CHG:	Find/Replace dialogs are shown with selected text

- BUG:	Warn when possibly overwriting a saved file
- BUG:	Replacing text doesn't set modified flag
- BUG:	Saving a saved file brings up the "Save As" dialog again
- BUG:	Text wrapping not doing anything
- BUG:	Find and replace dialogs should be exclusive