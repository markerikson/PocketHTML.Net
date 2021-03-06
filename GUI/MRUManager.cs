#region using directives
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
#endregion

#region Comments
// MRI list manager.
//
// Written by: Alex Farber
//
// alexm@cmt.co.il

/*******************************************************************************

Using:

1) Add menu item Recent Files (or any name you want) to main application menu.
   This item is used by MRUManager as popup menu for MRU list.

2) Implement IMRUClient inteface in the form class:

public class Form1 : System.Windows.Forms.Form, IMRUClient
{
     public void OpenMRUFile(string fileName)
     {
         // open file here
     }
 
     // ...    
}

3) Add MRUManager member to the form class and initialize it:

     private MRUManager mruManager;

     private void Form1_Load(object sender, System.EventArgs e)
     {
         mruManager = new MRUManager();
         mruManager.Initialize(
             this,                              // owner form
             mnuFileMRU,                        // Recent Files menu item
             "Software\\MyCompany\\MyProgram"); // Registry path to keep MRU list

        // Optional. Call these functions to change default values:

        mruManager.CurrentDir = ".....";           // default is current directory
        mruManager.MaxMRULength = ...;             // default is 10
        mruMamager.MaxDisplayNameLength = ...;     // default is 40
     }

     NOTES:
     - If Registry path is, for example, "Software\MyCompany\MyProgram",
       MRU list is kept in
       HKEY_CURRENT_USER\Software\MyCompany\MyProgram\MRU Registry entry.

     - CurrentDir is used to show file names in the menu. If file is in
       this directory, only file name is shown.

4) Call MRUManager Add and Remove functions when necessary:

       mruManager.Add(fileName);          // when file is successfully opened

       mruManager.Remove(fileName);       // when Open File operation failed

*******************************************************************************/

// Implementation details:
//
// MRUManager loads MRU list from Registry in Initialize function.
// List is saved in Registry when owner form is closed.
//
// MRU list in the menu is updated when parent menu is poped-up.
//
// Owner form OpenMRUFile function is called when user selects file
// from MRU list.
#endregion

namespace MRUSample
{
    /// <summary>
    /// Interface which should be implemented by owner form
    /// to use MRUManager.
    /// </summary>
    public interface IMRUClient
    {
        void OpenMRUFile(string fileName);
    }

	/// <summary>
	/// MRU manager - manages Most Recently Used Files list
	/// for Windows Form application.
	/// </summary>
	public class MRUManager
	{
        #region Members

        private Form ownerForm;                 // owner form

        private MenuItem menuItemMRU;           // Recent Files menu item
        private MenuItem menuItemParent;        // Recent Files menu item parent

        //private string registryPath;            // Registry path to keep MRU list

        private int maxNumberOfFiles = 5;      // maximum number of files in MRU list

        private int maxDisplayLength = 30;      // maximum length of file name for display

        private string currentDirectory;        // current directory

        private ArrayList mruList;              // MRU list (file names)
		private ArrayList itemsList;

        private const string regEntryName = "file";  // entry name to keep MRU (file0, file1...)


        #endregion

        #region Constructor

		public MRUManager()
		{
            mruList = new ArrayList();
			itemsList = new ArrayList();
		}

        #endregion

        #region Public Properties

        /// <summary>
        /// Maximum length of displayed file name in menu (default is 40).
        /// 
        /// Set this property to change default value (optional).
        /// </summary>
        public int MaxDisplayNameLength
        {
            set
            {
                maxDisplayLength = value;

                if ( maxDisplayLength < 10 )
                    maxDisplayLength = 10;
            }

            get
            {
                return maxDisplayLength;
            }
        }

        /// <summary>
        /// Maximum length of MRU list (default is 10).
        /// 
        /// Set this property to change default value (optional).
        /// </summary>
        public int MaxMRULength
        {
            set
            {
                maxNumberOfFiles = value;

                if ( maxNumberOfFiles < 1 )
                    maxNumberOfFiles = 1;

                if ( mruList.Count > maxNumberOfFiles )
                    mruList.RemoveRange(maxNumberOfFiles - 1, mruList.Count - maxNumberOfFiles);
            }

            get
            {
                return maxNumberOfFiles;
            }
        }

