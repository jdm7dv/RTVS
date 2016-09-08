﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Common.Core.IO;
using Microsoft.Common.Core.OS;
using Microsoft.Win32;
using static System.FormattableString;

namespace Microsoft.R.Interpreters {
    /// <summary>
    /// Verifies that R is installed in the folder
    /// specified in settings. If nothing is specified
    /// settings try and find highest version.
    /// </summary>
    public sealed class RInstallation {
        private const string _rCoreRegKey = @"SOFTWARE\R-core\R";
        private const string _rServer = "R_SERVER";
        private const string _installPathValueName = "InstallPath";
        private static readonly string[] rFolders = new string[] { "MRO", "RRO", "R" };

        private readonly IRegistry _registry;
        private readonly IFileSystem _fileSystem;

        public RInstallation() :
            this(new RegistryImpl(), new FileSystem()) { }

        public RInstallation(IRegistry registry, IFileSystem fileSystem) {
            _registry = registry;
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Retrieves path to the latest (highest version) R installation
        /// from registry. Typically in the form 'Program Files\R\R-3.2.1'
        /// Selects highest from compatible versions, not just the highest.
        /// </summary>
        public IEnumerable<IRInterpreterInfo> GetCompatibleEngines(ISupportedRVersionRange svl = null) {
            var mrc = GetMicrosoftRClientInfo();
            var engines = GetCompatibleEnginesFromRegistry(svl);
            engines = engines.Where(e => e.VerifyInstallation(svl, _fileSystem)).OrderBy(e => e.Version);
            if(mrc != null) {
                var list = new List<IRInterpreterInfo>() { mrc };
                list.AddRange(engines);
                return list;
            } else if (!engines.Any()) {
                var e = TryFindRInProgramFiles(svl);
                if (e != null) {
                    return new List<IRInterpreterInfo>() { e };
                }
            }
            return engines;
        }

        /// <summary>
        /// Retrieves path to the all compatible R installations from registry. 
        /// </summary>
        private IEnumerable<IRInterpreterInfo> GetCompatibleEnginesFromRegistry(ISupportedRVersionRange svr) {
            svr = svr ?? new SupportedRVersionRange();
            return GetInstalledEnginesFromRegistry().Where(e => svr.IsCompatibleVersion(e.Version));
        }

        /// <summary>
        /// Retrieves information on installed R versions in registry.
        /// </summary>
        private IEnumerable<IRInterpreterInfo> GetInstalledEnginesFromRegistry() {
            List<IRInterpreterInfo> engines = new List<IRInterpreterInfo>();

            // HKEY_LOCAL_MACHINE\SOFTWARE\R-core
            // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\R-core
            // HKEY_LOCAL_MACHINE\SOFTWARE\R-core\R64\3.3.0 Pre-release
            using (IRegistryKey hklm = _registry.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)) {
                try {
                    using (var rKey = hklm.OpenSubKey(@"SOFTWARE\R-core\R")) {
                        foreach (var name in rKey.GetSubKeyNames()) {
                            using (var subKey = rKey.OpenSubKey(name)) {
                                var path = subKey.GetValue(_installPathValueName) as string;
                                if (!string.IsNullOrEmpty(path)) {
                                    engines.Add(new RInterpreterInfo(Invariant($"R {name}"), path, _fileSystem));
                                }
                            }
                        }
                    }
                } catch (Exception) { }
            }
            return engines;
        }

        public IRInterpreterInfo GetMicrosoftRClientInfo() {
            // First check that MRS is present on the machine.
            bool mrsInstalled = false;
            try {
                using (var hklm = _registry.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)) {
                    using (var key = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\130\sql_shared_mr")) {
                        var path = (string)key?.GetValue("Path");
                        if (!string.IsNullOrEmpty(path) && path.Contains(_rServer)) {
                            mrsInstalled = true;
                        }
                    }
                }
            } catch (Exception) { }

            // If yes, check 32-bit registry for R engine installed by the R Server.
            // TODO: remove this when MRS starts writing 64-bit keys.
            if (mrsInstalled) {
                using (IRegistryKey hklm = _registry.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)) {
                    try {
                        using (var key = hklm.OpenSubKey(@"SOFTWARE\R-core\R64")) {
                            foreach (var keyName in key.GetSubKeyNames()) {
                                using (var rsKey = key.OpenSubKey(keyName)) {
                                    try {
                                        var path = (string)rsKey?.GetValue(_installPathValueName);
                                        if (!string.IsNullOrEmpty(path) && path.Contains(_rServer)) {
                                            var info = new RInterpreterInfo(string.Empty, path);
                                            if (info.VerifyInstallation(new SupportedRVersionRange(), _fileSystem)) {
                                                return new RInterpreterInfo(Invariant($"Microsoft R Client ({info.Version.Major}.{info.Version.Minor}.{info.Version.Build})"), info.InstallPath);
                                            }
                                        }
                                    } catch (Exception) { }
                                }
                            }
                        }
                    } catch (Exception) { }
                }
            }

            return null;
        }

        private static Version GetRVersionFromFolderName(string folderName) {
            if (folderName.StartsWith("R-", StringComparison.OrdinalIgnoreCase)) {
                try {
                    Version v;
                    if (Version.TryParse(folderName.Substring(2), out v)) {
                        return v;
                    }
                } catch (Exception) { }
            }
            return new Version(0, 0);
        }

        private IRInterpreterInfo TryFindRInProgramFiles(ISupportedRVersionRange supportedVersions) {
            // Force 64-bit PF
            var programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            var baseRFolder = Path.Combine(programFiles, @"R");
            var versions = new List<Version>();
            try {
                if (_fileSystem.DirectoryExists(baseRFolder)) {
                    IEnumerable<IFileSystemInfo> directories = _fileSystem.GetDirectoryInfo(baseRFolder)
                                                                    .EnumerateFileSystemInfos()
                                                                    .Where(x => (x.Attributes & FileAttributes.Directory) != 0);
                    foreach (IFileSystemInfo fsi in directories) {
                        string subFolderName = fsi.FullName.Substring(baseRFolder.Length + 1);
                        Version v = GetRVersionFromFolderName(subFolderName);
                        if (supportedVersions.IsCompatibleVersion(v)) {
                            versions.Add(v);
                        }
                    }
                }
            } catch (IOException) {
                // Don't do anything if there is no RRO installed
            }

            if (versions.Count > 0) {
                versions.Sort();
                Version highest = versions[versions.Count - 1];
                var name = string.Format(CultureInfo.InvariantCulture, "R-{0}.{1}.{2}", highest.Major, highest.Minor, highest.Build);
                var path = Path.Combine(baseRFolder, name);
                return new RInterpreterInfo(name, path);
            }

            return null;
        }
    }
}
