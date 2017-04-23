-------------------------------------
SCRIPT INSPECTOR 3
version 3.0.17, December 2016
Copyright © 2012-2016, Flipbook Games
-------------------------------------


Unity's legendary editor for C#, UnityScript, Boo, Shaders, and text, now transformed into an advanced C# IDE!!!


IMPORTANT NOTE to all OS X users with Unity 5 Personal or Professional and Unity 4 Pro:
You may want to extend Si3's functionality with @inventor2010's amazing free plugin posted here:
	http://forum.unity3d.com/threads/script-inspector-3.195218/page-13#post-2226154
or even better, the updated version posted here:
	http://forum.unity3d.com/threads/script-inspector-3.195218/page-20#post-2414302
The plugin allows handling Cmd-S, Cmd-Z, and Shift-Cmd-Z in Unity on OS X for more natural shortcuts for Save, Undo, and Redo operations in Si3!
Thank you @inventor2010 for being awesome!!! :)


Note: Version history is now moved to SI Changes.txt file.


1. Description


SCRIPT INSPECTOR 3 is the latest HUGE improvement of that legendary editor extension for Unity which you've been using so far only as a quick editor for your scripts, shaders and text assets. Within the last two years of development it has been transformed into a powerful C# IDE embedded right inside your Unity Editor!

Built with performance in mind since the beginning while keeping the quality at the highest standards, it was growing with new features, to finally become the fastest and most comfortable solution for programmers to write code for Unity.

Script Inspector 3 opens files instantly and you can start editing them immediately! No more waiting for external editors to start, load the project, and then your script, and by the time all that finishes you'll often forget what your intention was initially. :p Script Inspector 3 will keep you stay focused on programming instead of fighting with code editing tools. :)

Using its greatest advantage, being integrated inside Unity, Script Inspector 3 is definitely the greatest tool available for improving programmer’s workflow! Accessibility combined with quick iteration cycles achieved with Script Inspector 3 makes this extension the preferred IDE for many Unity programmers including myself, the author. As a result of that, the development of this magnificent tool was done in it, making Script Inspector 3 the first Unity extension programmed in itself! ;)


2. Motivation

After surprisingly high popularity of all of the previous versions of Script Inspector and by popular demand for adding automatic code completion to it, Script Inspector was transformed into a powerful IDE that’s not only a handy and nice tool for fixing typos or closing forgotten parentheses, but also a really solid IDE strong enough to support professional game development and to be used in real projects.


3. Using Script Inspector 3

First thing you will notice when you start using Script Inspector 3 is the SI Console tab which opens automatically and docks next to Unity’s default Console tab. You can also open it manually from the Window menu, select Window->Script Inspector Console and the new SI Console tab will dock next to the Console tab. SI Console works exactly the same as the original console with the only difference that on double-click or Enter/Return key it opens the associated script or shader with the selected log entry in a Si3 tab instead of opening it in the external IDE. You can still open log entries in the external IDE from the standard Console tab.

Whenever a Si3 supported asset is selected in Unity Editor, the Inspector tab displays its full content in a way typical for editing code or text files, with syntax highlighting, optional word-wrap, proper tab expansion and alignment, optional line numbers. Script Inspector 3 also allows editing files in dedicated windows.

To open a script, shader, or text asset in dedicated editor tabs there are a couple of options. One of them is to follow a log entry from the SI Console. From the SI Console you can also see the call stack and navigate to any of those locations from the context popup menu. Another way to open scripts, shaders, and text assets is to use the New Tab toolbar button in the upper left corner (or using the keyboard shortcut Ctrl+T, or Cmd-T on Mac). Si3 windows can be arranged, docked, undocked, maximized, or closed as any other regular Unity window.

You can open additional script, shader, or text assets into a particular tab group by dragging assets from the Project view to one of the existing Si3 windows. You can also drag Materials to open the shader used in them directly. Similarly you can drag GameObjects from the current scene or Prefabs to open their components based on MonoBehaviour scripts. Component scripts can also be easily accessed from the component’s wrench menus in the Inspector. Similarly shaders used in materials can be opened from the wrench menu.

Script Inspector 3 can (optionally) open a script, shader, or text assets on double-click in the Project tab. These options are located in the wrench menu of any of the Si3 views. Pressing Enter (Command-Down Arrow on Mac) on the selected asset in the Project tab would still open them in the external IDE, so you still have that option available.

Assets can be opened in multiple Si3 views at the same time showing different parts of their content with different caret position and selection. Editing is synchronized. Changes made in one of these tabs are immediately visible in the other tabs. The Undo/Redo buffer is also shared so that changes made in one tab can be reverted in another one, for example.