        /// <summary>
        /// Set current directory.
        /// 
        /// Default value is program current directory which is set when
        /// Initialize function is called.
        /// 
        /// Set this property to change default value (optional)
        /// after call to Initialize.
        /// </summary>
        public string CurrentDir
        {
            set
            {
                currentDirectory = value;
            }

            get
            {
                return currentDirectory;
            }
        }

		public string[] FileNames
		{
			get
			{
				string[] filenames = (string[])mruList.ToArray(typeof(string));
				return filenames;
			}
		}

        #endregion

        #region Public Functions

        /// <summary>
        /// Initialization. Call this function in form Load handler.
        /// </summary>
        /// <param name="owner">Owner form</param>
        /// <param name="mruItem">Recent Files menu item</param>
        /// <param name="regPath">Registry Path to keep MRU list</param>
        public void Initialize(Form owner, MenuItem mruItem)
        {
            // keep reference to owner form
            ownerForm = owner;

            // check if owner form implements IMRUClient interface
            if ( ! ( owner is IMRUClient ) )
            {
                throw new Exception(
                    "MRUManager: Owner form doesn't implement IMRUClient interface");
            }

            // keep reference to MRU menu item
            menuItemMRU = mruItem;

            // keep reference to MRU menu item parent
            try
            {
                menuItemParent = (MenuItem) menuItemMRU.Parent;
            }
            catch
            {
            }

            if ( menuItemParent == null )
            {
                throw new Exception(
                    "MRUManager: Cannot find parent of MRU menu item");
            }

            // keep current directory in the time of initialization
            //currentDirectory = Directory.GetCurrentDirectory();

            // subscribe to MRU parent Popup event
            menuItemParent.Popup += new EventHandler(this.OnMRUParentPopup);

            // subscribe to owner form Closing event
            //ownerForm.Closing += new System.ComponentModel.CancelEventHandler(OnOwnerClosing);

            // load MRU list from Registry
            //LoadMRU();
        }

        /// <summary>
        /// Add file name to MRU list.
        /// Call this function when file is opened successfully.
        /// If file already exists in the list, it is moved to the first place.
        /// </summary>
        /// <param name="file">File Name</param>
        public void Add(string file)
        {
            Remove(file);

            // if array has maximum length, remove last element
            if ( mruList.Count == maxNumberOfFiles )
                mruList.RemoveAt(maxNumberOfFiles - 1);

            // add new file name to the start of array
            mruList.Insert(0, file);
        }

		public void AddRange(string[] files)
		{
			for(int i = 0; i < files.Length; i++)
			{
				Add(files[i]);
			}
		}

