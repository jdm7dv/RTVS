﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.R.Host.Client {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.R.Host.Client.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no local R detected and no remote connection established.
        ///You can try re-connecting to the remote machine, 
        ///use another connection or install local R..
        /// </summary>
        internal static string NoConnectionsAvailable {
            get {
                return ResourceManager.GetString("NoConnectionsAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Interactive Window is disconnected from R Session.
        ///Try connecting to a remote machine in the Workspaces window or
        ///click Reset to restart local R interpreter..
        /// </summary>
        internal static string RHostDisconnected {
            get {
                return ResourceManager.GetString("RHostDisconnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn&apos;t connect session to the &apos;{0}&apos;: {1} because of the following error: {2}.
        /// </summary>
        internal static string RSessionProvider_RestartingSessionFailed {
            get {
                return ResourceManager.GetString("RSessionProvider_RestartingSessionFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Restarting sessions ({0})....
        /// </summary>
        internal static string RSessionProvider_RestartingSessionsFormat {
            get {
                return ResourceManager.GetString("RSessionProvider_RestartingSessionsFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connecting sessions ({0}) to the new R Workspace....
        /// </summary>
        internal static string RSessionProvider_StartConnectingToWorkspaceFormat {
            get {
                return ResourceManager.GetString("RSessionProvider_StartConnectingToWorkspaceFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Start switching to the &apos;{0}&apos;: {1}.
        /// </summary>
        internal static string RSessionProvider_StartSwitchingWorkspaceFormat {
            get {
                return ResourceManager.GetString("RSessionProvider_StartSwitchingWorkspaceFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Switching R Workspace completed.
        /// </summary>
        internal static string RSessionProvider_SwitchingRWorkspaceCompleted {
            get {
                return ResourceManager.GetString("RSessionProvider_SwitchingRWorkspaceCompleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connecting to the new R Workspace failed, restoring connection to the &apos;{0}&apos;: {1}.
        /// </summary>
        internal static string RSessionProvider_SwitchingWorkspaceFailed {
            get {
                return ResourceManager.GetString("RSessionProvider_SwitchingWorkspaceFailed", resourceCulture);
            }
        }
    }
}