Script Inspector 3 holds unlimited size of its Undo/Redo buffers for each edited asset independently. These buffers are also independent of the Unity’s built-in undo buffer, so that changes in scripts don’t interfere with the changes made in the rest of Unity and each can be reverted or repeated independently. There are Undo and Redo toolbar buttons in each Si3 view, and there are keyboard shortcuts associated with them – Ctrl+Z and Shift+Ctrl+Z on Windows and Control-Z and Shift-Control-Z on OS X for Undo and Redo respectively.

Script Inspector 3 handles very wide range of mouse and keyboard input actions, providing the users with an experience similar to other modern IDE’s. Cursor navigation, selecting words, selecting lines, Cut/Copy/Paste, drag-selections, dragging selected text to move or copy, search functionality, quick search, are only some of the features implemented in Script Inspector 3. See the Shortcuts section for a more complete list of included features.


4. Saving and Reloading

Script Inspector 3 keeps track of all changes made using it. Asterisk signs in Si3 tab’s titles indicate unsaved changed. You can save the changes with the Save toolbar button or with the keyboard shortcut Ctrl+S on Windows or Control-S on OS X. All modified assets will be automatically saved on entering Editor Game Mode.

Navigating away from a modified file in the Inspector or closing a Si3 tab will warn you and ask if you want to save the changes made in the asset, discard them, or keep them in memory and continue editing later. You’ll also get similar warnings on quitting the Editor or loading another Unity project, only without the option to keep the changes in memory, of course.

After saving the changes made Unity will compile them as usual and display all compile warnings and errors in both the Console and SI Console.

Reimported scripts, either manually or automatically as a result of external changes, will be updated in Si3 views automatically. Well, unless those scripts have been modified and not saved before reimporting, in which case Script Inspector 3 gives you a warning and asks which version to keep.


5. List of keyboard shortcuts and mouse functions

(assume Cmd instead of Ctrl on OS X if not noted otherwise)

a) Cursor navigation (all these clear the selection)

Arrow keys - caret navigation
Mouse click – sets caret position
Ctrl+Left and Ctrl+Right arrow key - Moves the cursor to the previous or next word on Windows or sub-word on OS Alt+Left and Alt+Right arrow key - Moves the cursor to the previous or next word on OS X
Ctrl+Up and Ctrl+Down arrow key - Scrolls the code view by one line
PageUp and PageDown - move the cursor by one page
End - moves the cursor to the end of a line
Home - moves the cursor before the first non-whitespace character on a line or at the beginning of the line
Ctrl+Home - move the cursor at the beginning of the whole script
Ctrl+End - move the cursor at the end of the whole script


b) Cursor navigation & history

Ctrl+G (Cmd-L on OS X) - Go To Line
Alt+Left arrow key (Alt-Cmd-Left arrow on OS X) - Go Back
Alt+Right arrow key (Alt-Cmd-Right arrow on OS X) - Go Forward
Alt+M (Ctrl-M on OS X) - opens the last code navigation breadcrumb button (C# files only)

c) Selections

Shift + any cursor navigation shortcut or mouse click - selects text or alters the selection
Mouse double-click or Ctrl+mouse click - selects the whole word
Mouse click and drag - mouse drag selection
Mouse double-click and drag or Ctrl+mouse click and drag - mouse drag selection for whole words
Mouse click on line numbers - line select
Mouse tripple-click - line select
Mouse click and drag on line numbers - mouse drag line selection
Mouse tripple-click and drag - mouse drag lines selection
Ctrl+A, or Edit->Select All from the main menu, or Select All from the context menu - selects the whole content of a file
Escape - clears the selection

d) Editing

Typing text – inserts text :p (replacing the selected text, if any)
Backspace – deletes selected text or the character before of the caret
Delete – deletes selected text or the character after the caret
Ctrl+Backspace – deletes selected text or the word or part of it before the caret
Ctrl+Delete – deletes selected text or the word or part of it after the caret

e) Cut, Copy, Paste, Duplicate

Ctrl+X, or Edit->Cut from the main menu, or Cut from the context menu - cuts the selected text
Ctrl+C, or Edit->Copy from the main menu, or Copy from the context menu - copies the selected text
Ctrl+V, or Edit->Paste from the main menu, or Paste from the context menu – pastes the clipboard text (replacing the selected text, if any)
Mouse dragging selected text – moves the selected text
Ctrl+mouse dragging selected text – duplicates the selected text
Ctrl+D, or Edit->Duplicate (and also Alt-Cmd-Down on OS X) - duplicates the line at cursor or selected lines