		/// <summary>
		/// Remove file name from MRU list.
		/// Call this function when File - Open operation failed.
		/// </summary>
		/// <param name="file">File Name</param>
		public void Remove(string file)
        {
            int i = 0;

            IEnumerator myEnumerator = mruList.GetEnumerator();

            while ( myEnumerator.MoveNext() )
            {
                if ( (string)myEnumerator.Current == file )
                {
                    mruList.RemoveAt(i);
                    return;
                }

                i++;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Update MRU list when MRU menu item parent is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMRUParentPopup(object sender, EventArgs e)
        {
			
            // remove all childs
            if (menuItemMRU.MenuItems.Count > 0)// menuItemMRU.IsParent )
            {
                menuItemMRU.MenuItems.Clear();
            }

            // Disable menu item if MRU list is empty
            if ( mruList.Count == 0 )
            {
                menuItemMRU.Enabled = false;
                return;
            }

            // enable menu item and add child items
            menuItemMRU.Enabled = true;

            MenuItem item;

            IEnumerator myEnumerator = mruList.GetEnumerator();

			itemsList.Clear();

            while ( myEnumerator.MoveNext() )
            {
                item = new MenuItem();
				item.Text = GetDisplayName((string)myEnumerator.Current);
				//itemsList.Add(item);

                // subscribe to item's Click event
                item.Click += new EventHandler(this.OnMRUClicked);

                menuItemMRU.MenuItems.Add(item);
            }
        }

        /// <summary>
        /// MRU menu item is clicked - call owner's OpenMRUFile function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMRUClicked(object sender, EventArgs e)
        {
            string s;

            try
            {
                // cast sender object to MenuItem
                MenuItem item = (MenuItem) sender;

                if ( item != null )
                {
                    // Get file name from list using item index
					//s = item.Text;//(string)mruList[item.Index];

					int itemIndex = item.Parent.MenuItems.IndexOf(item);
					s = (string)mruList[itemIndex];
                    // call owner's OpenMRUFile function
                    if ( s.Length > 0 )
                    {
                        ((IMRUClient)ownerForm).OpenMRUFile(s);
                    }
                }
            }
            catch ( Exception ex )
            {
				Debug.WriteLine("Exception in OnMRUClicked: " + ex.Message);
                //Trace.WriteLine("Exception in OnMRUClicked: " + ex.Message);
            }
        }

        /// <summary>
        /// Save MRU list in Registry when owner form is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		/*
        private void OnOwnerClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int i, n;

            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath);

                if ( key != null )
                {
                    n = mruList.Count;

                    for ( i = 0; i < maxNumberOfFiles; i++ )
                    {
                        key.DeleteValue(regEntryName + i.ToString(), false);
                    }

                    for ( i = 0; i < n; i++ )
                    {
                        key.SetValue(regEntryName + i.ToString(), mruList[i]);
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine("Saving MRU to Registry failed: " + ex.Message);
            }
        }
		*/

        #endregion

        #region Private Functions

		static public String ShortenPathname(String pathname, int maxLength)
		{
			if (pathname.Length <= maxLength)
				return pathname;

			String root = Path.GetPathRoot(pathname);
			if (root.Length > 3)
				root += Path.DirectorySeparatorChar;

			String[] elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			int filenameIndex = elements.GetLength(0) - 1;

			if (elements.GetLength(0) == 1) // pathname is just a root and filename
			{
				if (elements[0].Length > 5) // long enough to shorten
				{
					// if path is a UNC path, root may be rather long
					if (root.Length + 6 >= maxLength)
					{
						return root + elements[0].Substring(0, 3) + "...";
					}
					else
					{
						return pathname.Substring(0, maxLength - 3) + "...";
					}
				}
			}
			else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
			{
				root += "...\\";

				int len = elements[filenameIndex].Length;
				if (len < 6)
					return root + elements[filenameIndex];

				if ((root.Length + 6) >= maxLength)
				{
					len = 3;
				}
				else
				{
					len = maxLength - root.Length - 3;
				}
				return root + elements[filenameIndex].Substring(0, len) + "...";
			}
			else if (elements.GetLength(0) == 2)
			{
				return root + "...\\" + elements[1];
			}
			else
			{
				int len = 0;
				int begin = 0;

				for (int i = 0; i < filenameIndex; i++)
				{
					if (elements[i].Length > len)
					{
						begin = i;
						len = elements[i].Length;
					}
				}

				int totalLength = pathname.Length - len + 3;
				int end = begin + 1;

				while (totalLength > maxLength)
				{
					if (begin > 0)
						totalLength -= elements[--begin].Length - 1;

					if (totalLength <= maxLength)
						break;

					if (end < filenameIndex)
						totalLength -= elements[++end].Length - 1;

					if (begin == 0 && end == filenameIndex)
						break;
				}

				// assemble final string

				for (int i = 0; i < begin; i++)
				{
					root += elements[i] + '\\';
				}

				root += "...\\";

				for (int i = end; i < filenameIndex; i++)
				{
					root += elements[i] + '\\';
				}

				return root + elements[filenameIndex];
			}
			return pathname;
		}


        /// <summary>
        /// Get display file name from full name.
        /// </summary>
        /// <param name="fullName">Full file name</param>
        /// <returns>Short display name</returns>
        private string GetDisplayName(string fullName)
        {
			return ShortenPathname(fullName, MaxDisplayNameLength);

        }
        #endregion

	}
}
