﻿using Microsoft.R.Host.Client;
using Microsoft.VisualStudio.R.Package.Commands;
using Microsoft.VisualStudio.R.Package.Shell;
using Microsoft.VisualStudio.R.Packages.R;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.R.Package.Repl.Debugger {
    internal abstract class DebuggerCommand : PackageCommand {
        protected readonly IRSessionProvider _rSessionProvider;
        private DebuggerCommandVisibility _visibility;

        public DebuggerCommand(IRSessionProvider rSessionProvider, int cmdId, DebuggerCommandVisibility visibility)
            : base(RGuidList.RCmdSetGuid, cmdId) {
            _rSessionProvider = rSessionProvider;
            _visibility = visibility;
        }

        protected override void SetStatus() {
            Enabled = false;
            Visible = false;

            if (_rSessionProvider.Current == null) {
                return;
            }

            var debugger = VsAppShell.Current.GetGlobalService<IVsDebugger>(typeof(SVsShellDebugger));
            if (debugger == null) {
                return;
            }

            var mode = new DBGMODE[1];
            if (debugger.GetMode(mode) < 0) {
                return;
            }

            if (mode[0] == DBGMODE.DBGMODE_Design) {
                if (_visibility == DebuggerCommandVisibility.DesignMode) {
                    Enabled = true;
                    Visible = true;
                }
                return;
            }

            if ((_visibility & DebuggerCommandVisibility.DebugMode) > 0) {
                Visible = true;

                if (mode[0] == DBGMODE.DBGMODE_Break) {
                    Enabled = (_visibility & DebuggerCommandVisibility.Stopped) > 0;
                    return;
                }

                if (mode[0] == DBGMODE.DBGMODE_Run) {
                    Enabled = (_visibility & DebuggerCommandVisibility.Run) > 0;
                    return;
                }
            }
        }
    }
}