f) More editing

Ctrl+Z (Control-Z on OS X) – Undo
Shift+Ctrl+Z (Shift-Control-Z on OS X) – Redo
Alt+Up/Down arrow keys - moves current line or selected lines up or down and re-indents
Ctrl+K or Ctrl+/ - toggles on or off code comments on a single line or selected lines (except while editing text assets, of course)
Ctrl+[ - decreases indentation of a single line or more selected lines
Ctrl+] - increases indentation of a single line or more selected lines
Tab – inserts a Tab character if nothing is selected, otherwise increases indentation
Shift+Tab – deletes the Tab character before caret, or otherwise decreases indentation

g) Automatic completion (C# only)

Escape or Ctrl+Space - opens aut-completion popup window
Up/Down arrow keys - select word to complete automatically
Typing - filters the completion list
Enter, Tab, or a non-alphanumeric character - accepts selected completion
Escape - cancels completion list popup window

h) Searching & Replacing

F12 (F12 or Cmd-Y on OS X) - Go To Definition
Shift+F12 (Shift-F12 or Shift-Cmd-Y on OS X) - Find All References
Shift+Ctrl+F - Find in Files
Shift+Ctrl+H - Replace in Files
Shift+Ctrl+Up – finds the previous occurrence of the word at caret or of the selected text
Shift+Ctrl+Down – finds the next occurrence of the word at caret or of the selected text
Ctrl+F – sets the keyboard input to the search field in the toolbar
Escape inside the search field – sets the keyboard focus back to the code editor
Enter inside the search field – finds the next occurrence of the searched text
Shift+Enter inside the search field – finds the previous occurrence of the searched text
Up/Down Arrow keys and arrow buttons inside the search field – find previous/next occurrence of the searched text
F3, or Ctrl+G – finds next occurrence of the searched text
Shift+F3, or Shift+Ctrl+G – finds the previous occurrence of the searched text

i) Si3 tabs related

Alt+T, or Window->Toggle Si3 Tabs - toggle show/hide all floating Si3 tabs
Ctrl+F4 or Ctrl+W (Cmd-F4 or Cmd-Shift-W on OS X) – closes the active Si3 tab
Ctrl+Tab and Shift+Ctrl+Tab (Alt-Tab and Shift-Alt-Tab on OS X) - Tab history navigation (hold Ctrl to see full history and then navigate with arrow keys)
Ctrl+PageUp or Ctrl+Alt+Left arrow key (Cmd-PageUp or Ctrl-Alt-Left on OS X) – activates the first Si3 tab to the left of the current one
Ctrl+PageDown or Ctrl+Alt+Right arrow key (Cmd-PageDown or Ctrl-Alt-Right on OS X) – activates the first Si3 tab to the right of the current one
Shift+Ctrl+Alt+Left (same on OS X) – moves the active Si3 tab one position to the left in a dock group
Shift+Ctrl+Alt+Right (same on OS X) – moves the active Si3 tab one position to the right in a dock group

j) Font size

Shift+Ctrl+Minus, Ctrl+Plus, Shift+Ctrl+Equals sign, Ctrl+Mouse Wheel, and magnify touchpad gestures – increase and decrease font size

k) Opening & Saving, New Tabs, External IDE

Shift+Ctrl+O (Shift-Control-O on OS X) - open file
Shift+Alt+O (Cmd-Alt-O on OS X) - open file from any folder
Ctrl+S (Control-S on OS X) - saves changes
Ctrl+R (Cmd-Alt-R on OS X) - saves all modified files
Ctrl+T – opens the same file in a new tab
Ctrl+Enter, or Open at Line from the context menu - opens the script in the external editor at the same line where the caret is


6. Support, Bugs, Requests, and Feedback

Feel free to contact Flipbook Games at info@flipbookgames.com or visit http://flipbookgames.com/ for support, bug reports, suggestions, feedback, etc...


A huge thank you to all members of the Unity community involved in the development and beta testing of Script Inspector 3!!! Si3 wouldn't be possible without your help!!! :D


Thank you for purchasing the Script Inspector 3! I hope you will enjoy using it, and that if you do, you will support its future development with suggestions, comments, some nice reviews and maybe even five star ratings ;)

 * Follow me on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join discussion in Unity forums http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 
 
P.S.: Make sure you also check the Favorites Tab[s] http://u3d.as/3hG, another great Unity extension created by Flipbook Games!
