
if (IsMac)
{
  Item (XreItem.Xcode_10_1_0).XcodeSelect ();
  Item ("https://download.visualstudio.microsoft.com/download/pr/edd7782e-dc4f-4be5-9b55-8a51eeb7a718/ed8c238ec095b5d1051e2539cea38113/xamarin.android-10.0.0.40.pkg");
	Item ("https://download.mono-project.com/archive/6.4.0/macos-10-universal/MonoFramework-MDK-6.4.0.189.macos10.xamarin.universal.pkg");

	var dotnetVersion = "2.1.701";
	DotNetCoreSdk (dotnetVersion);
	File.WriteAllText ("../../global.json", @"{ ""sdk"": { ""version"": """ + dotnetVersion + @""" } }");
}