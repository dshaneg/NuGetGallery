// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace NuGetGallery
{
    public static class TestPackage
    {
        public static void WriteNuspec(
            Stream stream,
            bool leaveStreamOpen,
            string id,
            string version,
            string title = "Package Id",
            string summary = "Package Summary",
            string authors = "Package author",
            string owners = "Package owners",
            string description = "Package Description",
            string tags = "Package tags",
            string language = null,
            string copyright = null,
            string releaseNotes = null,
            string minClientVersion = null,
            Uri licenseUrl = null,
            Uri projectUrl = null,
            Uri iconUrl = null,
            bool requireLicenseAcceptance = false)
        {
            using (var streamWriter = new StreamWriter(stream, new UTF8Encoding(false, true), 1024, leaveStreamOpen))
            {
                streamWriter.WriteLine(@"<?xml version=""1.0""?>
                    <package xmlns=""http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd"">
                        <metadata" + (!string.IsNullOrEmpty(minClientVersion) ? @" minClientVersion=""" + minClientVersion + @"""" : string.Empty) + @">
                            <id>" + id + @"</id>
                            <version>" + version + @"</version>
                            <title>" + title + @"</title>
                            <summary>" + summary + @"</summary>
                            <description>" + description + @"</description>
                            <tags>" + tags + @"</tags>
                            <requireLicenseAcceptance>" + requireLicenseAcceptance + @"</requireLicenseAcceptance>
                            <authors>" + authors + @"</authors>
                            <owners>" + owners + @"</owners>
                            <language>" + (language ?? string.Empty) + @"</language>
                            <copyright>" + (copyright ?? string.Empty) + @"</copyright>
                            <releaseNotes>" + (releaseNotes ?? string.Empty) + @"</releaseNotes>
                            <licenseUrl>" + (licenseUrl != null ? licenseUrl.ToString() : string.Empty) + @"</licenseUrl>
                            <projectUrl>" + (projectUrl != null ? projectUrl.ToString() : string.Empty) + @"</projectUrl>
                            <iconUrl>" + (iconUrl != null ? iconUrl.ToString() : string.Empty) + @"</iconUrl>
                        </metadata>
                    </package>");
            }
        }

        public static Stream CreateTestPackageStream(
            string id,
            string version,
            string title = "Package Id",
            string summary = "Package Summary",
            string authors = "Package author",
            string owners = "Package owners",
            string description = "Package Description",
            string tags = "Package tags",
            string language = null,
            string copyright = null,
            string releaseNotes = null,
            string minClientVersion = null,
            Uri licenseUrl = null,
            Uri projectUrl = null,
            Uri iconUrl = null,
            bool requireLicenseAcceptance = false,
            Action<ZipArchive> populatePackage = null)
        {
            return CreateTestPackageStream(packageArchive =>
            {
                var nuspecEntry = packageArchive.CreateEntry(id + ".nuspec", CompressionLevel.Fastest);
                using (var stream = nuspecEntry.Open())
                {
                    WriteNuspec(stream, true, id, version, title, summary, authors, owners, description, tags, language, copyright, releaseNotes, minClientVersion, licenseUrl, projectUrl, iconUrl, requireLicenseAcceptance);
                }

                if (populatePackage != null)
                {
                    populatePackage(packageArchive);
                }
            });
        }

        public static Stream CreateTestPackageStreamFromNuspec(string id, string nuspec, Action<ZipArchive> populatePackage = null)
        {
            return CreateTestPackageStream(packageArchive =>
            {
                var nuspecEntry = packageArchive.CreateEntry(id + ".nuspec", CompressionLevel.Fastest);
                using (var streamWriter = new StreamWriter(nuspecEntry.Open()))
                {
                    streamWriter.WriteLine(nuspec);
                }

                if (populatePackage != null)
                {
                    populatePackage(packageArchive);
                }
            });
        }

        public static Stream CreateTestPackageStream(Action<ZipArchive> populatePackage)
        {
            var packageStream = new MemoryStream();
            using (var packageArchive = new ZipArchive(packageStream, ZipArchiveMode.Create, true))
            {
                if (populatePackage != null)
                {
                    populatePackage(packageArchive);
                }
            }

            packageStream.Position = 0;

            return packageStream;
        }
    }
}