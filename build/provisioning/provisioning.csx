
if (IsMac)
{
  Item (XreItem.Xcode_10_1_0).XcodeSelect ();
  Item ("https://download.visualstudio.microsoft.com/download/pr/edd7782e-dc4f-4be5-9b55-8a51eeb7a718/ed8c238ec095b5d1051e2539cea38113/xamarin.android-10.0.0.40.pkg");
	Item ("https://download.mono-project.com/archive/6.4.0/macos-10-universal/MonoFramework-MDK-6.4.0.189.macos10.xamarin.universal.pkg");
	Item ("https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/xcode11/dc5fc120e090317bbd78049f25dacd84e4f3dc48/254/package/xamarin.ios-12.99.4.5.pkg");
	Item ("https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/xcode11/dc5fc120e090317bbd78049f25dacd84e4f3dc48/254/package/xamarin.mac-5.99.4.5.pkg");
  ForceJavaCleanup();
	Item (XreItem.Java_OpenJDK_1_8_0_25);
	var dotnetVersion = "2.1.701";
	DotNetCoreSdk (dotnetVersion);
	File.WriteAllText ("../../global.json", @"{ ""sdk"": { ""version"": """ + dotnetVersion + @""" } }");

  // VSTS installs into a non-default location. Let's hardcode it here because why not.
	var vstsBaseInstallPath = Path.Combine (Environment.GetEnvironmentVariable ("HOME"), ".dotnet", "sdk");
	var vstsInstallPath = Path.Combine (vstsBaseInstallPath, dotnetVersion);
	var defaultInstallLocation = Path.Combine ("/usr/local/share/dotnet/sdk/", dotnetVersion);
	if (Directory.Exists (vstsBaseInstallPath) && !Directory.Exists (vstsInstallPath))
		ln (defaultInstallLocation, vstsInstallPath);

}


void ln (string source, string destination)
{
	Console.WriteLine ($"ln -sf {source} {destination}");
	if (!Config.DryRun)
		Exec ("/bin/ln", "-sf", source, destination);
}